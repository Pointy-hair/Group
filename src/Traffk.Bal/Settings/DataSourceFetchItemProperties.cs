using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.EncoderDecoders;
using System;
using System.Collections.Generic;
using Traffk.Bal.Services;

namespace Traffk.Bal.Settings
{
    public class DataSourceFetchItemProperties
    {
        public static DataSourceFetchItemProperties CreateFromJson(string json)
            => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<DataSourceFetchItemProperties>(json);

        [JsonProperty("error")]
        public ExceptionError Error { get; set; }

        [JsonProperty("lastModifiedAtUtc")]
        public DateTime? LastModifiedAtUtc { get; set; }

        public string ToJson() 
            => JsonConvert.SerializeObject(this);

        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        private void SetOrRemoveMetadataValue(string key, string val)
        {
            if (val == null)
            {
                Metadata.Remove(key);
            }
            else
            {
                Metadata[key] = val;
            }
        }

        [JsonIgnore]
        public string ETag
        {
            get => Metadata.GetValue(BlobStorageServices.MetaKeyNames.ETag);
            set => SetOrRemoveMetadataValue(BlobStorageServices.MetaKeyNames.ETag, value);
        }

        [JsonIgnore]
        public byte[] ContentMD5
        {
            get
            {
                var z = Metadata.GetValue(BlobStorageServices.MetaKeyNames.ContentMD5);
                if (z != null)
                {
                    try
                    {
                        return Base16.Decode(z);
                    }
                    catch (Exception)
                    { }
                }
                return null;
            }
            set => SetOrRemoveMetadataValue(BlobStorageServices.MetaKeyNames.ContentMD5, value == null ? null : Base16.Encode(value));
        }

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
