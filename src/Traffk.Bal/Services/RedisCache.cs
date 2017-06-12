using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Traffk.Bal.Services
{
    public class RedisCache : ICacher
    {
        private readonly RedisCachingServicesOptions RedisOptions;
        private readonly CachingServicesOptions CachingServicesOptions;
        private readonly Lazy<ConnectionMultiplexer> LazyConnection;
        private readonly IDatabase CacheDatabase;
        private readonly TimeSpan? DefaultExpiration;
        private readonly int TenantId;

        public RedisCache(IOptions<RedisCachingServicesOptions> redisOptions, 
            IOptions<CachingServicesOptions> cachingServicesOptions, 
            ITraffkTenantFinder tenantFinder)
        {
            TenantId = tenantFinder.GetTenantIdAsync().Result;
            RedisOptions = redisOptions.Value;
            CachingServicesOptions = cachingServicesOptions.Value;
            DefaultExpiration = CachingServicesOptions.Default.DurationByPeriod[FlushPeriods.Long.ToString()];
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

        public T CheckCache<T>(string key)
        {
            var scopedKey = TenantId + key;
            string serializedTokenResponse = CacheDatabase.StringGet(scopedKey);
            if (!String.IsNullOrEmpty(serializedTokenResponse))
            {
                return JsonConvert.DeserializeObject<T>(serializedTokenResponse);
            }
            return default(T);
        }

        public void SetCacheEntry<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration == null)
            {
                expiration = DefaultExpiration;
            }

            var scopedKey = TenantId + key;

            CacheDatabase.StringSet(scopedKey, JsonConvert.SerializeObject(value), expiration);
        }

        CacheEntry<TVal> ICacher.FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator, bool forceCreate)
        {
            ICacheEntry e = null;
            var scopedKey = TenantId + key;

            var cachedObject = CheckCache<TVal>(scopedKey);

            e = new CacheEntry<TVal>(cachedObject);

            if (forceCreate || cachedObject == null)
            {
                if (creator != null)
                {
                    e = creator(scopedKey);
                    SetCacheEntry(scopedKey, e);
                }
            }
            return e as CacheEntry<TVal>;
        }
    }

    public class RedisCachingServicesOptions
    {
        public string ConnectionString { get; set; }
    }

    public enum FlushPeriods
    {
        Short,
        Medium,
        Long,
    }

    public class CachingServicesOptions
    {
        public static readonly CachingServicesOptions Default = new CachingServicesOptions();

        static CachingServicesOptions()
        {
            Default = new CachingServicesOptions();
            Default.DurationByPeriod[FlushPeriods.Short.ToString()] = TimeSpan.FromSeconds(5);
            Default.DurationByPeriod[FlushPeriods.Medium.ToString()] = TimeSpan.FromMinutes(1);
            Default.DurationByPeriod[FlushPeriods.Long.ToString()] = TimeSpan.FromMinutes(5);
        }

        public Dictionary<string, TimeSpan> DurationByPeriod { get; } = new Dictionary<string, TimeSpan>();

        public CachingServicesOptions()
        {

        }
    }
}
