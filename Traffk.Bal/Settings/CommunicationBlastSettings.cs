using Newtonsoft.Json;
using Traffk.Bal.Data;

namespace Traffk.Bal.Settings
{
    public class CommunicationBlastSettings
    {
        [JsonProperty("recurrence")]
        public RecurrenceSettings Recurrence { get; set; }

        public static CommunicationBlastSettings CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CommunicationBlastSettings>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
