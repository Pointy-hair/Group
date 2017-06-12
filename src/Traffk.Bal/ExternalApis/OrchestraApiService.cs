using System;
using Newtonsoft.Json;
using StackExchange.Redis;
using Traffk.Bal.Services;
using Traffk.Orchestra;
using Traffk.Orchestra.Models;

namespace Traffk.Bal.ExternalApis
{
    public class OrchestraApiService
    {
        private readonly OrchestraRxApiClient Client;
        private readonly RedisCache Cache;

        public OrchestraApiService(RedisCache cache, OrchestraRxApiClient client)
        {
            Cache = cache;
            Client = client;

            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(OrchestraRxTokenResponse).Name);
            var tokenResponse = Cache.CheckCache<OrchestraRxTokenResponse>(cacheKey);
            if (tokenResponse == null)
            {
                tokenResponse = Client.Authenticate();
                Cache.SetCacheEntry(cacheKey, tokenResponse, TimeSpan.FromSeconds(tokenResponse.expires_in));
            }

            Client.SetToken(tokenResponse);
        }

        public PharmacyResponse PharmacySearch(string zip, int radius)
        {
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(PharmacyResponse).Name + zip + radius);

            var pharmacyResponse = Cache.CheckCache<PharmacyResponse>(cacheKey);
            pharmacyResponse = pharmacyResponse ?? Client.PharmacySearch(zip, radius).Result;

            Cache.SetCacheEntry<PharmacyResponse>(cacheKey, pharmacyResponse);

            return pharmacyResponse;;
        }
        
    }
}
