using Newtonsoft.Json;

namespace Traffk.Bal.Permissions
{
    public class PermissionClaimValue
    {
        [JsonProperty("Granted")]
        public bool Granted { get; set; }

        [JsonProperty("Version")]
        public int Version { get; set; } = 1;

        public PermissionClaimValue(bool granted = false)
        {
            Granted = true;
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this);

        public static PermissionClaimValue CreateFromJson(string json)
            => JsonConvert.DeserializeObject<PermissionClaimValue>(json);
    }
}
