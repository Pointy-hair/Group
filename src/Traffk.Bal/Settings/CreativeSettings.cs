using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class CreativeSettings
    {
        public static CreativeSettings CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CreativeSettings>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        [JsonProperty("emailSubject")]
        public string EmailSubject { get; set; }

        [DataType(DataType.Html)]
        [JsonProperty("emailHtmlBody")]
        public string EmailHtmlBody { get; set; }

        [JsonProperty("emailTextBody")]
        public string EmailTextBody { get; set; }

        [JsonProperty("textMessageBody")]
        public string TextMessageBody { get; set; }

        [JsonIgnore]
        public bool SupportsEmailHtml => !string.IsNullOrWhiteSpace(EmailHtmlBody);

        [JsonIgnore]
        public bool SupportsEmailText => !string.IsNullOrWhiteSpace(EmailTextBody);

        [JsonIgnore]
        public bool SupportsTextMessage => !string.IsNullOrWhiteSpace(TextMessageBody);
    }
}
