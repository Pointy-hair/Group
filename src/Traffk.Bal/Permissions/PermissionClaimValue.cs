using RevolutionaryStuff.Core;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Traffk.Bal.Permissions
{
    [DataContract]
    public class PermissionClaimValue
    {
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(PermissionClaimValue));

        [DataMember]
        public bool Granted { get; set; }

        [DataMember]
        public int Version { get; set; } = 1;

        public PermissionClaimValue(bool granted = false)
        {
            Granted = true;
        }

        public static PermissionClaimValue CreateFromJson(string json)
        {
            return JsonSerializer.ReadObjectFromString<PermissionClaimValue>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.WriteObjectToString(this);
        }
    }
}
