using Microsoft.Extensions.Options;
using Renci.SshNet;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.Core.Crypto;
using RevolutionaryStuff.Core.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Traffk.Bal;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Utility;
using H = HtmlAgilityPack;

namespace Traffk.BackgroundJobServer
{
    public partial class DataSourceSyncRunner : BaseJobRunner, IDataSourceSyncJobs
    {
        private readonly IOptions<BlobStorageServices.Config> BlobConfig;
        private readonly IVault Vault;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly string TenantName;

        public DataSourceSyncRunner(
            IJobInfoFinder jobInfoFinder,
            IHttpClientFactory httpClientFactory,
            JobRunnerProgram jobRunnerProgram,
            TraffkGlobalDbContext gdb,
            Bal.Data.Rdb.TraffkTenantShards.TraffkTenantShardsDbContext tdb,
            IVault vault,
            IOptions<BlobStorageServices.Config> blobConfig,
            Serilog.ILogger logger)
            : base(gdb, jobInfoFinder, logger)
        {
            HttpClientFactory = httpClientFactory;
            BlobConfig = blobConfig;
            Vault = vault;
            TenantName = tdb.TenantFindByTenantId(jobInfoFinder.JobInfo.TenantId.Value).Result.First().TenantName;
        }

        private static readonly object DecryptAsyncLocker = new object();

        private async Task DecryptAsync(Stream st, string destinationFile)
        {
            var pw = await Vault.GetSecretAsync(Bal.Services.Vault.CommonSecretUris.TraffkPgpPrivateKeyPasswordUri);
            var sk = await Vault.GetSecretAsync(Bal.Services.Vault.CommonSecretUris.TraffkPgpPrivateKeyUri);

            lock (DecryptAsyncLocker)
            {
                var ring = Cache.DataCacher.FindOrCreateValWithSimpleKey(sk, () =>
                {
                    using (var keyStream = StreamHelpers.Create(sk, System.Text.ASCIIEncoding.ASCII))
                    {
                        return EasyNetPGP.PgpEncryptorDecryptor.CreatePgpSecretKeyRingBundle(keyStream);
                    }
                });
                EasyNetPGP.PgpEncryptorDecryptor.DecryptFile(
                    st,
                    null,
                    pw,
                    destinationFile,
                    ring);
            }
        }

        Task IDataSourceSyncJobs.DataSourceFetchAsync(int dataSourceId)
            => new Fetcher(this, dataSourceId).FetchAsync();

        /// <remarks>https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/</remarks>
        public sealed class AsyncLocker : BaseDisposable
        {
            private readonly SemaphoreSlim GdbSemaphore = new SemaphoreSlim(1, 1);
            public AsyncLocker()
            {

            }

            public async Task GoAsync(Func<Task> a)
            {
                await GdbSemaphore.WaitAsync();
                try
                {
                    await a();
                }
                finally
                {
                    GdbSemaphore.Release();
                }
            }
        }


        public partial class Fetcher
        {
            private readonly AsyncLocker GdbLocker = new AsyncLocker();
            private readonly DataSourceSyncRunner Runner;
            private readonly DataSource DS;
            TraffkGlobalDbContext Gdb => Runner.GlobalContext;
            private readonly IDictionary<string, DataSourceFetchItem> FetchItemByEvidence = new Dictionary<string, DataSourceFetchItem>();
            private readonly string BlobRootPath;

            public Fetcher(DataSourceSyncRunner runner, int dataSourceId)
            {
                Requires.NonNull(runner, nameof(runner));
                Runner = runner;
                DS = Gdb.DataSources.Find(dataSourceId);
                Requires.NonNull(DS, nameof(DS));
                PopulateEvidence(dataSourceId);
                BlobRootPath = $"{BlobStorageServices.GetDataSourceFetchItemRoot(runner.TenantName, dataSourceId)}{Runner.StartedAtUtc.ToYYYYMMDD()}/{Runner.StartedAtUtc.ToHHMMSS()}/";
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
                => key==null?null:FetchItemByEvidence.FindOrDefault(key);

            private void PopulateEvidence(DataSourceFetchItem item)
            {
                lock (FetchItemByEvidence)
                {
                    foreach (var e in EvidenceFactory.CreateEvidence(item))
                    {
                        FetchItemByEvidence[e] = item;
                    }
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
                await Task.WhenAll(settings.DownloadUrls.ConvertAll(u => FetchTheWebItemAsync(fetch, u, handler)));
            }

            private async Task FetchTheWebItemAsync(DataSourceFetche fetch, Uri u, HttpClientHandler handler)
            {
                FileDetails details;
                using (var client = Runner.HttpClientFactory.Create(handler, false))
                {
                    var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, u));
                    details = new FileDetails(resp);
                }
                await FetchTheItemAsync(fetch, details, DataSourceFetchItem.DataSourceFetchItemTypes.Original, null, async _ => 
                {
                    var tfn = Stuff.FindOrigFileName(Path.Combine(Runner.TempFolderPath, details.Name));
                    using (var client = Runner.HttpClientFactory.Create(handler, false))
                    {
                        using (var st = await client.GetStreamAsync(u))
                        {
                            using (var dst = File.Create(tfn))
                            {
                                await st.CopyToAsync(dst);
                            }
                        }
                    }
                    return tfn;
                });
            }

