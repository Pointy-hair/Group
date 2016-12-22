using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Traffk.Bal.Settings
{
    [DataContract]
    public class ApplicationSettings
    {
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(ApplicationSettings));

        [DataMember(Name = "EmailSenderName")]
        public string EmailSenderName { get; set; }

        [DataMember(Name = "EmailSenderAddress")]
        public string EmailSenderAddress { get; set; }

        [DataMember(Name = "Hosts")]
        public HostSettings Hosts { get; set; }

        [DataMember(Name = "PortalOptions")]
        public PortalOptions PortalOptions { get; set; }

        [DataMember(Name = "Registration")]
        public RegistrationSettings Registration { get; set; }

        [DataMember(Name = "ReusableValues")]
        public IList<ReusableValue> ResourceValues { get; set; }

        public static ApplicationSettings CreateFromJson(string json)
        {
            return JsonSerializer.ReadObjectFromString<ApplicationSettings>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.WriteObjectToString(this);
        }

        internal void EnsureMemberClasses()
        {
            Hosts = Hosts ?? new HostSettings();
            PortalOptions = PortalOptions ?? new PortalOptions();
            Registration = Registration ?? new RegistrationSettings();
        }
    }
}
