using Newtonsoft.Json;
using Traffk.Bal.Data;

namespace Traffk.Bal.Settings
{
    public class CommunicationSettings
    {
        [JsonProperty("recurrence")]
        public RecurrenceSettings Recurrence { get; set; }
        [JsonProperty("reportName")]
        public string ReportName { get; set; }
        [JsonProperty("reportDescription")]
        public string ReportDescription { get; set; }
        [JsonProperty("reportId")]
        public string ReportId { get; set; }
        [JsonProperty("recurringJobId")]
        public string RecurringJobId { get; set; }

        public static CommunicationSettings CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CommunicationSettings>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