            async Task ProcessFetchAsync(DataSourceFetche fetch, DataSourceSettings.FtpSettings settings)
            {
                Requires.NonNull(settings, nameof(settings));
                var cred = await Runner.Vault.GetCredentialsAsync(settings.CredentialsKeyUri);
                var ci = new ConnectionInfo(settings.Hostname, settings.Port, cred.Username, new PasswordAuthenticationMethod(cred.Username, cred.Password));
                using (var client = new SftpClient(ci))
                {
                    client.Connect();
                    await Task.WhenAll(settings.FolderPaths.ConvertAll(fp => FetchFtpFolderFilesAsync(client, fetch, fp)));
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

            private async Task FetchTheItemAsync(DataSourceFetche fetch, FileDetails details, DataSourceFetchItem.DataSourceFetchItemTypes dataSourceFetchItemType,  DataSourceFetchItem parentFetchItem, Func<FileDetails, Task<string>> fetchAsync)
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
                    if (sameDataSourceReplicatedDataSourceFetchItem != null)
                    {
                        item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate;
                        item.SameDataSourceReplicatedDataSourceFetchItem = sameDataSourceReplicatedDataSourceFetchItem;
                        return;
                    }
                    //                      Logger.LogInformation("Downloading", file.FullName, file.Length, tfn);
                    tfn = await fetchAsync(details);
                    using (var st = File.OpenRead(tfn))
                    {
                        item.Size = st.Length;
                        using (var muxer = new StreamMuxer(st, true))
                        {
                            var p = new BlobStorageServices.FileProperties
                            {
                                LastModifiedAtUtc = details.LastModifiedAtUtc
                            };
                            p.Metadata[BlobStorageServices.MetaKeyNames.SourcePath] = details.Folder;
                            p.Metadata[BlobStorageServices.MetaKeyNames.SourceFullName] = details.FullName;
                            var urns = new List<string>();
                            Parallel.ForEach(
                                new[]
                                {
                                    Hash.CommonHashAlgorithmNames.Md5,
                                    Hash.CommonHashAlgorithmNames.Sha1,
                                    Hash.CommonHashAlgorithmNames.Sha512,
                                },
                                hashAlgName =>
                                {
                                    var urn = Hash.Compute(muxer.OpenRead(), hashAlgName).Urn;
                                    if (urn == null) return; //yes... in some cases this somehow happens...
                                    urns.Add(urn);
                                });
                            if (urns.Count > 0)
                            {
                                p.Metadata[BlobStorageServices.MetaKeyNames.Urns] = CSV.FormatLine(urns, false);
                                sameDataSourceReplicatedDataSourceFetchItem = FindEvidenceItems(urns).FirstOrDefault();
                                if (sameDataSourceReplicatedDataSourceFetchItem != null)
                                {
                                    item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Duplicate;
                                    item.SameDataSourceReplicatedDataSourceFetchItem = sameDataSourceReplicatedDataSourceFetchItem;
                                    return;
                                }
                            }
                            var res = await BlobStorageServices.StoreStreamAsync(
                                Runner.BlobConfig,
                                BlobStorageServices.ContainerNames.Secure,
                                $"{BlobRootPath}{details.Folder.Substring(1)}{details.Name}",
                                muxer.OpenRead(),
                                p,
                                amt => Trace.WriteLine($"Uploading {amt}/{muxer.Length}")
                                );
                            item.DataSourceFetchItemProperties = new DataSourceFetchItemProperties();
                            item.DataSourceFetchItemProperties.Set(p);
                            item.Url = res.Uri.ToString();
                            PopulateEvidence(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    item.DataSourceFetchItemType = DataSourceFetchItem.DataSourceFetchItemTypes.Errored;
                    item.DataSourceFetchItemProperties.Error = new ExceptionError(ex);
                    Trace.WriteLine(ex);
                }
                finally
                {
                    if (item != null)
                    {
                        await GdbLocker.GoAsync(async () => {
                            Gdb.DataSourceFetchItems.Add(item);
                            await Gdb.SaveChangesAsync();
                        });
                    }
                }
                var ext = Path.GetExtension(details.Name).ToLower();
                if (ext == ".pgp" || details.Name.ToLower().Contains(".pgp."))
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
                        DataSourceFetchItem.DataSourceFetchItemTypes.Decrypted,
                        item,
                        async _ =>
                        {
                            var utfp = Path.GetTempFileName();
                            using (var st = File.OpenRead(tfn))
                            {
                                await Runner.DecryptAsync(st, utfp);
                            }
                            return utfp;
                        }
                        );
                }
                else if (
                    MimeType.Application.Zip.DoesExtensionMatch(details.Name) && 
                    DS.DataSourceSettings.DecompressItems && 
                    dataSourceFetchItemType!= DataSourceFetchItem.DataSourceFetchItemTypes.UnpackedRecompressedSingleton)
                {
                    var relUnzipFolder = Path.GetFileNameWithoutExtension(details.Name);
                    var unzipFolder = Path.Combine(Path.GetDirectoryName(tfn), relUnzipFolder);
                    using (var st = File.OpenRead(tfn))
                    {
                        using (var za = new ZipArchive(st, ZipArchiveMode.Read))
                        {
                            if (za.Entries.Count < 2) return;
                        }
                    }                        
                    ZipFile.ExtractToDirectory(tfn, unzipFolder);
                    await TaskWhenAllOneAtATime(
                        Directory.GetFiles(unzipFolder, "*.*", SearchOption.AllDirectories).ConvertAll(
                        unzipped =>
                        {
                            string rezipped = unzipped;
                            bool isRezipped = false;
                            if (!MimeType.Application.Zip.DoesExtensionMatch(unzipped))
                            {
                                rezipped = unzipped + MimeType.Application.Zip.PrimaryFileExtension;
                                using (var st = File.Create(rezipped))
                                {
                                    using (var za = new ZipArchive(st, ZipArchiveMode.Create))
                                    {
                                        za.CreateEntryFromFile(unzipped, Path.GetFileName(unzipped));
                                    }
                                    isRezipped = true;
                                }
                            }
                            return FetchTheItemAsync(
                                fetch,
                                new FileDetails(new FileInfo(rezipped), Path.Combine(details.Folder, relUnzipFolder)),
                                 isRezipped ? DataSourceFetchItem.DataSourceFetchItemTypes.UnpackedRecompressedSingleton : DataSourceFetchItem.DataSourceFetchItemTypes.Unpacked,
                                 item,
                                 _ => Task.FromResult(rezipped)
                                );
                        }));
                    Stuff.Noop();
                }
            }

            private async static Task TaskWhenAllOneAtATime(IEnumerable<Task> tasks)
            {
                var exceptions = new List<Exception>();
                foreach (var task in tasks)
                {
                    try
                    {
                        await Task.Run(()=>task);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }
            }

            private async Task FetchFtpFolderFilesAsync(SftpClient client, DataSourceFetche fetch, string path)
            {
                if (IsAlreadyVisited(path)) return;
                client.ChangeDirectory(path);
                var entries = client.ListDirectory(path);
                await TaskWhenAllOneAtATime(
                    entries.ConvertAll(async file => 
                {
                    if (file.IsDirectory)
                    {
                        await FetchFtpFolderFilesAsync(client, fetch, file.FullName);
                    }
                    else if (file.IsRegularFile)
                    {
                        if (IsAlreadyVisited(file.FullName)) return;
                        await FetchTheItemAsync(
                            fetch, 
                            new FileDetails(file),
                            DataSourceFetchItem.DataSourceFetchItemTypes.Original,
                            null,
                            async fd => 
                            {
                                var fn = Stuff.GetTempFileName(Path.GetExtension(fd.FullName), Runner.TempFolderPath);
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
                }));
            }
        }
    }
}
