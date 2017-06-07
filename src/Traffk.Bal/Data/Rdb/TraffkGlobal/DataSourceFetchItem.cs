using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Collections.Generic;
using Traffk.Bal.Services;

namespace Traffk.Bal.Data.Rdb.TraffkGlobal
{
    public partial class DataSourceFetchItem
    {
        public enum DataSourceFetchItemTypes
        {
            Duplicate,
            Original,
            Decrypted,
            Unpacked,
        }

        public static string CreateEvidenceFromNameSizeModified(string name, long? size, DateTime? lastModifiedAtUtc)
            => Cache.CreateKey(name, size, lastModifiedAtUtc);

        public static string CreateEvidenceFromUrn(string urn)
            => urn;

        public static ICollection<string> CreateEvidenceFromMeta(IDictionary<string, string> meta)
        {
            if (meta != null)
            {
                var urns = meta.FindOrDefault(BlobStorageServices.MetaKeyNames.Urns);
                if (urns != null)
                {
                    return CSV.ParseLine(urns);
                }
            }
            return Empty.StringArray;
        }

        [JsonIgnore]
        public ICollection<string> Evidence
        {
            get
            {
                var e = new List<string>(CreateEvidenceFromMeta(this.DataSourceFetchItemProperties.Metadata));
                e.Add(CreateEvidenceFromNameSizeModified(this.Name, this.Size, this.DataSourceFetchItemProperties.LastModifiedAtUtc));
                e.Add(CreateEvidenceFromNameSizeModified(this.DataSourceFetchItemProperties.Metadata.FindOrDefault(BlobStorageServices.MetaKeyNames.SourceFullName), this.Size, this.DataSourceFetchItemProperties.LastModifiedAtUtc));
                return e;
            }
        }
    }
}
