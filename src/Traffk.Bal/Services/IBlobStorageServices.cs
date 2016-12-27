using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Traffk.Bal.Services
{
    public interface IBlobStorageServices
    {
        Task<ICollection<CloudBlob>> GetFileInfosAsync(bool secure, BlobStorageServices.Roots root, string prefix);
        Task<Uri> StoreFileAsync(bool secure, BlobStorageServices.Roots root, IFormFile file, string name=null, bool addUniqueRef=false);
    }
}