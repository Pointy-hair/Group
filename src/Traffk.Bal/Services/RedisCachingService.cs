using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Traffk.Bal.Services
{
    public class RedisCachingService
    {
        private readonly RedisCachingServicesOptions Options;
        private readonly Lazy<ConnectionMultiplexer> LazyConnection;

        public RedisCachingService(IOptions<RedisCachingServicesOptions> options)
        {
            Options = options.Value;
            LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = Options.ConnectionString;
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }
    }

    public class RedisCachingServicesOptions
    {
        public string ConnectionString { get; set; }
    }
}
