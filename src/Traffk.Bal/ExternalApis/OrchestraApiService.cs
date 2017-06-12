﻿using System;
using RevolutionaryStuff.Core.Caching;
using Traffk.Bal.Services;
using Traffk.Orchestra;
using Traffk.Orchestra.Models;

namespace Traffk.Bal.ExternalApis
{
    public class OrchestraApiService
    {
        private readonly OrchestraRxApiClient Client;
        private readonly ICacher Cache;

        public OrchestraApiService(ISynchronizedRedisCache cache, OrchestraRxApiClient client, ITraffkTenantFinder tenantFinder)
        {
            var tenantId = tenantFinder.GetTenantIdAsync().Result;
            Cache = cache.CreateScope(tenantId);
            Client = client;

            var tokenResponse = this.FindOrCreateTokenResponseWithExpiration();

            Client.SetToken(tokenResponse);
        }

        private OrchestraRxTokenResponse FindOrCreateTokenResponseWithExpiration()
        {
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(OrchestraRxTokenResponse).Name);
            //Just a find
            var tokenResponseCacheEntry = Cache.FindOrCreate<OrchestraRxTokenResponse>(cacheKey);
            if (tokenResponseCacheEntry == null)
            {
                var tokenResponse = Client.Authenticate();
                var timeoutInSeconds = tokenResponse.expires_in;
                var tokenResponseAfterCaching = Cache.FindOrCreate(cacheKey, key => new CacheEntry<OrchestraRxTokenResponse>(tokenResponse), true, TimeSpan.FromSeconds(timeoutInSeconds));
                return tokenResponseAfterCaching.Value;
            }
            else
            {
                var tokenResponse = tokenResponseCacheEntry.Value;
                return tokenResponse;
            }
        }

        public PharmacyResponse PharmacySearch(string zip, int radius)
        {
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(PharmacyResponse).Name, zip, radius);

            var pharmacyResponse = Cache.FindOrCreate<PharmacyResponse>(cacheKey, key => Client.PharmacySearchAsync(zip,radius).Result);

            return pharmacyResponse?.Value;
        }

        public DrugResponse DrugSearch(string query)
        {
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(DrugResponse).Name, query);

            var drugResponse = Cache.FindOrCreate<DrugResponse>(cacheKey, key=> Client.DrugSearchAsync(query).Result);

            return drugResponse?.Value;
        }
    }
}
