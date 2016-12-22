using Newtonsoft.Json;
using Traffk.Bal.Data;

namespace Traffk.Bal.Settings
{
    public class CommunicationSettings
    {
        [JsonProperty("recurrence")]
        public RecurrenceSettings Recurrence { get; set; }

        public static CommunicationSettings CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CommunicationSettings>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
