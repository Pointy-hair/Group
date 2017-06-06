using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Traffk.Bal.BackgroundJobs
{
    public class JobStateDetails
    {
        [NotMapped]
        [DisplayName("Parent Job ID")]
        public int ParentId { get; set; }

        public static JobStateDetails CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<JobStateDetails>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
