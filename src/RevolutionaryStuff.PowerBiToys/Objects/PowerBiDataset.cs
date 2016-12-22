using RevolutionaryStuff.Core.ApplicationParts;
using System.Runtime.Serialization;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    [DataContract]
    public class PowerBiDataset : PowerBiResource, IName
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        public override string ToString() => $"{base.ToString()} name={Name}";
    }
}
