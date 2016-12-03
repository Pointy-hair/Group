using Newtonsoft.Json;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    public class DdbDocumentResults<TEntity> where TEntity : class
    {
        [JsonProperty("_rid")]
        public string Rid { get; set; }
        [JsonProperty("_count")]
        public int Count { get; set; }

        [JsonProperty("Documents")]
        public TEntity[] Documents { get; set; }

        public bool HasData => Documents != null && Documents.Length > 0;
    }
}
