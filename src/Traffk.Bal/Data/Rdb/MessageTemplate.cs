using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Traffk.Bal.Services;

namespace Traffk.Bal.Data.Rdb
{
    public partial class MessageTemplate
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public string AttachmentPrefix => $"messageTemplate/{this.MessageTemplateId}/attachments/";

        public Task<ICollection<CloudBlob>> GetFileAttachmentInfosAsync(BlobStorageServices blobs)
            => blobs.GetFileInfosAsync(false, BlobStorageServices.Roots.Portal, AttachmentPrefix);
    }
}
