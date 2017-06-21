using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Traffk.Bal.Data.ApiModels.Rx;
using Traffk.Bal.Services;
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
            if (tokenResponseCacheEntry.Value == null)
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

        public Data.ApiModels.Rx.DrugResponse DrugSearch(string query)
        {
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(DrugResponse).Name, query, "traffk");
            var tDrugResponse = Cache.FindOrCreate<Data.ApiModels.Rx.DrugResponse>(cacheKey, outerKey =>
            {
                var searchCacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(DrugResponse).Name, query);
                var oDrugResponse = Cache.FindOrCreate<DrugResponse>(searchCacheKey,
                    key => Client.DrugSearchAsync(query).Result);
                var oDrugs = oDrugResponse.Value.Drugs.ToList();

                var tDrugs = new List<Drug>();

                Parallel.ForEach(oDrugs, (oDrug) =>
                    {
                        var tDrug = new Drug(oDrug);
                        var oDrugDetail = Cache.FindOrCreate<DrugDetailResponse>((oDrug.DrugID.ToString()),
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
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(DrugDetailResponse).Name, ndcOrOrchestraId);
            var drugDetailResponse = Cache.FindOrCreate<DrugDetailResponse>(cacheKey, key => Client.DrugDetailAsync(ndcOrOrchestraId).Result);
            return drugDetailResponse?.Value;
        }

        public DrugAlternativeResponse DrugAlternativeSingleSearch(DrugAlternativeSearchQuery searchQuery)
        {
            var drugAlternativeResponse = Client.DrugAlternativeSingleSearchAsync(searchQuery).ExecuteSynchronously();
            return drugAlternativeResponse;
        }

        //May change parameter to TraffkID or NDC ID and do a lookup to convert to OrchestraID
        public DrugDetailResponse DrugDosageAlternatives(string orchestraDrugId)
        {
            var cacheKey = RevolutionaryStuff.Core.Caching.Cache.CreateKey(typeof(DrugDetailResponse).Name, orchestraDrugId);
            var drugDetailResponse = Cache.FindOrCreate<DrugDetailResponse>(cacheKey, key => Client.DrugDosageAlternativesAsync(orchestraDrugId).Result);
            return drugDetailResponse?.Value;
        }
    }
}
