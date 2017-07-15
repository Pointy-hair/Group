using H = HtmlAgilityPack;
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
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Bal;
using System.Net;

namespace Traffk.BackgroundJobServer
{
    public partial class DataSourceSyncRunner : BaseJobRunner, IDataSourceSyncJobs
    {
        private readonly IOptions<BlobStorageServices.Config> BlobOptions;
        private readonly IVault Vault;
        private readonly IHttpClientFactory HttpClientFactory;

        public DataSourceSyncRunner(
            IJobInfoFinder jobInfoFinder,
            IHttpClientFactory httpClientFactory,
            JobRunnerProgram jobRunnerProgram, 
            TraffkGlobalDbContext gdb, 
            IVault vault,
            IOptions<BlobStorageServices.Config> blobOptions, 
            Serilog.ILogger logger)
            : base(gdb, jobInfoFinder, logger)
        {
            HttpClientFactory = httpClientFactory;
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


        Task IDataSourceSyncJobs.DataSourceFetchAsync(int dataSourceId)
            => new Fetcher(this, dataSourceId).FetchAsync();

        public partial class Fetcher
        {
            private readonly DataSourceSyncRunner Runner;
            private readonly DataSource DS;
            TraffkGlobalDbContext Gdb => Runner.GlobalContext;
            private readonly IDictionary<string, DataSourceFetchItem> FetchItemByEvidence = new Dictionary<string, DataSourceFetchItem>();
            private readonly string BlobRootPath;

            public Fetcher(DataSourceSyncRunner runner, int dataSourceId)
            {
                Runner = runner;
                DS = Gdb.DataSources.Find(dataSourceId);
                PopulateEvidence(dataSourceId);
                BlobRootPath = $"{BlobStorageServices.GetDataSourceFetchItemRoot(null, dataSourceId)}{Runner.StartedAtUtc.ToYYYYMMDD()}/{Runner.StartedAtUtc.ToHHMMSS()}/";
            }

            private ICollection<DataSourceFetchItem> FindEvidenceItems(IEnumerable<string> evidence)
            {
                var matches = new HashSet<DataSourceFetchItem>();
                foreach (var e in evidence)
                {
                    var item = FindEvidenceItem(e);
                    if (item != null)
                    {
                        matches.Add(item);
                    }
                }
                return matches;
            }

            private DataSourceFetchItem FindEvidenceItem(string key)
                => FetchItemByEvidence.FindOrDefault(key);

            private void PopulateEvidence(DataSourceFetchItem item)
            {
                foreach (var e in EvidenceFactory.CreateEvidence(item))
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

            public async Task FetchAsync()
            {
                var fetch = new DataSourceFetche
                {
                    DataSource = DS
                };
                Gdb.DataSourceFetches.Add(fetch);
                if (DS.DataSourceSettings.IsFtp)
                {
                    await ProcessFetchAsync(fetch, DS.DataSourceSettings.FTP);
                }
                else if (DS.DataSourceSettings.IsWeb)
                {
                    await ProcessFetchAsync(fetch, DS.DataSourceSettings.Web);
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized datasource");
                }
                await Gdb.SaveChangesAsync();
            }

            async Task ProcessFetchAsync(DataSourceFetche fetch, DataSourceSettings.WebSettings settings)
            {
                Requires.NonNull(settings, nameof(settings));
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true
                };
                if (settings.LoginPageConfig != null)
                {
                    var cred = await Runner.Vault.GetCredentialsAsync(settings.CredentialsKeyUri);
                    var client = Runner.HttpClientFactory.Create(handler, false);
                    using (var st = await client.GetStreamAsync(settings.LoginPageConfig.LoginPage))
                    {
                        var doc = new H.HtmlDocument();
                        doc.Load(st);
                        foreach (var formNode in doc.DocumentNode.SelectNodesOrEmpty("//form"))
                        {
                            var d = new Dictionary<string, string>();
                            string action = formNode.GetAttributeValue("action", settings.LoginPageConfig.LoginPage.ToString());
                            foreach (var inputNode in formNode.SelectNodesOrEmpty("//input|//textarea|//select"))
                            {
                                string val = null;
                                var fieldName = inputNode.GetAttributeValue("name", null);
                                if (fieldName == settings.LoginPageConfig.PasswordFieldName)
                                {
                                    val = cred.Password;
                                }
                                else if (fieldName == settings.LoginPageConfig.UsernameFieldName)
                                {
                                    val = cred.Username;
                                }
                                else
                                {
                                    switch (inputNode.Name)
                                    {
                                        case "input":
                                            var inputType = inputNode.GetAttributeValue("type", "text");
                                            if (inputType == "submit") continue;
                                            val = inputNode.GetAttributeValue("value", null);
                                            break;
                                        case "textarea":
                                            val = inputNode.InnerText;
                                            break;
                                        case "select":
                                            break;
                                    }
                                }
                                d[fieldName] = val;
                            }
                            if (d.ContainsKey(settings.LoginPageConfig.PasswordFieldName) &&
                                d.ContainsKey(settings.LoginPageConfig.UsernameFieldName))
                            {
                                client = Runner.HttpClientFactory.Create(handler, false);
                                var postAction = new Uri(settings.LoginPageConfig.LoginPage, action);
                                var content = new FormUrlEncodedContent(d);
                                await client.PostAsync(postAction, content);
                                goto AuthenticationDone;
                            }
                        }
                        throw new Exception($"Form was not there or missing fields [{settings.LoginPageConfig.UsernameFieldName}] or [{settings.LoginPageConfig.PasswordFieldName}]");
                    }
                }
                AuthenticationDone:
                Stuff.TaskWaitAllForEach(settings.DownloadUrls, u => FetchTheWebItemAsync(fetch, u, handler));
            }

            private async Task FetchTheWebItemAsync(DataSourceFetche fetch, Uri u, HttpClientHandler handler)
            {
                using (var client = Runner.HttpClientFactory.Create(handler, false))
                {
                    var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, u));
                    var details = new FileDetails(resp);
                    await FetchTheItemAsync(fetch, details, null, DataSourceFetchItem.DataSourceFetchItemTypes.Original, null, async _ => 
                    {
                        var tfn = Stuff.GetTempFileName(".dat", Runner.TempFolderPath);
                        using (var st = await client.GetStreamAsync(u))
                        {
                            using (var dst = File.Create(tfn))
                            {
                                await st.CopyToAsync(dst);
                            }
                        }
                        return tfn;
                    });
                }
            }

