using RevolutionaryStuff.Core.ApplicationParts;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Traffk.Bal.Settings
{
    public class ReusableValue
    {
        public static readonly ReusableValue[] None = new ReusableValue[0];

        [DataMember(Name = "ResourceType")]
        public ReusableValueTypes ResourceType { get; set; }

        [Key]
        [MinLength(1)]
        [NotNull]
        [DataMember(Name = "Key")]
        public string Key { get; set; }

        [DataMember(Name = "Value")]
        public string Value { get; set; }
    }
}
