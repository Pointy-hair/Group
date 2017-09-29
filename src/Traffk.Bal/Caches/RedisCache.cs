using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Traffk.Bal.Caches
{
    public interface IRedisCache : ICacher
    {
        ConnectionMultiplexer Connection { get; }
    }

    public class RedisCache : ICacher, IRedisCache
    {
        public class Config
        {
            public const string ConfigSectionName = "RedisCachingServicesOptions";
            public string ConnectionString { get; set; }
            public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromMinutes(5);
        }

        ConnectionMultiplexer IRedisCache.Connection => Connection;

        private static Config RedisOptions;
        private readonly IDatabase CacheDatabase;
        private readonly TimeSpan? DefaultExpiration;

        public RedisCache(IOptions<Config> redisOptions)
        {
            RedisOptions = redisOptions.Value;
            DefaultExpiration = RedisOptions.ExpirationTime;
            CacheDatabase = Connection.GetDatabase();
        }

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection
            = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(RedisOptions.ConnectionString));

        public static ConnectionMultiplexer Connection
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

    public class SynchronizedRedisCache : SynchronizedCacher, ISynchronizedRedisCache
    {
        public SynchronizedRedisCache(IRedisCache inner) : base(inner)
        {

        }
    }
}
