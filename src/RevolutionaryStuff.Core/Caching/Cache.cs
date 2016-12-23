﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RevolutionaryStuff.Core.Crypto;
using System.Reflection;
using System.Threading.Tasks;

namespace RevolutionaryStuff.Core.Caching
{
    /// <summary>
    /// Implements extensions and helpers for interacting with the ICache and ICacher interfaces
    /// </summary>
    public static class Cache
    {
        private static readonly IDictionary<int, object> LockByKey = new Dictionary<int, object>();

        private static int GetLockKeyName(object cacheGuy, object key) => (cacheGuy.GetHashCode() ^ (key ?? "").GetHashCode()) & 0x0FFF;

        public static CacheEntry<TVal> FindOrCreate<TVal>(this ICacher cacher, string key, Func<string, TVal> creator)
        {
            return cacher.FindOrCreate(key, k => {
                var val = creator(k);
                return new CacheEntry<TVal>(val);
            });
        }

        public class BasicCacher : ICacher
        {
            private readonly IDictionary<string, ICacheEntry> EntriesByKey = new Dictionary<string, ICacheEntry>();

            public CacheEntry<TVal> FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator)
            {
                ICacheEntry e;
                if (!EntriesByKey.TryGetValue(key, out e) || e.IsExpired)
                {
                    e = creator(key);
                    EntriesByKey[key] = e;
                }
                return e as CacheEntry<TVal>;
            }
        }

