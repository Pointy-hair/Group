using RevolutionaryStuff.Core;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    internal class DocumentDbHttpClient
    {
        protected readonly DdbContext Context;
        protected readonly ResourceTypes ResourceType;
        protected readonly RestApiVersions ApiVersion;
        protected readonly HttpClient Client = new HttpClient();

        public static class CommonHeaderNames
        {
            public const string PartitionKeyHeaderName = "x-ms-documentdb-partitionkey";
            public const string PreTriggerInclude = "x-ms-documentdb-pre-trigger-include";
            public const string PostTriggerInclude = "x-ms-documentdb-post-trigger-include";
            public const string ConsistencyLevel = "x-ms-consistency-level";
            public const string IsQuery = "x-ms-documentdb-isquery";
            public const string EnableCrossPartitionQuery = "x-ms-query-enable-crosspartition";
            public const string Continuation = "x-ms-continuation";
            public const string MaxItemsPerPage = "x-ms-max-item-count";
        }

        public static class CommonContentTypes
        {
            public const string JsonQuery = "application/query+json";
            public const string ContentTypeJson = "application/json";
        }

        public Uri Uri { get; }

        public DocumentDbHttpClient(DdbContext context, Uri uri, ResourceTypes resourceType, RestApiVersions apiVersion = RestApiVersions.Current)
        {
            Requires.NonNull(uri, nameof(uri));

            Context = context;
            Uri = uri;
            ResourceType = resourceType;
            ApiVersion = apiVersion;
        }

        private readonly IList<string> PartitionKeyValues = new List<string>();

        public void AddPartitionKeyValue(string partitionKeyValue)
        {
            PartitionKeyValues.Add(partitionKeyValue);

        }

        public void AddPreTriggers(IEnumerable<DocumentPreTriggerAttribute> triggers, DocumentTriggerOperations? triggerOperation)
        {
            AddTriggers(CommonHeaderNames.PreTriggerInclude, triggers, triggerOperation);
        }
        public void AddPostTriggers(IEnumerable<DocumentPostTriggerAttribute> triggers, DocumentTriggerOperations? triggerOperation)
        {
            AddTriggers(CommonHeaderNames.PostTriggerInclude, triggers, triggerOperation);
        }

        private void AddTriggers(string headerName, IEnumerable<DocumentTriggerAttribute> triggers, DocumentTriggerOperations? triggerOperation)
        {
            if (triggerOperation == null) return;
            var names = new HashSet<string>();
            foreach (var t in triggers)
            {
                if (t.Operations.HasFlag(triggerOperation.Value))
                {
                    names.Add(t.TriggerName);
                }
            }
            Requires.Between(names.Count, "trigger names", 0, 1);
            if (names.Count > 0)
            {
                AddHeader(headerName, names.Join(","));
            }
        }

        public void AddHeader(string headerName, string headerValue)
        {
            Client.DefaultRequestHeaders.Add(headerName, headerValue);
        }

        protected string GetId(bool munge)
        {
            var id = Uri.LocalPath.Substring(1);
            if (munge)
            {
                var idParts = id.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (idParts.Length.IsOdd())
                {
                    Array.Resize(ref idParts, idParts.Length - 1);
                    id = idParts.Join("/");
                }
            }
            return id;
        }

        private bool Called;

        public Task<string> GetStringAsync()
        {
            Requires.SingleCall(ref Called);
            InitializeRequest(WebHelpers.Methods.Get, ResourceType, GetId(true));
            return Client.GetStringAsync(Uri);
        }

        public Task<HttpResponseMessage> DeleteAsync()
        {
            Requires.SingleCall(ref Called);
            InitializeRequest(WebHelpers.Methods.Delete, ResourceType, GetId(true));
            return Client.DeleteAsync(Uri);
        }

        public Task<HttpResponseMessage> PostObjectAsJsonAsync(object o)
        {
            Requires.NonNull(o, nameof(o));
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(o));
            content.Headers.ContentType = new MediaTypeHeaderValue(CommonContentTypes.ContentTypeJson);
            return PostAsync(content);
        }

        public Task<HttpResponseMessage> PostAsync(HttpContent content)
        {
            Requires.SingleCall(ref Called);
            InitializeRequest(WebHelpers.Methods.Post, ResourceType, GetId(true));
            return Client.PostAsync(Uri, content);
        }

        public Task<HttpResponseMessage> PutObjectAsJsonAsync(object o)
        {
            Requires.NonNull(o, nameof(o));
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(o));
            content.Headers.ContentType = new MediaTypeHeaderValue(CommonContentTypes.ContentTypeJson);
            return PutAsync(content);
        }

        public Task<HttpResponseMessage> PutAsync(HttpContent content)
        {
            Requires.SingleCall(ref Called);
            InitializeRequest(WebHelpers.Methods.Put, ResourceType, GetId(true));
            return Client.PutAsync(Uri, content);
        }

        private void InitializeRequest(string verb, ResourceTypes resourceType, string resourceId)
        {
            var now = DateTime.UtcNow;
            if (ApiVersion >= RestApiVersions.FirstParitionKeyVersion)
            {
                AddHeader(CommonHeaderNames.PartitionKeyHeaderName, JsonConvert.SerializeObject(PartitionKeyValues.ToArray()));
            }
            else
            {
                Requires.Zero(PartitionKeyValues.Count, nameof(PartitionKeyValues));
            }
            Client.DefaultRequestHeaders.Add("x-ms-date", now.ToString("r"));
            Client.DefaultRequestHeaders.Add("x-ms-version", EnumeratedStringValueAttribute.GetValue(ApiVersion));
            Client.DefaultRequestHeaders.Add("authorization", GenerateMasterKeyAuthorizationSignature(now, verb, resourceId, resourceType, Context.Options.Value.AccountKey, "master", "1.0"));
        }

        private static string GenerateMasterKeyAuthorizationSignature(DateTime requestCalledAtUtc, string verb, string resourceId, ResourceTypes resourceType, string key, string keyType, string tokenVersion)
        {
            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };

            string payLoad = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n",
                    verb.ToLowerInvariant(),
                    EnumeratedStringValueAttribute.GetValue(resourceType),
                    resourceId,
                    requestCalledAtUtc.ToString("r").ToLowerInvariant(),
                    ""
            );

            byte[] hashPayLoad = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payLoad));
            string signature = Convert.ToBase64String(hashPayLoad);

            return WebUtility.UrlEncode(string.Format(System.Globalization.CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}",
                keyType,
                tokenVersion,
                signature));
        }
    }
}
