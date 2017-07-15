using Newtonsoft.Json;

namespace Traffk.Orchestra.Models
{
    public class OrchestraRxTokenResponse
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }

        [JsonProperty("expires_in")]
        public int expires_in { get; set; }

        [JsonProperty("token_type")]
        public string token_type { get; set; }
    }
}