        private class PassthroughCacher : ICacher
        {
            CacheEntry<TVal> ICacher.FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator) => creator(key);
        }

        public static readonly ICacher DataCacher = new SynchronizedCacher(new BasicCacher());

        public static readonly ICacher Passthrough = new PassthroughCacher();

        private class SynchronizedCacher : ICacher
        {
            private readonly ICacher Inner;

            public SynchronizedCacher(ICacher inner)
            {
                Requires.NonNull(inner, nameof(inner));
                Inner = inner;
            }

            CacheEntry<TVal> ICacher.FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator)
            {
                var lockName = GetLockKeyName(Inner, key);
                Start:
                object o;
                lock (LockByKey)
                {
                    if (!LockByKey.TryGetValue(lockName, out o))
                    {
                        o = new object();
                        LockByKey[lockName] = o;
                    }
                    if (Monitor.TryEnter(o)) goto Run;
                }
                Monitor.Enter(o);
                Monitor.Exit(o);
                goto Start;
                Run:
                try
                {
                    return Inner.FindOrCreate(key, creator);
                }
                finally
                {
                    lock (LockByKey)
                    {
                        LockByKey.Remove(lockName);
                    }
                    Monitor.Exit(o);
                }
            }
        }

        private class ScopedCacher : ICacher
        {
            private readonly ICacher Inner;
            private readonly string ScopeKey;

            public ScopedCacher(ICacher inner, params object[] keyParts)
            {
                Requires.NonNull(inner, nameof(inner));
                Inner = inner;
                ScopeKey = CreateKey(keyParts);
            }

            public CacheEntry<TVal> FindOrCreate<TVal>(string key, Func<string, CacheEntry<TVal>> creator)
                => Inner.FindOrCreate(CreateKey(key, ScopeKey), creator);
        }

        public static ICacher Synchronized(ICacher inner) => inner as SynchronizedCacher ?? new SynchronizedCacher(inner);

        public static ICacher CreateScope(this ICacher inner, params object[] keyParts) => new ScopedCacher(inner, keyParts);

        public static CacheEntry<TVal> FindOrCreate<TVal>(this ICacher inner, string key, Func<string, Task<CacheEntry<TVal>>> asynccreator)
        {
            return inner.FindOrCreate(key, k => asynccreator(k).ExecuteSynchronously());
        }

        public static ICache<K, V> CreateSynchronized<K, V>(int maxItems = int.MaxValue)
        {
            return new SynchronizedCache<K, V>(new RandomCache<K, V>(maxItems));
        }

        public static bool ContainsKey<K, D>(this ICache<K, D> cache, K key)
        {
            D data;
            return cache.Find(key, out data);
        }

        public static void Refresh<K, D>(this ICache<K, D> cache, Action creator)
        {
            Stuff.SingleActor(creator, cache);
        }

        public static D Do<K, D>(this ICache<K, D> cache, K key, Func<D> creator)
        {
            return Do(cache, key, false, true, z=>creator());
        }

        public static D Do<K, D>(this ICache<K, D> cache, K key, Func<D, D> creator)
        {
            return Do(cache, key, false, true, creator);
        }

        public static D Do<K, D>(this ICache<K, D> cache, K key, bool mustBeFresh, bool storeNull, Func<D,D> creator) //where D : class
        {
            D v = default(D);
        Start:
            if (!cache.Find(key, out v) || mustBeFresh)
            {
                var lockName = GetLockKeyName(cache, key);
                object o;
                lock (LockByKey)
                {
                    if (!LockByKey.TryGetValue(lockName, out o))
                    {
                        o = new object();
                        LockByKey[lockName] = o;
                    }
                    if (Monitor.TryEnter(o)) goto Run;
                }
                Monitor.Enter(o);
                Monitor.Exit(o);
                goto Start;
            Run:
                try
                {
                    v = creator(v);
                    if (v != null || storeNull)
                    {
                        cache.Add(key, v);
                    }
                }
                finally
                {
                    lock (LockByKey)
                    {
                        LockByKey.Remove(lockName);
                    }
                    Monitor.Exit(o);
                }
            }
            return v;
        }


        public static IDictionary<K, V> Do<K, V>(this ICache<K, V> cache, IEnumerable<K> keys, bool mustBeFresh, Func<IEnumerable<K>, IDictionary<K, V>> creator)
        {
            var ret = new Dictionary<K, V>();
            List<K> misses = null;
            foreach (var k in keys)
            {
                V v;
                if (!mustBeFresh && cache.Find(k, out v))
                {
                    ret[k] = v;
                }
                else
                {
                    misses = misses ?? new List<K>();
                    misses.Add(k);
                }
            }
            if (misses != null)
            {
                var hits = creator(misses);
                foreach (var kvp in hits)
                {
                    var k = kvp.Key;
                    var v = kvp.Value;
                    ret.Add(k, v);
                    cache.Add(k, v);
                }
            }
            return ret;
        }

        public static V Find<K, V>(this ICache<K, V> cache, K key)
        {
            V retVal = default(V);

            cache.Find(key, out retVal);

            return retVal;
        }

        public static IList<V> DoInOrder<K, V>(this ICache<K, V> cache, IEnumerable<K> keys, bool mustBeFresh, Func<IEnumerable<K>, IDictionary<K, V>> creator)
        {
            var d = cache.Do<K, V>(keys, mustBeFresh, creator);
            return d.ToOrderedValuesList(keys);
        }

        public static V Find<K, V>(ICache<K, V> cache, K key, V missingVal)
        {
            V v;
            if (cache.Find(key, out v)) return v;
            return missingVal;
        }

        public static ICache<K, D> Synchronized<K, D>(ICache<K, D> inner)
        {
            return new SynchronizedCache<K, D>(inner);
        }

        public static string CreateKey(params object[] args)
        {
            var sb = new StringBuilder();
            for (int pos = 0; pos < args.Length; ++pos)
            {
                object o = args[pos];
                if (o == null || o is string)
                {
                    Stuff.Noop();
                }
                else if (o is bool)
                {
                    o = (bool)o ? 1 : 0;
                }
                else if (o.GetType().GetTypeInfo().IsEnum)
                {
                    o = Convert.ToUInt64(o);
                }
                else if (o is IEnumerable)
                {
                    o = (o as IEnumerable).Format(",");
                }
                else if (o is Type)
                {
                    o = ((Type)o).FullName;
                }
                else if (o is TimeSpan)
                {
                    o = ((TimeSpan)o).TotalMilliseconds;
                }
                sb.AppendFormat("{0}: {1}\n", pos, o);
            }
            return CanonicalizeCacheKey(sb.ToString());
        }

        private static string CanonicalizeCacheKey(string key)
        {
            if (key == null) return "special:__NULL";
            if (key.Length < 123) return "lit:" + key;
            byte[] buf = Encoding.UTF8.GetBytes(key);
            return string.Format("urn:crc32:{0}{1}", CRC32Checksum.Do(buf), key.GetHashCode());
        }
    }
}