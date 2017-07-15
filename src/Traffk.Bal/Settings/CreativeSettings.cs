using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Services;

namespace Traffk.Bal.Settings
{
    public class CreativeSettings
    {
        public static CreativeSettings CreateFromJson(string json)
            => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CreativeSettings>(json);

        public string ToJson() 
            => JsonConvert.SerializeObject(this);

        [JsonProperty("attachments")]
        public List<CloudFilePointer> Attachments { get; set; } = new List<CloudFilePointer>();

        [JsonProperty("emailSubject")]
        [DisplayName("Email Subject")]
        public string EmailSubject { get; set; }

        [DataType(DataType.Html)]
        [JsonProperty("emailHtmlBody")]
        [DisplayName("Email Body HTML")]
        public string EmailHtmlBody { get; set; }

        [JsonProperty("emailTextBody")]
        [DisplayName("Email Body Text")]
        public string EmailTextBody { get; set; }

        [JsonProperty("textMessageBody")]
        [DisplayName("Text Message Body")]
        public string TextMessageBody { get; set; }

        [JsonIgnore]
        public bool SupportsEmailHtml => !string.IsNullOrWhiteSpace(EmailHtmlBody);

        [JsonIgnore]
        public bool SupportsEmailText => !string.IsNullOrWhiteSpace(EmailTextBody);

        [JsonIgnore]
        public bool SupportsTextMessage => !string.IsNullOrWhiteSpace(TextMessageBody);
    }
}
