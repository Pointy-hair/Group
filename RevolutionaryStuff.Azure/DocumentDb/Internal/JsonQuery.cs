using Newtonsoft.Json;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Collections.Generic;
using RevolutionaryStuff.Core;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    public class JsonQuery : IValidate
    {
        [JsonIgnore]
        internal int? SkipN;

        [JsonIgnore]
        internal int? TakeN;

        public class Parameter
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("parameters")]
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public JsonQuery(string query)
        {
            Query = query;
        }

        public static implicit operator JsonQuery(string query)
        {
            return new JsonQuery(query);
        }

        public void Validate()
        {
            Requires.NonNull(Query, nameof(Query));
        }

        public override string ToString() => $"({Query}).Skip({SkipN.GetValueOrDefault()}).Take({TakeN.GetValueOrDefault()})";

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
