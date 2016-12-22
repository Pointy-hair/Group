using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Runtime.Serialization;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    [DataContract]
    public abstract class PowerBiResource
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        public override string ToString()
        {
            var s = $"{GetType().Name} id={Id}";
            var iname = this as IName;
            if (iname != null)
            {
                s += $"name=[{iname.Name}]";
            }
            return s;
        }
    }


    [DataContract]
    public abstract class PowerBiEmbeddableResource : PowerBiResource
    {
        public override string ToString() => $"{base.ToString()} embedUrl=[{EmbedUrl}]";

        [DataMember(Name = "embedUrl")]
        public Uri EmbedUrl
        {
            get; set;
        }
    }
}
