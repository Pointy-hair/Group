using System;
using System.Linq;

namespace RevolutionaryStuff.Core.Caching
{
    public class TraffkCache : ICacher
    {
        private readonly ISynchronizedRedisCache RedisCache;
        private readonly ICacher InnerDataCacher;

        public TraffkCache(ISynchronizedRedisCache redisCache)
        {
            RedisCache = redisCache;
            InnerDataCacher = Cache.DataCacher;
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

                return InnerDataCacher.FindOrCreate<TVal>(key, creator, forceCreate, timeout);
            }
            catch (Exception e)
            {
                return InnerDataCacher.FindOrCreate<TVal>(key, creator, forceCreate, timeout);
            }
        }
    }
}
