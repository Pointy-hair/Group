using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    public abstract class DdbEntity
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonIgnore]
        internal string InstanceKey
        {
            get
            {
                if (InstanceKey_p == null)
                {
                    InstanceKey_p = Id ?? Interlocked.Increment(ref NewInstanceCounter).ToString();
                }
                return InstanceKey_p;
            }
        }
        private string InstanceKey_p;
        private static int NewInstanceCounter;


        [JsonExtensionData]
        internal IDictionary<string, JToken> AdditionalData;

        public override string ToString() => $"{GetType().Name} id={Id}";
    }
}
