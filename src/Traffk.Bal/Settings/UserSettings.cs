using System;
using System.ComponentModel;
using RevolutionaryStuff.Core;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Traffk.Bal.Settings
{
    [DataContract]
    public class UserSettings
    {
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(UserSettings));

        [DataMember(Name = "PortalOptions")]
        public PortalOptions PortalOptions { get; set; }

        [DataMember(Name = "TableauUserName")]
        public string TableauUserName { get; set; }

        [DataMember(Name = "TableauPassword")]
        public string TableauPassword { get; set; }

        [DisplayName("API Key")]
        [DataMember(Name = "ApiKey")]
        public Guid ApiKey { get; set; }

        public UserSettings()
        { }

        public static UserSettings CreateFromJson(string json)
        {
            return JsonSerializer.ReadObjectFromString<UserSettings>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.WriteObjectToString(this);
        }
    }
}
