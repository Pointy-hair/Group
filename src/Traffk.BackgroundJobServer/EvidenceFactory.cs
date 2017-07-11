using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.Core.EncoderDecoders;
using System;
using System.Collections.Generic;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;

namespace Traffk.BackgroundJobServer
{
    internal static class EvidenceFactory
    {
        private static string CreateFromNameSizeModified(string name, long? size, DateTime? lastModifiedAtUtc)
            => Cache.CreateKey(name, size, lastModifiedAtUtc);

        private static string CreateFromUrn(string urn)
            => urn;

        private static string CreateFromContentMD5(byte[] md5)
            => Base16.Encode(md5);

        private static string CreateFromContentMD5(string md5)
            => md5;

        private static string CreateFromContentETag(string etag)
            => etag;

        public static ICollection<string> CreateEvidence(this DataSourceFetchItem item)
        {
            Requires.NonNull(item, nameof(item));

            var e = new List<string>();
            var meta = item.DataSourceFetchItemProperties?.Metadata;
            if (meta != null)
            {
                var urns = meta.FindOrDefault(BlobStorageServices.MetaKeyNames.Urns);
                if (urns != null)
                {
                    foreach (var urn in CSV.ParseLine(urns))
                    {
                        e.Add(CreateFromUrn(urn));
                    }
                }
                if (meta.ContainsKey(BlobStorageServices.MetaKeyNames.ETag))
                {
                    e.Add(CreateFromContentETag(meta[BlobStorageServices.MetaKeyNames.ETag]));
                }
                if (meta.ContainsKey(BlobStorageServices.MetaKeyNames.ContentMD5))
                {
                    e.Add(CreateFromContentMD5(meta[BlobStorageServices.MetaKeyNames.ContentMD5]));
                }
            }
            e.Add(CreateFromNameSizeModified(item.Name, item.Size, item.DataSourceFetchItemProperties.LastModifiedAtUtc));
            e.Add(CreateFromNameSizeModified(item.DataSourceFetchItemProperties.Metadata.FindOrDefault(BlobStorageServices.MetaKeyNames.SourceFullName), item.Size, item.DataSourceFetchItemProperties.LastModifiedAtUtc));
            return e;
        }

        public static ICollection<string> CreateEvidence(this DataSourceSyncRunner.FileDetails item)
        {
            Requires.NonNull(item, nameof(item));

            var e = new List<string>();
            foreach (var urn in item.Urns)
            {
                e.Add(CreateFromUrn(urn));
            }
            if (item.ETag != null)
            {
                e.Add(CreateFromContentETag(item.ETag));
            }
            if (item.ContentMD5 != null)
            {
                e.Add(CreateFromContentMD5(item.ContentMD5));
            }
            e.Add(CreateFromNameSizeModified(item.Name, item.Size, item.LastModifiedAtUtc));
            e.Add(CreateFromNameSizeModified(item.FullName, item.Size, item.LastModifiedAtUtc));
            return e;
        }
    }
}
