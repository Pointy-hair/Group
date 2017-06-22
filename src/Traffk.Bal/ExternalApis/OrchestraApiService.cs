using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.ApiModels.Rx;
using Traffk.Orchestra;
using Traffk.Orchestra.Models;
using Drug = Traffk.Bal.Data.ApiModels.Rx.Drug;
using DrugResponse = Traffk.Orchestra.Models.DrugResponse;
using PharmacyResponse = Traffk.Orchestra.Models.PharmacyResponse;

namespace Traffk.Bal.ExternalApis
{
    public class OrchestraApiService
    {
        private readonly OrchestraRxApiClient Client;
        private readonly ICacher ScopedCacher;

        public OrchestraApiService(ISynchronizedRedisCache cache, OrchestraRxApiClient client, ITraffkTenantFinder tenantFinder)
        {
            var tenantId = tenantFinder.GetTenantIdAsync().Result;
            ScopedCacher = cache.CreateScope(tenantId);
            Client = client;

            var tokenResponse = this.FindOrCreateTokenResponseWithExpiration();

            Client.SetToken(tokenResponse);
        }

        private OrchestraRxTokenResponse FindOrCreateTokenResponseWithExpiration()
        {
            var cacheKey = Cache.CreateKey(typeof(OrchestraRxTokenResponse).Name);
            //Just a find
            var tokenResponseCacheEntry = ScopedCacher.FindOrCreate<OrchestraRxTokenResponse>(cacheKey);
            if (tokenResponseCacheEntry.Value == null)
            {
                var tokenResponse = Client.Authenticate();
                var timeoutInSeconds = tokenResponse.expires_in;
                var tokenResponseAfterCaching = ScopedCacher.FindOrCreate(cacheKey, key => new CacheEntry<OrchestraRxTokenResponse>(tokenResponse), true, TimeSpan.FromSeconds(timeoutInSeconds));
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
            var cacheKey = Cache.CreateKey(typeof(PharmacyResponse).Name, zip, radius);

            var pharmacyResponse = ScopedCacher.FindOrCreate<PharmacyResponse>(cacheKey, key => Client.PharmacySearchAsync(zip,radius).Result);

            return pharmacyResponse?.Value;
        }

        public Data.ApiModels.Rx.DrugResponse DrugSearch(string query)
        {
            var cacheKey = Cache.CreateKey(typeof(DrugResponse).Name, query, "traffk");
            var tDrugResponse = ScopedCacher.FindOrCreate<Data.ApiModels.Rx.DrugResponse>(cacheKey, outerKey =>
            {
                var searchCacheKey = Cache.CreateKey(typeof(DrugResponse).Name, query);
                var oDrugResponse = ScopedCacher.FindOrCreate<DrugResponse>(searchCacheKey,
                    key => Client.DrugSearchAsync(query).Result);
                var oDrugs = oDrugResponse.Value.Drugs.ToList();

                var tDrugs = new List<Drug>();

                Parallel.ForEach(oDrugs, (oDrug) =>
                    {
                        var tDrug = new Drug(oDrug);
                        var oDrugDetail = ScopedCacher.FindOrCreate<DrugDetailResponse>((oDrug.DrugID.ToString()),
                            key => Client.DrugDetailAsync(oDrug.DrugID.ToString()).ExecuteSynchronously());
                        var oDosages = oDrugDetail.Value.Dosages;
                        tDrug.Dosages = new Dosages(oDosages);
                        tDrugs.Add(tDrug);
                    }
                );

                var drugResponse = new Data.ApiModels.Rx.DrugResponse {Data = tDrugs.ToArray()};
                return drugResponse;
            });

            return tDrugResponse.Value;
        }

        public DrugDetailResponse DrugDetail(string ndcOrOrchestraId)
        {
            var cacheKey = Cache.CreateKey(typeof(DrugDetailResponse).Name, ndcOrOrchestraId);
            var drugDetailResponse = ScopedCacher.FindOrCreate<DrugDetailResponse>(cacheKey, key => Client.DrugDetailAsync(ndcOrOrchestraId).Result);
            return drugDetailResponse?.Value;
        }

        public DrugOption[] DrugAlternativeSearch(IEnumerable<DrugAlternativeSearchQuery> searchQuery)
        {
            var queryCacheKey = Cache.CreateKey(typeof(DrugOption[]).Name, searchQuery);
            var drugOptions = ScopedCacher.FindOrCreate<DrugOption[]>(queryCacheKey,
                key => Client.DrugAlternativeMultipleSearchAsync(searchQuery).ExecuteSynchronously());
            return drugOptions.Value;
        }
    }
}
