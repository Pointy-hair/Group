using Microsoft.Extensions.Options;
using MimeKit.Cryptography;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Crypto;
using RevolutionaryStuff.Core.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;

namespace Traffk.BackgroundJobServer
{
    public class DataSourceSyncRunner : BaseJobRunner, IDataSourceSyncJobs
    {
        private readonly IOptions<BlobStorageServices.BlobStorageServicesOptions> BlobOptions;
        private readonly IVault Vault;
        
        public DataSourceSyncRunner(
            JobRunnerProgram jobRunnerProgram, 
            TraffkGlobalsContext gdb, 
            IVault vault,
            IOptions<BlobStorageServices.BlobStorageServicesOptions> blobOptions, 
            Serilog.ILogger logger)
            : base(gdb, jobRunnerProgram, logger)
        {
            BlobOptions = blobOptions;
            Vault = vault;
        }

        private static bool IsPgpEncrypted(string fullPath)
        {
            if (fullPath == null || !File.Exists(fullPath)) return false;
            using (var st = File.OpenRead(fullPath))
            {
                return IsPgpEncrypted(st);
            }
        }

        private static bool IsPgpEncrypted(Stream st)
        {
            var sr = new StreamReader(st);
            var buf = new char[32];
            var count = sr.ReadBlock(buf, 0, buf.Length);
            var s = new string(buf, 0, count);
            return s.StartsWith("-----BEGIN PGP MESSAGE-----");
        }

        private class MyGnuPGContext : GnuPGContext
        {
            private readonly string Password;

            public MyGnuPGContext(string password)
            {
                Password = password;
            }

            protected override string GetPasswordForKey(PgpSecretKey key)
                => Password;
        }

        private static readonly object DecryptLock = new object();

        void Decrypt(Stream st, string destinationFile)
        {
            var pw = Vault.GetSecretAsync(Bal.Services.Vault.CommonSecretUris.TraffkPgpPrivateKeyPasswordUri).ExecuteSynchronously();
            lock (DecryptLock)
            {
                using (var ctx = new MyGnuPGContext(pw))
                {
                    var sk = Vault.GetSecretAsync(Bal.Services.Vault.CommonSecretUris.TraffkPgpPrivateKeyUri).ExecuteSynchronously();
                    using (var skst = StreamHelpers.Create(sk))
                    {
                        ctx.ImportSecretKeys(skst);
                    }
                    using (var decryptedStream = ctx.GetDecryptedStream(st))
                    {
                        using (var destinationStream = File.Create(destinationFile))
                        {
                            decryptedStream.CopyTo(destinationStream);
                        }
                    }
                }
            }
        }


        void IDataSourceSyncJobs.DataSourceFetch(int dataSourceId)
            => new Fetcher(this, dataSourceId).Fetch();

        private class Fetcher
        {
            private readonly DataSourceSyncRunner Runner;
            private readonly DataSource DS;
            TraffkGlobalsContext Gdb => Runner.GlobalContext;
            private readonly IDictionary<string, DataSourceFetchItem> FetchItemByEvidence = new Dictionary<string, DataSourceFetchItem>();
            private readonly string BlobRootPath;

            public Fetcher(DataSourceSyncRunner runner, int dataSourceId)
            {
                Runner = runner;
                DS = Gdb.DataSources.Find(dataSourceId);
                PopulateEvidence(dataSourceId);
                BlobRootPath = $"{BlobStorageServices.GetDataSourceFetchItemRoot(null, dataSourceId)}{Runner.StartedAtUtc.ToYYYYMMDD()}/{Runner.StartedAtUtc.ToHHMMSS()}/";
            }

            private DataSourceFetchItem FindEvidenceItem(string key)
                => FetchItemByEvidence.FindOrDefault(key);

            private void PopulateEvidence(DataSourceFetchItem item)
            {
                foreach (var e in item.Evidence.WhereNotNull())
                {
                    FetchItemByEvidence[e] = item;
                }
            }

            private void PopulateEvidence(int dataSourceId)
            {
                var t = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate.ToString();
                var items = from i in Gdb.DataSourceFetchItems
                            where i.DataSourceFetch.DataSourceId == dataSourceId && i.DataSourceFetchItemTypeStringValue!=t
                            select i;
                foreach (var item in items)
                {
                    PopulateEvidence(item);
                }
            }

            public void Fetch()
            {
                lock (typeof(DataSourceSyncRunner))
                {
                    var fetch = new DataSourceFetche
                    {
                        DataSource = DS
                    };
                    Gdb.DataSourceFetches.Add(fetch);
                    if (DS.DataSourceSettings.IsFtp)
                    {
                        ProcessFtpFetch(fetch, DS.DataSourceSettings.FTP);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unrecognized datasource");
                    }
                    Gdb.SaveChanges();
                }
            }

