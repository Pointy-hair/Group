using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Runtime.Serialization;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    [DataContract]
    public class PowerBiReport : PowerBiEmbeddableResource, IName
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "webUrl")]
        public Uri WebUrl { get; set; }

        public override string ToString() => $"{base.ToString()} webUrl=[{WebUrl}]";
    }
}
