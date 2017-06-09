using System.Runtime.Serialization;

namespace Traffk.OrchestraRx.Models
{
    public class OrchestraRxTokenResponse
    {
        [DataMember(Name = "access_token")]
        public string access_token { get; set; }

        [DataMember(Name = "expires_in")]
        public int expires_in { get; set; }

        [DataMember(Name = "token_type")]
        public string token_type { get; set; }
    }
}