            void ProcessFtpFetch(DataSourceFetche fetch, DataSourceSettings.FtpSettings ftpSettings)
            {
                Requires.NonNull(ftpSettings, nameof(ftpSettings));
                var cred = Runner.Vault.GetCredentialsAsync(ftpSettings.CredentialsKeyUri).ExecuteSynchronously();
                var ci = new ConnectionInfo(ftpSettings.Hostname, ftpSettings.Port, cred.Username, new PasswordAuthenticationMethod(cred.Username, cred.Password));
                using (var client = new SftpClient(ci))
                {
                    client.Connect();
                    Parallel.ForEach(
                        ftpSettings.FolderPaths,
//                        new ParallelOptions { MaxDegreeOfParallelism = 1 },
                        fp => FetchFtpFolderFiles(client, fetch, fp));
                }
            }

            private readonly HashSet<string> PathsChecked = new HashSet<string>();

            private bool IsAlreadyVisited(string path)
            {
                lock (PathsChecked)
                {
                    if (PathsChecked.Contains(path)) return true;
                    PathsChecked.Add(path);
                }
                return false;
            }

            private class FileDetails
            {
                public readonly string Path;
                public readonly string FullName;
                public readonly string Name;
                public readonly long Length;
                public readonly DateTime? LastWriteTimeUtc;

                private static string ToUnixPath(string windowsPath)
                    => windowsPath.Replace('\\', '/');

                private static string ToWindowsPath(string unixPath)
                    => unixPath.Replace('/', '\\');

                public FileDetails(FileDetails baseDetails, string name=null)
                {
                    FullName = ToWindowsPath(baseDetails.FullName);
                    Name = name ?? System.IO.Path.GetFileName(FullName);
                    Path = System.IO.Path.GetDirectoryName(FullName);

                    FullName = ToUnixPath(FullName);
                    Path = ToUnixPath(Path);
                }

                public FileDetails(SftpFile f)
                {
                    FullName = f.FullName;
                    Name = f.Name;
                    Path = ToUnixPath(System.IO.Path.GetDirectoryName(ToWindowsPath(FullName)));
                    Length = f.Length;
                    LastWriteTimeUtc = f.LastWriteTimeUtc;
                }
            }

