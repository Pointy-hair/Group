using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Traffk.Bal.Data.Rdb
{
    public partial class Creative
    {
        [JsonIgnore]
        [IgnoreDataMember]
        [NotMapped]
        public string AttachmentPrefix => $"creatives/{CreativeId}/attachments/";
    }
}
