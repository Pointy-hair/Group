using Newtonsoft.Json;

namespace Traffk.Bal.Settings
{
    public class HostSettings
    {
        public class HostInfo
        {
            [JsonProperty("Hostname")]
            public string Hostname { get; set; }
        }

        [JsonProperty("HostInfos")]
        public HostInfo[] HostInfos { get; set; }
    }
}