            private void FetchTheItem(DataSourceFetche fetch, FileDetails details, string folder, DataSourceFetchItem.DataSourceFetchItemTypes dataSourceFetchItemType,  DataSourceFetchItem parentFetchItem, Func<FileDetails, string> fetcher)
            {
                string tfn = null;
                var item = new DataSourceFetchItem
                {
                    DataSourceFetch = fetch,
                    DataSourceFetchItemType = dataSourceFetchItemType,
                    ParentDataSourceFetchItem = parentFetchItem,
                    Size = details.Length,
                    Name = details.Name,
                };
                try
                {
                    Trace.WriteLine($"Checking {details.FullName} size={details.Length} LastWriteTimeUtc={details.LastWriteTimeUtc}");
                    var sameDataSourceReplicatedDataSourceFetchItem =
                        FindEvidenceItem(DataSourceFetchItem.CreateEvidenceFromNameSizeModified(details.FullName, details.Length, details.LastWriteTimeUtc)) ??
                        FindEvidenceItem(DataSourceFetchItem.CreateEvidenceFromNameSizeModified(details.Name, details.Length, details.LastWriteTimeUtc));
                    if (sameDataSourceReplicatedDataSourceFetchItem!=null)
                    {
                        item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate;
                        item.SameDataSourceReplicatedDataSourceFetchItem = sameDataSourceReplicatedDataSourceFetchItem;
                        return;
                    }
                    //                      Logger.LogInformation("Downloading", file.FullName, file.Length, tfn);
                    try
                    {
                        tfn = fetcher(details);
                        var fi = new FileInfo(tfn);
                        item.Size = fi.Length;
                    }
                    catch (Exception)
                    {
                        item = null;
                        return;
                    }
                    using (var st = File.OpenRead(tfn))
                    {
                        using (var muxer = new StreamMuxer(st, true))
                        {
                            var p = new BlobStorageServices.FileProperties
                            {
                                LastModifiedAtUtc = details.LastWriteTimeUtc
                            };
                            p.Metadata[BlobStorageServices.MetaKeyNames.SourcePath] = details.Path;
                            p.Metadata[BlobStorageServices.MetaKeyNames.SourceFullName] = details.FullName;
                            if (IsPgpEncrypted(muxer.OpenRead()))
                            {
                                p.Metadata[BlobStorageServices.MetaKeyNames.IsPgpEncrypted] = JsonConvert.SerializeObject(true);
                            }
                            var urns = new List<string>();
                            Parallel.ForEach(
                                new[]
                                {
                                            Hash.CommonHashAlgorithmNames.Md5,
                                            Hash.CommonHashAlgorithmNames.Sha1,
                                            Hash.CommonHashAlgorithmNames.Sha512,
                                },
                                hashAlgName => urns.Add(Hash.Compute(muxer.OpenRead(), hashAlgName).Urn));
                            if (urns.Count > 0)
                            {
                                p.Metadata[BlobStorageServices.MetaKeyNames.Urns] = CSV.FormatLine(urns, false);
                                foreach (var urn in urns)
                                {
                                    sameDataSourceReplicatedDataSourceFetchItem =
                                        FindEvidenceItem(DataSourceFetchItem.CreateEvidenceFromUrn(urn));
                                    if (sameDataSourceReplicatedDataSourceFetchItem!=null)
                                    {
                                        item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate;
                                        item.SameDataSourceReplicatedDataSourceFetchItem = sameDataSourceReplicatedDataSourceFetchItem;
                                        return;
                                    }
                                }
                            }
                            var res = BlobStorageServices.StoreStreamAsync(
                                Runner.BlobOptions,
                                BlobStorageServices.ContainerNames.Secure,
                                $"{BlobRootPath}{details.Path.Substring(1)}/{folder}/{details.Name}",
                                muxer.OpenRead(),
                                p,
                                amt => Trace.WriteLine($"Uploading {amt}/{details.Length}")
                                ).ExecuteSynchronously();
                            item.DataSourceFetchItemProperties = new DataSourceFetchItemProperties();
                            item.DataSourceFetchItemProperties.Set(p);
                            item.Url = res.Uri.ToString();
                            PopulateEvidence(item);
                        }
                    }
                }
                finally
                {
                    if (item != null)
                    {
                        lock (Gdb)
                        {
                            Gdb.DataSourceFetchItems.Add(item);
                            Gdb.SaveChanges();
                        }
                        if (IsPgpEncrypted(tfn))
                        {
                            var name = details.Name;
                            if (name.ToLower().EndsWith(".pgp"))
                            {
                                name = name.Left(name.Length - 4);
                            }
                            else if (name.ToLower().EndsWith(".pgp.asc"))
                            {
                                name = name.Left(name.Length - 8);
                            }
                            else if (name.ToLower().Contains(".pgp."))
                            {
                                name = new Regex(@"\.pgp\.", RegexOptions.IgnoreCase).Replace(name, ".");
                            }
                            FetchTheItem(
                                fetch,
                                new FileDetails(details, name),
                                "decrypted",
                                DataSourceFetchItem.DataSourceFetchItemTypes.Decrypted,
                                item,
                                _ =>
                                {
                                    var utfp = Path.GetTempFileName();
                                    using (var st = File.OpenRead(tfn))
                                    {
                                        Runner.Decrypt(st, utfp);
                                    }
                                    return utfp;
                                }
                                );
                        }
                    }
                    Stuff.FileTryDelete(tfn);
                }
            }

            private void FetchFtpFolderFiles(SftpClient client, DataSourceFetche fetch, string path)
            {
                if (IsAlreadyVisited(path)) return;
                client.ChangeDirectory(path);
                var entries = client.ListDirectory(path);
                Parallel.ForEach(
                    entries, 
//                    new ParallelOptions { MaxDegreeOfParallelism = 1 }, 
                    delegate (SftpFile file)
                {
                    if (IsAlreadyVisited(file.FullName)) return;
                    if (file.IsDirectory)
                    {
                        FetchFtpFolderFiles(client, fetch, file.FullName);
                    }
                    else if (file.IsRegularFile)
                    {
                        FetchTheItem(
                            fetch, 
                            new FileDetails(file),
                            "raw",
                            DataSourceFetchItem.DataSourceFetchItemTypes.Original,
                            null,
                            fd => 
                            {
                                var fn = Path.GetTempFileName();
                                using (var st = File.Create(fn))
                                {
                                    Trace.WriteLine($"Starting {fd.FullName} to [{fn}]");
                                    client.DownloadFile(fd.FullName, st, amt => Trace.WriteLine($"Downloading {fd.FullName} to [{fn}] => {amt}/{fd.Length}"));// Logger.LogDebug("Downloading", amt, file.Length));
                                    Trace.WriteLine($"Finishing {fd.FullName} to [{fn}]");
                                }
                                return fn;
                            });
                    }
                });
            }
        }
    }
}
