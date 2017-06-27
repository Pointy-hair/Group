using System;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.Caching;
using StackExchange.Redis;

namespace Traffk.Bal.Caches
{
    public interface IRedisCache : ICacher
    {
        ConnectionMultiplexer Connection { get; }
    }

    public class RedisCache : ICacher, IRedisCache
    {
        private readonly RedisCachingServicesOptions RedisOptions;
        private readonly Lazy<ConnectionMultiplexer> LazyConnection;
        private readonly IDatabase CacheDatabase;
        private readonly TimeSpan? DefaultExpiration;

        public RedisCache(IOptions<RedisCachingServicesOptions> redisOptions)
        {
            RedisOptions = redisOptions.Value;
            DefaultExpiration = RedisOptions.ExpirationTime;
            LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = RedisOptions.ConnectionString;
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
            CacheDatabase = Connection.GetDatabase();
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }

        private TVal CheckCache<TVal>(string key)
        {
            string serializedTokenResponse = CacheDatabase.StringGet(key);
            if (!String.IsNullOrEmpty(serializedTokenResponse))
            {
                var deserializedCacheEntry = JsonConvert.DeserializeObject<TVal>(serializedTokenResponse);
                return deserializedCacheEntry;
            }
            return default(TVal);
        }

        private void SetCacheEntry<TVal>(string key, TVal value, TimeSpan? expiration = null)
        {
            if (expiration == null)
            {
                expiration = DefaultExpiration;
            }
            
            CacheDatabase.StringSet(key, JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            }), 
            expiration);
        }

        CacheEntry<TVal> ICacher.FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator, bool forceCreate, TimeSpan? timeout)
        {
            var objectInCache = CheckCache<TVal>(key);
            if (forceCreate || objectInCache == null)
            {
                if (creator != null)
                {
                    objectInCache = creator(key).Value;
                    SetCacheEntry<TVal>(key, objectInCache, timeout);
                }
            }
            return new CacheEntry<TVal>(objectInCache);
        }
    }

    public interface ISynchronizedRedisCache : ICacher
    {
    }

    public class SynchronizedRedisCache : RevolutionaryStuff.Core.Caching.Cache.SynchronizedCacher, ISynchronizedRedisCache
    {
        public SynchronizedRedisCache(IRedisCache inner) : base(inner)
        {

        }
    }

    public class RedisCachingServicesOptions
    {
        public string ConnectionString { get; set; }
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromMinutes(5);

    }
}
