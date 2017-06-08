using StackExchange.Redis;
using System;
using TraffkPortal;

namespace Traffk.Portal.Services
{
    public class RedisCachingServices
    {
        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = Startup.RedisConnectionString;
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
