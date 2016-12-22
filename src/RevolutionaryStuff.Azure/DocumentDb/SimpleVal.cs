using Newtonsoft.Json;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    public class SimpleVal
    {
        public const string StringValuePropertyName = "sval";

        [JsonProperty(StringValuePropertyName)]
        public string StringValue { get; set; }
    }
}
