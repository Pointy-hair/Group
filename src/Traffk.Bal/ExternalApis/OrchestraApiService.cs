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
        private readonly IDatabase Cache;

        public OrchestraApiService(RedisCachingService cache, OrchestraRxApiClient client)
        {
            Cache = cache.Connection.GetDatabase();
            Client = client;

            var cachedToken = CheckCache<OrchestraRxTokenResponse>(typeof(OrchestraRxTokenResponse).Name);
            Client.SetToken(cachedToken);
        }

        public PharmacyResponse PharmacySearch(string zip, int radius)
        {
            var cacheKey = typeof(PharmacyResponse).Name + zip + radius;

            var pharmacyResponse = CheckCache<PharmacyResponse>(cacheKey);
            pharmacyResponse = pharmacyResponse ?? Client.PharmacySearch(zip, radius).Result;

            SetCacheEntry<PharmacyResponse>(cacheKey, pharmacyResponse);

            return pharmacyResponse;;
        }

        private T CheckCache<T>(string key) where T : class
        {
            string serializedTokenResponse = Cache.StringGet(key);
            return JsonConvert.DeserializeObject<T>(serializedTokenResponse);
        }

        private void SetCacheEntry<T>(string key, T value) where T : class
        {
            Cache.StringSet(key, JsonConvert.SerializeObject(value));
        }
    }
}
