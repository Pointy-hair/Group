using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Azure.DocumentDb.Internal;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.Azure.DocumentDb.Internal.AccountInformation;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Collections;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    /// <remarks>
    /// https://msdn.microsoft.com/library/azure/dn783368.aspx
    /// https://github.com/Azure/azure-documentdb-dotnet/blob/master/samples/rest-from-.net/Program.cs
    /// https://github.com/Azure/azure-documentdb-dotnet/issues/60
    /// https://msdn.microsoft.com/en-us/library/azure/dn781481.aspx
    /// </remarks>
    public abstract class DdbContext
    {
        private static readonly ICache<string, Account> AccountCache = Cache.CreateSynchronized<string, Account>();
        public static readonly IFlushable Flusher = new Flusher(() => AccountCache.Flush());

        public readonly IOptions<DdbOptions> Options;

        public readonly DdbChangeTracker ChangeTracker = new DdbChangeTracker();

        public DdbContext(IOptions<DdbOptions> options)
        {
            Requires.NonNull(options, nameof(options));

            Options = options;

            foreach (var pi in this.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (pi.GetValue(this) != null) continue;
                var ci = pi.PropertyType.GetConstructors().FirstOrDefault(z => z.GetParameters().Length == 1 && z.GetParameters()[0].ParameterType.IsA(typeof(DdbContext)));
                if (ci == null) continue;
                var docset = ci.Invoke(new[] { this });
                pi.SetValue(this, docset);
            }
        }

        public void Update<TEntity>(TEntity entity) where TEntity : DdbEntity
        {
            Requires.NonNull(entity, nameof(entity));
            ChangeTracker.Find(entity).State = EntityState.Modified;
        }

        public void SaveChanges()
        {
            SaveChangesAsync().ExecuteSynchronously();
        }

        public Task InsertEntitiesAsync<TEntity>(params TEntity[] entities) where TEntity : DdbEntity
        {
            return SaveAsync(entities.ConvertAll(e => new DocSetEntry<TEntity>(e, EntityState.Added)));
        }

        public Task InsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : DdbEntity
        {
            return SaveAsync(entities.ConvertAll(e => new DocSetEntry<TEntity>(e, EntityState.Added)));
        }

        public virtual Task SaveChangesAsync()
        {
            return SaveAsync(ChangeTracker.EntityEntryList<DdbEntity>(EntityState.Added, EntityState.Modified, EntityState.Deleted));
        }

        private async Task SaveAsync(IEnumerable<DocSetEntry> entries)
        {
            var touchedEntriesByType = entries.ForEach(delegate (DocSetEntry z) { if (z.State == EntityState.Added) { z.Entity.Id = z.Entity.Id ?? Guid.NewGuid().ToString(); } }).ToMultipleValueDictionary(z => z.GetType());

            foreach (var entryType in touchedEntriesByType.Keys)
            {
                var entityType = entryType.GenericTypeArguments[0];
                var dca = entityType.GetCustomAttribute<DocumentCollectionAttribute>();
                var preTriggers = entityType.GetCustomAttributes<DocumentPreTriggerAttribute>();
                var postTriggers = entityType.GetCustomAttributes<DocumentPostTriggerAttribute>();
                Debug.Assert(dca != null);
                var paritionKeyMemberInfo = entityType.GetAttributedMembers<PartitionKeyAttribute>(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public).SingleOrDefault();
                foreach (var entry in touchedEntriesByType[entryType])
                {
                    var entity = entry.Entity;
                    Uri url;
                    DocumentTriggerOperations? triggerOperation = null;
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            url = CreateDatabaseCollectionDocumentsUrl(dca);
                            triggerOperation = DocumentTriggerOperations.Create;
                            break;
                        case EntityState.Deleted:
                            url = CreateDatabaseCollectionDocumentsUrl(dca, entity.Id);
                            triggerOperation = DocumentTriggerOperations.Delete;
                            break;
                        case EntityState.Modified:
                            url = CreateDatabaseCollectionDocumentsUrl(dca, entity.Id);
                            triggerOperation = DocumentTriggerOperations.Replace;
                            break;
                        default:
                            throw new UnexpectedSwitchValueException(entry.State);
                    }
                    entry.State = await CallAllowingForTransientNetworkingFailuresAsync(async delegate ()
                    {
                        var client = new DocumentDbHttpClient(this, url, ResourceTypes.Document);
                        client.AddPreTriggers(preTriggers, triggerOperation);
                        client.AddPostTriggers(postTriggers, triggerOperation);
                        if (paritionKeyMemberInfo != null)
                        {
                            client.AddPartitionKeyValue(paritionKeyMemberInfo.GetValue(entity).ToString());
                        }
                        EntityState successState;
                        HttpResponseMessage resp;
                        switch (entry.State)
                        {
                            case EntityState.Added:
                                client.AddHeader(WebHelpers.HeaderStrings.AcceptTypes, DocumentDbHttpClient.CommonContentTypes.ContentTypeJson);
                                resp = await client.PostObjectAsJsonAsync(entity);
                                successState = EntityState.Unchanged;
                                break;
                            case EntityState.Deleted:
                                resp = await client.DeleteAsync();
                                successState = EntityState.Deleted;
                                break;
                            case EntityState.Modified:
                                client.AddHeader(WebHelpers.HeaderStrings.AcceptTypes, DocumentDbHttpClient.CommonContentTypes.ContentTypeJson);
                                resp = await client.PutObjectAsJsonAsync(entity);
                                successState = EntityState.Unchanged;
                                break;
                            default:
                                throw new UnexpectedSwitchValueException(entry.State);

                        }
                        if (!resp.IsSuccessStatusCode) throw new WebResponseException($"Persistence({entry.State}) of object {entity.ToString()} failed", resp);
                        return successState;
                    });
                }
            }
        }

        private class WebResponseException : Exception
        {
            public HttpResponseMessage Resp { get; }

            public WebResponseException(string message, HttpResponseMessage resp)
                : base(message)
            {
                Resp = resp;
            }
        }

        private static async Task<TRet> CallAllowingForTransientNetworkingFailuresAsync<TRet>(Func<Task<TRet>> func)
        {
            return await DelegateHelpers.CallAndRetryOnFailureAsync(func, exceptionChecker: delegate (Exception ex)
            {
                var wex = ex as WebResponseException;
                if (wex == null || wex.Resp == null) return false;
                switch ((int)wex.Resp.StatusCode)
                {
                    case 429: //too many requests
                    case 503: //ServiceUnavailable
                        return true;
                }
                return false;
            });
        }

        private class NotAnEntity : DdbEntity { }

        public IAsyncEnumerable<TResult> Query<TResult>(string databaseName, string collectionName, JsonQuery query) where TResult : class => new QueryAsyncEnumerable<TResult, NotAnEntity>(this, databaseName, collectionName, query);

        internal IAsyncEnumerable<TResult> Query<TResult, TEntity>(string databaseName, string collectionName, JsonQuery query) where TResult : class where TEntity : DdbEntity
        {
            return new QueryAsyncEnumerable<TResult, TEntity>(this, databaseName, collectionName, query);
        }

        private class QueryAsyncEnumerable<TResult, TEntity> : IEnumerable<TResult>, IAsyncEnumerable<TResult>, IAsyncEnumerator<TResult> where TResult : class where TEntity : DdbEntity
        {
            private readonly DdbContext Context;
            private readonly bool IsTypeTrackable;
            private readonly JsonQuery Query;
            private readonly string DatabaseName;
            private readonly string CollectionName;
            private int TakeN;
            private int SkipN;

            public QueryAsyncEnumerable(DdbContext context, string databaseName, string collectionName, JsonQuery query)
            {
                Requires.Valid(query, nameof(query));

                Query = query;
                IsTypeTrackable = typeof(TResult).IsA(typeof(TEntity));
                Context = context;
                DatabaseName = databaseName;
                CollectionName = collectionName;
                TakeN = Query.TakeN.GetValueOrDefault(int.MaxValue);
                SkipN = Stuff.Max(0, Query.SkipN.GetValueOrDefault());
            }

            public TResult Current => CurrentResultSet.Documents[CurrentResultIndex];

            private bool GetEnumeratorCalled;

            IAsyncEnumerator<TResult> IAsyncEnumerable<TResult>.GetEnumerator()
            {
                Requires.SingleCall(ref GetEnumeratorCalled);
                return this;
            }

            private DdbDocumentResults<TResult> CurrentResultSet;
            private int CurrentResultIndex;
            private string ContinuationToken;

            private DocumentDbHttpClient CreateClient()
            {
                var client = new DocumentDbHttpClient(Context, Context.CreateDatabaseCollectionsQueryUrl(DatabaseName, CollectionName), ResourceTypes.Query);
                client.AddHeader(DocumentDbHttpClient.CommonHeaderNames.IsQuery, "true");
                client.AddHeader(DocumentDbHttpClient.CommonHeaderNames.EnableCrossPartitionQuery, "false");
                client.AddHeader(WebHelpers.HeaderStrings.AcceptTypes, DocumentDbHttpClient.CommonContentTypes.ContentTypeJson);
                client.AddHeader(DocumentDbHttpClient.CommonHeaderNames.MaxItemsPerPage, "100");
                if (ContinuationToken != null)
                {
                    client.AddHeader(DocumentDbHttpClient.CommonHeaderNames.Continuation, ContinuationToken);
                }
                return client;
            }

            private HttpContent CreateContent()
            {
                var content = new StringContent(Query.ToJson());
                content.Headers.ContentType = new MediaTypeHeaderValue(DocumentDbHttpClient.CommonContentTypes.JsonQuery);
                return content;
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                Again:
                if (CurrentResultSet == null)
                {
                    var client = CreateClient();
                    var resp = await client.PostAsync(CreateContent());
                    ContinuationToken = resp.Headers.GetValueOrDefault(DocumentDbHttpClient.CommonHeaderNames.Continuation);
                    var json = await resp.Content.ReadAsStringAsync();
                    CurrentResultSet = JsonConvert.DeserializeObject<DdbDocumentResults<TResult>>(json);
                    if (IsTypeTrackable && CurrentResultSet.HasData)
                    {
                        CurrentResultSet.Documents = Context.ChangeTracker.Track(CurrentResultSet.Documents.OfType<TEntity>(), EntityState.Unchanged).ConvertAll(z => (TResult)(object)z.Entity).ToArray();
                    }
                    CurrentResultIndex = -1;
                }
                if (CurrentResultSet.Documents != null && ++CurrentResultIndex < CurrentResultSet.Documents.Length)
                {
                    if (SkipN > 0)
                    {
                        if (SkipN >= CurrentResultSet.Documents.Length)
                        {
                            Debug.Assert(CurrentResultIndex == 1);
                            SkipN -= CurrentResultSet.Documents.Length;
                            goto FetchNextBlock;
                        }
                        else
                        {
                            CurrentResultIndex += SkipN;
                            SkipN = 0;
                        }
                    }
                    return TakeN-- > 0;
                }
                if (ContinuationToken == null) return false;
                FetchNextBlock:
                CurrentResultSet = null;
                goto Again;
            }

            void IDisposable.Dispose()
            { }

            IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
            {
                while (MoveNext(CancellationToken.None).ExecuteSynchronously())
                {
                    yield return Current;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                while (MoveNext(CancellationToken.None).ExecuteSynchronously())
                {
                    yield return Current;
                }
            }
        }


        internal async Task<DdbDocumentResults<TEntity>> QueryDocuments<TEntity>(string databaseName, string collectionName) where TEntity : class
        {
            var json = await GetDatabaseCollectionDocumentsJson(databaseName, collectionName);
            var res = JsonConvert.DeserializeObject<DdbDocumentResults<TEntity>>(json);
            return res;
        }

        private Account Account
        {
            get
            {
                if (Account_p == null)
                {
                    Account_p = AccountCache.Do(Options.Value.DatabaseAccount, () =>
                    {
                        var client = new DocumentDbHttpClient(this, CreateAccountUrl(), ResourceTypes.Account);
                        var json = client.GetStringAsync().ExecuteSynchronously();
                        return JsonConvert.DeserializeObject<Account>(json);
                    }
                   );
                }
                return Account_p;
            }
        }
        private Account Account_p;

        private string WriteableEndpoint
        {
            get
            {
                if (WriteableEndpoint_p == null)
                {
                    WriteableEndpoint_p = Account.writableLocations.First().databaseAccountEndpoint;
                }
                return WriteableEndpoint_p;
            }
        }
        private string WriteableEndpoint_p;


        #region Well Known Clients

        private Task<string> GetDatabasesJson(string databaseName = null) => new DocumentDbHttpClient(this, CreateDatabasesUrl(databaseName), ResourceTypes.Database).GetStringAsync();
        private Task<string> GetDatabaseCollectionsJson(string databaseName, string collectionName = null) => new DocumentDbHttpClient(this, CreateDatabaseCollectionsUrl(databaseName, collectionName), ResourceTypes.Collection).GetStringAsync();
        private Task<string> GetDatabaseCollectionDocumentsJson(string databaseName, string collectionName, string docName = null) => new DocumentDbHttpClient(this, CreateDatabaseCollectionDocumentsUrl(databaseName, collectionName, docName), ResourceTypes.Document).GetStringAsync();

        #endregion

        #region Well Known Uri
        private Uri CreateAccountUrl() => new Uri($"https://{Options.Value.DatabaseAccount}.documents.azure.com/");
        private Uri CreateDatabasesUrl(string databaseName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}");
        private Uri CreateDatabaseUsersUrl(string databaseName, string userName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/users/{userName}");
        private Uri CreateDatabaseUserPermissionsUrl(string databaseName, string userName, string permissionName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/users/{userName}/permissions/{permissionName}");
        private Uri CreateDatabaseCollectionsUrl(string databaseName, string collectionName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}");
        private Uri CreateDatabaseCollectionsQueryUrl(string databaseName, string collectionName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}/docs");
        private Uri CreateDatabaseCollectionStoredProceduresUrl(string databaseName, string collectionName, string sprocName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}/sprocs/{sprocName}");
        private Uri CreateDatabaseCollectionTriggersUrl(string databaseName, string collectionName, string triggerName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}/triggers/{triggerName}");
        private Uri CreateDatabaseCollectionUserDefinedFunctionsUrl(string databaseName, string collectionName, string udfName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}/udfs/{udfName}");
        private Uri CreateDatabaseCollectionDocumentsUrl(DocumentCollectionAttribute dca, string docName = null) => CreateDatabaseCollectionDocumentsUrl(dca.DatabaseName, dca.CollectionName, docName);
        private Uri CreateDatabaseCollectionDocumentsUrl(string databaseName, string collectionName, string docName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}/docs/{docName}");
        private Uri CreateDatabaseCollectionDocumentAttachmentsUrl(string databaseName, string collectionName, string docName, string attachmentName = null) => new Uri($"{WriteableEndpoint}dbs/{databaseName}/colls/{collectionName}/docs/{docName}/attachments/{attachmentName}");
        private Uri CreateOffersUrl(string offerName = null) => new Uri($"{WriteableEndpoint}offers/{offerName}");

        #endregion

        public Task<TResult> CallSprocAsync<TCollectionEntity, TResult>(string sprocName, params object[] args) where TCollectionEntity : DdbEntity
        {
            var t = typeof(TCollectionEntity);
            var dca = t.GetCustomAttribute<DocumentCollectionAttribute>();
            return CallSprocAsync<TResult>(dca.DatabaseName, dca.CollectionName, sprocName, args);
        }

        public async Task<TResult> CallSprocAsync<TResult>(string databaseName, string collectionName, string sprocName, params object[] args)
        {
            return await CallAllowingForTransientNetworkingFailuresAsync(async delegate ()
            {
                var client = new DocumentDbHttpClient(this, CreateDatabaseCollectionStoredProceduresUrl(databaseName, collectionName, sprocName), ResourceTypes.StoredProcedure);
                client.AddHeader(WebHelpers.HeaderStrings.AcceptTypes, DocumentDbHttpClient.CommonContentTypes.ContentTypeJson);
                client.AddHeader(WebHelpers.HeaderStrings.CacheControl, "no-cache");
                client.AddHeader(DocumentDbHttpClient.CommonHeaderNames.ConsistencyLevel, "Session");
                var resp = await client.PostObjectAsJsonAsync(args);
                if (!resp.IsSuccessStatusCode) throw new WebResponseException($"CallSprocAsync({databaseName},{collectionName},{sprocName}) failed with {resp.StatusCode}", resp);
                var json = await resp.Content.ReadAsStringAsync();
                if (typeof(TResult) == typeof(DoNotCare)) return default(TResult);
                return JsonConvert.DeserializeObject<TResult>(json);
            });
        }
    }
}
