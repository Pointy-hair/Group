using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.Core.Collections;
using System;
using System.Collections.Generic;

namespace TraffkPortal.Services
{
    public class CachingServices
    {
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

        private static bool InitializeCalled;
        public static void Initialize(IServiceProvider provider)
        {
            Requires.SingleCall(ref InitializeCalled);
            Instance = provider.GetService<CachingServices>();
        }

        public static CachingServices Instance { get; private set; }

        private readonly IDictionary<FlushPeriods, PeriodicAction> Flushers = new Dictionary<FlushPeriods, PeriodicAction>();
        private readonly MultipleValueDictionary<FlushPeriods, IFlushable> FlushersByPeriod = new MultipleValueDictionary<FlushPeriods, IFlushable>();

        public CachingServices(IOptions<CachingServicesOptions> options)
        {
            Requires.NonNull(options, nameof(options));
            var cacheOptions = options.Value;
            foreach (var e in Stuff.GetEnumValues<FlushPeriods>())
            {
                TimeSpan timeout;
                if (!cacheOptions.DurationByPeriod.TryGetValue(e.ToString(), out timeout))
                {
                    timeout = CachingServicesOptions.Default.DurationByPeriod[e.ToString()];
                }
                Flushers[e] = new PeriodicAction(() => 
                {
                    lock (FlushersByPeriod)
                    {
                        foreach (var flusher in FlushersByPeriod[e])
                        {
                            flusher.Flush();
                        }
                    }
                }, timeout, timeout);
            }
            GenericCache = CreateSynchronized<string, object>(FlushPeriods.Medium);
        }

        private readonly ICache<string, object> GenericCache;

        public void Register(IFlushable flushable, FlushPeriods period)
        {
            Requires.NonNull(flushable, nameof(flushable));
            lock (FlushersByPeriod)
            {
                FlushersByPeriod.Add(period, flushable);
            }
        }

        public ICache<K, V> CreateSynchronized<K, V>(FlushPeriods period = FlushPeriods.Long)
        {
            var c = Cache.CreateSynchronized<K, V>();
            Register(c, period);
            return c; 
        }

        public TVal Do<TKey, TVal>(TKey key, Func<TVal> creator) => (TVal)GenericCache.Do(Cache.CreateKey(key, typeof(TKey), typeof(TVal)), () => creator);
    }
}
