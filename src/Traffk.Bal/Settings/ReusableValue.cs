using Newtonsoft.Json;
using RevolutionaryStuff.Core.ApplicationParts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class ReusableValue
    {
        public static readonly ReusableValue[] None = new ReusableValue[0];

        [DisplayName("Resource Type")]
        [JsonProperty("ResourceType")]
        public ReusableValueTypes ResourceType { get; set; }

        [Key]
        [MinLength(1)]
        [NotNull]
        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}
