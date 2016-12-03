using RevolutionaryStuff.Core.ApplicationParts;
using System.Runtime.Serialization;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    [DataContract]
    public class PowerBiDashboard : PowerBiResource, IName
    {
        [DataMember(Name = "displayName")]
        public string Name { get; set; }

        [DataMember(Name = "isReadOnly")]
        public bool IsReadOnly { get; set; }

        public override string ToString() => $"{base.ToString()} isreadonly={IsReadOnly}";
    }
}
