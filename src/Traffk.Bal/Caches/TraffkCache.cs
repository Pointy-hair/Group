using System;
using System.Diagnostics;
using System.Linq;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Traffk.Utility;

namespace Traffk.Bal.Caches
{
    public class TraffkCache : ICacher
    {
        private readonly ISynchronizedRedisCache RedisCache;
        private readonly ICacher InnerDataCacher;

        public TraffkCache(ISynchronizedRedisCache redisCache)
        {
            RedisCache = redisCache;
            InnerDataCacher = RevolutionaryStuff.Core.Caching.Cache.DataCacher;
        }

        CacheEntry<TVal> ICacher.FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator, bool forceCreate, TimeSpan? timeout)
        {
            try
            {
                var isSerializable = true;
                var isSerializableAttributes = typeof(TVal).GetCustomAttributes<IsSerializableAttribute>().ToList();
                if (isSerializableAttributes.Any())
                {
                    isSerializable = isSerializableAttributes[0].IsSerializable;
                }

                if (isSerializable)
                {
                    return RedisCache.FindOrCreate<TVal>(key, creator, forceCreate, timeout);
                }
                else
                {
                    return InnerDataCacher.FindOrCreate<TVal>(key, creator, forceCreate, timeout);
                }
            }
            catch (Exception ex)
            {
                //Catching this exception because deserialization fails on inaccessible libraries.
                Trace.WriteLine(ex);
                return InnerDataCacher.FindOrCreate<TVal>(key, creator, forceCreate, timeout);
            }
        }
    }
}
