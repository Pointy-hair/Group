using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Traffk.Bal.Services;

namespace Traffk.Bal.Settings
{
    public class DataSourceFetchItemProperties
    {
        public static DataSourceFetchItemProperties CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<DataSourceFetchItemProperties>(json);
        }

        [JsonProperty("lastModifiedAtUtc")]
        public DateTime? LastModifiedAtUtc { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);

        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public void Set(BlobStorageServices.FileProperties p)
        {
            Metadata = Metadata ?? new Dictionary<string, string>();
            if (p.Metadata != null)
            {
                foreach (var kvp in p.Metadata)
                {
                    Metadata[kvp.Key] = kvp.Value;
                }
            }
            this.LastModifiedAtUtc = p.LastModifiedAtUtc;
        }
    }
}
