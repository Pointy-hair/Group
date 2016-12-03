using RevolutionaryStuff.Core.ApplicationParts;
using System.Runtime.Serialization;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    [DataContract]
    public class PowerBiTile : PowerBiEmbeddableResource, IName
    {
        [DataMember(Name = "title")]
        public string title { get; set; }

        string IName.Name
        {
            get { return title; }
        }
    }
}
