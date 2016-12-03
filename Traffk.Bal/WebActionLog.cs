using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal
{
    public class WebActionLog
    {
        [JsonProperty("createdAtUtc")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAtUtc { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("userAgentString")]
        public string UserAgentString { get; set; }

        [JsonProperty("traceIdentifier")]
        public string TraceIdentifier { get; set; }
    }
}
