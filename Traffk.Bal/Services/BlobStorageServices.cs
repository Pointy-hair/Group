using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Services
{
    public class BlobStorageServices
    {
        private const string PortalRootContainerName = "portal";
        private const string SecureRootContainerName = "secure";

        private readonly CloudStorageAccount StorageAccount;
        private readonly CloudBlobClient BlobClient;
        private readonly CurrentTenantServices CurrentTenant;
        private readonly ICurrentUser CurrentUser;

        public enum Roots
        {
            Portal,
            User,
        }

        private CloudBlobContainer GetContainer(bool secure) => BlobClient.GetContainerReference(secure ? SecureRootContainerName : PortalRootContainerName);

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

        public async Task<Uri> StoreFileAsync(bool secure, Roots root, IFormFile file, string name=null, bool addUniqueRef=false)
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
                await block.UploadFromStreamAsync(st);
            }
            var blob = container.GetBlobReference(name ?? file.Name);
            blob.Properties.ContentType = file.ContentType;
            blob.Metadata["TenantId"] = tenant.TenantId.ToString();
            blob.Metadata["TenantName"] = tenant.TenantName;
            blob.Metadata["UploadedContentDisposition"] = file.ContentDisposition;
            blob.Metadata["UploadedFileName"] = file.FileName;
            blob.Metadata["UploadedByUserName"] = CurrentUser.User.UserName;
            blob.Metadata["UploadedByUserId"] = CurrentUser.User.Id;
            await blob.SetPropertiesAsync();
            await blob.SetMetadataAsync();
            return block.Uri;
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
    }
}
