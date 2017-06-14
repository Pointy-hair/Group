using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.Crypto;
using RevolutionaryStuff.Core.EncoderDecoders;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Services
{
    public enum CloudFilePointerTypes
    {
        AzureBlob = 1,
    }

    public class CloudFilePointer
    {
        [JsonProperty("cloudFilePointerType")]
        public CloudFilePointerTypes CloudFilePointerType { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("containerName")]
        public string ContainerName { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }
    }

    public class BlobStorageServices
    {
        private const string GlobalsLoginDomain = "_GLOBALS";

        private readonly CloudStorageAccount StorageAccount;
        private readonly CloudBlobClient BlobClient;
        private readonly CurrentTenantServices CurrentTenant;
        private readonly ICurrentUser CurrentUser;

        public static class MetaKeyNames
        {
            public const string Urns = "Urns";
            public const string SourcePath = "SourcePath";
            public const string SourceFullName = "SourceFullName";
            public const string IsPgpEncrypted = "IsPgpEncrypted";
        }

        public static class ContainerNames
        {
            public const string Secure = "secure";
            public const string Portal = "portal";
        }

        public enum Roots
        {
            Portal,
            User,
        }

        public static string GetDataSourceFetchItemRoot(string tenantLoginDomain, int dataSourceId)
        {
            return $"{tenantLoginDomain ?? GlobalsLoginDomain}/{dataSourceId}/";
        }

        public static CloudBlobContainer GetContainer(CloudBlobClient client, string containerName)
            => client.GetContainerReference(containerName);

        private CloudBlobContainer GetContainer(bool secure)
            => GetContainer(BlobClient, secure ? ContainerNames.Secure : ContainerNames.Portal);

        private string ConstructRealName(Tenant tenant, Roots root, string name)
        {
            switch (root)
            {
                case Roots.Portal:
                    return $"{tenant.LoginDomain}/{name}";
                case Roots.User:
                    return $"{tenant.LoginDomain}/user/{CurrentUser.User.NormalizedUserName}/{name}";
                default:
                    throw new UnexpectedSwitchValueException(root);
            }
        }

        public async Task<ICollection<CloudBlob>> GetFileInfosAsync(bool secure, Roots root, string prefix)
        {
            var container = GetContainer(secure);
            var tenant = await CurrentTenant.GetTenantAsync();
            var name = ConstructRealName(tenant, root, prefix);
            var dir = container.GetDirectoryReference(name);


            var results = new List<CloudBlob>();
            BlobContinuationToken continuationToken = null;

            do
            {
                var segment = await dir.ListBlobsSegmentedAsync(false, BlobListingDetails.Metadata, null, continuationToken, null, null);
                foreach (var item in segment.Results)
                {
                    if (item is CloudBlob)
                    {
                        results.Add((CloudBlob)item);
                    }
                }
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);
            return results;
        }

        private const int UploadChunkSize = 1024 * 1024 * 64;
        /// <remarks>
        /// http://blog.geuer-pollmann.de/blog/2014/07/21/uploading-blobs-to-azure-the-robust-way/
        /// http://wely-lau.net/2012/02/26/uploading-big-files-in-windows-azure-blob-storage-with-putlistblock/
        /// </remarks>
        private static async Task UploadStreamAsync(CloudBlockBlob blob, Stream st, Action<long> uploadProgress = null)
        {
            bool big = false;
            long uploaded = 0;
            try
            {
                big = st.Length <= UploadChunkSize;
            }
            catch (Exception) { }
            if (big)
            {
                uploaded = st.Length;
                await blob.UploadFromStreamAsync(st);
            }
            else
            {
                var blocklist = new List<string>();
                var buf = new byte[UploadChunkSize];
                for (;;)
                {
                    var read = await st.ReadAsync(buf, 0, buf.Length);
                    if (read <= 0) break;
                    var blockId = Base64.ToBase64String(Salt.CreateRandomBuf(32));
                    using (var mst = new MemoryStream(buf, 0, read, false))
                    {
                        await blob.PutBlockAsync(blockId, mst, null);
                    }
                    uploaded += read;
                    uploadProgress?.Invoke(uploaded);
                    blocklist.Add(blockId);
                }
                await blob.PutBlockListAsync(blocklist);
            }
            uploadProgress?.Invoke(uploaded);
        }

        public async Task<CloudFilePointer> StoreFileAsync(bool secure, Roots root, IFormFile file, string name=null, bool addUniqueRef=false)
        {
            var container = GetContainer(secure);

            name = name ?? Path.GetFileName(file.FileName);
            if (addUniqueRef)
            {
                name = $"{Path.GetFileNameWithoutExtension(name)}.{Stuff.Random.Next()}{Path.GetExtension(name)}";
            }
            var tenant = await CurrentTenant.GetTenantAsync();
            name = ConstructRealName(tenant, root, name);
            var block = container.GetBlockBlobReference(name ?? file.Name);
            using (var st = file.OpenReadStream())
            {
                await UploadStreamAsync(block, st);
            }
            var blob = container.GetBlobReference(name ?? file.Name);
            blob.Properties.ContentType = file.ContentType;
            blob.Metadata["TenantId"] = tenant.TenantId.ToString();
            blob.Metadata["TenantName"] = tenant.TenantName;
            blob.Metadata["UploadedContentDisposition"] = file.ContentDisposition;
            blob.Metadata["UploadedFileName"] = file.FileName;
            blob.Metadata["UploadedByUserName"] = CurrentUser.User.UserName;
            blob.Metadata["UploadedByUserId"] = CurrentUser.User.Id;
            //Metadata fields cannot be null
            await blob.SetPropertiesAsync();
            await blob.SetMetadataAsync();
            return new CloudFilePointer
            {
                CloudFilePointerType = CloudFilePointerTypes.AzureBlob,
                Uri = block.Uri,
                Path = name,
                ContainerName = container.Name,
                ContentType = file.ContentType
            };
        }

        public BlobStorageServices(CurrentTenantServices currentTenant, ICurrentUser currentUser, IOptions<BlobStorageServicesOptions> options)
        {
            CurrentTenant = currentTenant;
            CurrentUser = currentUser;
            Options = options;
            StorageAccount = CloudStorageAccount.Parse(Options.Value.ConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        private readonly IOptions<BlobStorageServicesOptions> Options;

        public class BlobStorageServicesOptions
        {
            public string ConnectionString { get; set; }
        }

        public class FileProperties
        {
            public DateTime? LastModifiedAtUtc { get; set; }
            public DateTime? CreatedAtUtc { get; set; }
            public string ContentType { get; set; }
            public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        }

        public static async Task<CloudFilePointer> StoreStreamAsync(IOptions<BlobStorageServicesOptions> blobOptions, string containerName, string path, Stream st, FileProperties p, Action<long> uploadProgress=null)
        {
            p = p ?? new FileProperties();
            Requires.ReadableStreamArg(st, nameof(st));

            var sa = CloudStorageAccount.Parse(blobOptions.Value.ConnectionString);
            var client = sa.CreateCloudBlobClient();
            var container = GetContainer(client, containerName);

            var block = container.GetBlockBlobReference(path);
            await UploadStreamAsync(block, st, uploadProgress);

            var blob = container.GetBlobReference(path);
            if (p.ContentType != null)
            {
                blob.Properties.ContentType = p.ContentType;
            }
            if (p.Metadata != null)
            {
                foreach (var kvp in p.Metadata)
                {
                    blob.Metadata[kvp.Key] = kvp.Value;
                }
            }
            await blob.SetPropertiesAsync();
            await blob.SetMetadataAsync();
            return new CloudFilePointer
            {
                CloudFilePointerType = CloudFilePointerTypes.AzureBlob,
                Uri = block.Uri,
                Path = path,
                ContainerName = container.Name,
                ContentType = p.ContentType
            };

        }
    }
}
