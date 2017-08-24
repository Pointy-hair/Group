using Newtonsoft.Json;
using RevolutionaryStuff.Core;

namespace Traffk.Bal
{
    public class JobResult
    {
        public static JobResult CreateFromJson(string json) =>
            TraffkHelpers.JsonConvertDeserializeObjectOrFallback<JobResult>(json);

        public string ToJson() 
            => JsonConvert.SerializeObject(this);

        [JsonProperty("previousResult")]
        public JobResult PreviousResult { get; set; }

        [JsonProperty("error")]
        public ExceptionError Error { get; set; }
    }
}