            async Task ProcessFetchAsync(DataSourceFetche fetch, DataSourceSettings.FtpSettings settings)
            {
                Requires.NonNull(settings, nameof(settings));
                var cred = await Runner.Vault.GetCredentialsAsync(settings.CredentialsKeyUri);
                var ci = new ConnectionInfo(settings.Hostname, settings.Port, cred.Username, new PasswordAuthenticationMethod(cred.Username, cred.Password));
                using (var client = new SftpClient(ci))
                {
                    client.Connect();
                    Stuff.TaskWaitAllForEach(
                        settings.FolderPaths,
                        fp => FetchFtpFolderFilesAsync(client, fetch, fp)
                        );
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

            private async Task FetchTheItemAsync(DataSourceFetche fetch, FileDetails details, string folder, DataSourceFetchItem.DataSourceFetchItemTypes dataSourceFetchItemType,  DataSourceFetchItem parentFetchItem, Func<FileDetails, Task<string>> fetcher)
            {
                string tfn = null;
                var item = new DataSourceFetchItem
                {
                    DataSourceFetch = fetch,
                    DataSourceFetchItemType = dataSourceFetchItemType,
                    ParentDataSourceFetchItem = parentFetchItem,
                    Size = details.Size,
                    Name = details.Name,
                };
                item.DataSourceFetchItemProperties.LastModifiedAtUtc = details.LastModifiedAtUtc;
                item.DataSourceFetchItemProperties.ContentMD5 = details.ContentMD5;
                item.DataSourceFetchItemProperties.ETag = details.ETag;
                try
                {
                    Trace.WriteLine($"Checking {details.FullName} size={details.Size} LastWriteTimeUtc={details.LastModifiedAtUtc}");
                    var sameDataSourceReplicatedDataSourceFetchItem = FindEvidenceItems(details.CreateEvidence()).FirstOrDefault();
                    if (sameDataSourceReplicatedDataSourceFetchItem!=null)
                    {
                        item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate;
                        item.SameDataSourceReplicatedDataSourceFetchItem = sameDataSourceReplicatedDataSourceFetchItem;
                        return;
                    }
                    //                      Logger.LogInformation("Downloading", file.FullName, file.Length, tfn);
                    try
                    {
                        tfn = await fetcher(details);
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
                                LastModifiedAtUtc = details.LastModifiedAtUtc
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
                                sameDataSourceReplicatedDataSourceFetchItem = FindEvidenceItems(urns).FirstOrDefault();
                                if (sameDataSourceReplicatedDataSourceFetchItem!=null)
                                {
                                    item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate;
                                    item.SameDataSourceReplicatedDataSourceFetchItem = sameDataSourceReplicatedDataSourceFetchItem;
                                    return;
                                }
                            }
                            var res = await BlobStorageServices.StoreStreamAsync(
                                Runner.BlobOptions,
                                BlobStorageServices.ContainerNames.Secure,
                                $"{BlobRootPath}{details.Path.Substring(1)}/{folder}/{details.Name}",
                                muxer.OpenRead(),
                                p,
                                amt => Trace.WriteLine($"Uploading {amt}/{details.Size}")
                                );
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
                            await FetchTheItemAsync(
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
                                    return Task.FromResult(utfp);
                                }
                                );
                        }
                    }
                    Stuff.FileTryDelete(tfn);
                }
            }

            private Task FetchFtpFolderFilesAsync(SftpClient client, DataSourceFetche fetch, string path)
            {
                if (IsAlreadyVisited(path)) return Task.CompletedTask;
                client.ChangeDirectory(path);
                var entries = client.ListDirectory(path);
                Stuff.TaskWaitAllForEach(
                    entries, 
                    async delegate (SftpFile file)
                {
                    if (IsAlreadyVisited(file.FullName)) return;
                    if (file.IsDirectory)
                    {
                        await FetchFtpFolderFilesAsync(client, fetch, file.FullName);
                    }
                    else if (file.IsRegularFile)
                    {
                        await FetchTheItemAsync(
                            fetch, 
                            new FileDetails(file),
                            "raw",
                            DataSourceFetchItem.DataSourceFetchItemTypes.Original,
                            null,
                            async fd => 
                            {
                                var fn = Path.GetTempFileName();
                                using (var st = File.Create(fn))
                                {
                                    Trace.WriteLine($"Starting {fd.FullName} to [{fn}]");
                                    await Task.Factory.FromAsync(
                                        client.BeginDownloadFile(fd.FullName, st, null, null, amt => Trace.WriteLine($"Downloading {fd.FullName} to [{fn}] => {amt}/{fd.Size}")),
                                        client.EndDownloadFile);
                                    Trace.WriteLine($"Finishing {fd.FullName} to [{fn}]");
                                }
                                return fn;
                            });
                    }
                });
                return Task.CompletedTask;
            }
        }
    }
}
