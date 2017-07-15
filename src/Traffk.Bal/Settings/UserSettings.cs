using Newtonsoft.Json;
using System.ComponentModel;

namespace Traffk.Bal.Settings
{
    public class UserSettings
    {
        [JsonProperty("PortalOptions")]
        public PortalConfig PortalOptions { get; set; }

        [JsonProperty("TableauUserName")]
        public string TableauUserName { get; set; }

        [JsonProperty("TableauPassword")]
        public string TableauPassword { get; set; }

        [DisplayName("API Key")]
        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }

        public UserSettings()
        { }

        public static UserSettings CreateFromJson(string json)
            => JsonConvert.DeserializeObject<UserSettings>(json);

        public string ToJson()
            => JsonConvert.SerializeObject(this);
    }
}
