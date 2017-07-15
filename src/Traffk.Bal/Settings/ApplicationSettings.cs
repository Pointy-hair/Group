using Newtonsoft.Json;
using System.Collections.Generic;
using Traffk.Bal.Communications;

namespace Traffk.Bal.Settings
{
    public class ApplicationSettings
    {
        [JsonProperty("EmailSenderName")]
        public string EmailSenderName { get; set; }

        [JsonProperty("EmailSenderAddress")]
        public string EmailSenderAddress { get; set; }

        [JsonProperty("Hosts")]
        public HostSettings Hosts { get; set; }

        [JsonProperty("PortalOptions")]
        public PortalConfig PortalOptions { get; set; }

        [JsonProperty("Registration")]
        public RegistrationSettings Registration { get; set; }

        [JsonProperty("ReusableValues")]
        public IList<ReusableValue> ResourceValues { get; set; }

        [JsonProperty("CreativeIdBySystemCommunicationPurpose")]
        public IDictionary<SystemCommunicationPurposes, int> CreativeIdBySystemCommunicationPurpose { get; set; } = new Dictionary<SystemCommunicationPurposes, int>();

        public static ApplicationSettings CreateFromJson(string json)
            => JsonConvert.DeserializeObject<ApplicationSettings>(json);

        public string ToJson()
            => JsonConvert.SerializeObject(this);

        internal void EnsureMemberClasses()
        {
            Hosts = Hosts ?? new HostSettings();
            PortalOptions = PortalOptions ?? new PortalConfig();
            Registration = Registration ?? new RegistrationSettings();
        }
    }
}
