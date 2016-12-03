using Newtonsoft.Json;
using Traffk.Bal.Data.Ddb;

namespace Traffk.Bal
{
    public class JobResult
    {
        public static JobResult CreateFromJson(string json) => string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<JobResult>(json);

        public string ToJson() => JsonConvert.SerializeObject(this);

        [JsonProperty("previousResult")]
        public JobResult PreviousResult { get; set; }

        [JsonProperty("error")]
        public ExceptionError Error { get; set; }
    }
}
