using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Traffk.Orchestra.Models;
using Traffk.Utility;

namespace Traffk.Orchestra
{
    public class OrchestraRxApiClient
    {
        private readonly string ApiVersion = "v1";
        private readonly string AcceptHeader = "application/json; charset=utf-8";
        private string ApiToolsUrlPortion => $"/APITools/{ApiVersion}";
        private string DrugCompareUrlPortion => $"/API/{ApiVersion}";
        private readonly OrchestraRxConfig Config;

        private readonly IHttpClientFactory HttpClientFactory;
        private OrchestraRxTokenResponse Token;
        private HttpClient HttpClientWithHeaders;

        public static class OrchestraEndpoints
        {
            public const string Auth = @"/auth/token?format=json";
            public const string PharmacySearch = @"/Pharmacies/Search";
            public const string DrugSearch = @"/Drugs/Search";
            public const string DrugDetail = @"/Drugs/{DrugID}";
            public const string DrugAlternatives = @"Drugs/Alternatives";
        }

        public OrchestraRxApiClient(IOptions<OrchestraRxConfig> configOptions, 
            IHttpClientFactory httpClientFactory)
        {
            Config = configOptions.Value;
            HttpClientFactory = httpClientFactory;
        }

        public void SetToken(OrchestraRxTokenResponse token)
        {
            Token = token;
            HttpClientWithHeaders = CreateHttpClientWithHeaders();
        }

        public OrchestraRxTokenResponse Authenticate()
        {
            var httpClient = HttpClientFactory.Create();

            var plainTextBytes = Encoding.UTF8.GetBytes($"{Config.Key}:{Config.Secret}");
            var base64KeySecret = Convert.ToBase64String(plainTextBytes);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {base64KeySecret}");

            var tokenRequestFormContent = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
            };

            var content = new FormUrlEncodedContent(tokenRequestFormContent);

            var response = httpClient.PostAsync(Config.AuthUrl, content).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var tokenResponse = JsonConvert.DeserializeObject<OrchestraRxTokenResponse>(json);

            return tokenResponse;
        }

        public async Task<DrugToReplace[]> DrugAlternativeMultipleSearchAsync(
            IEnumerable<DrugAlternativeSearchQuery> searchQueries)
        {
            var drugFilter = "";
            foreach (var query in searchQueries)
            {
                drugFilter += $"{query.NDC}|{query.DaysOfSupply}|{query.MetricQuantity},";
            }
            var apiRoute = $"{DrugCompareUrlPortion}/Drugs/Alternatives?drugFilters={drugFilter}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<DrugToReplace[]>(json);
        }

        public async Task<HasAlternativesResponse> HasAlternatives(string ndcReference)
        {
            var apiRoute = $"{DrugCompareUrlPortion}/Drugs/{ndcReference}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<HasAlternativesResponse>(json);
        }

        public async Task<DrugToReplace[]> PlanDrugAlternativeMultipleSearchAsync(
            string planId,
            IEnumerable<DrugAlternativeSearchQuery> searchQueries, 
            IEnumerable<DrugAlternativePharmacyFilterQuery> pharmacyFilterQueries)
        {
            var drugFilter = "";
            foreach (var query in searchQueries)
            {
                drugFilter += $"{query.NDC}|{query.DaysOfSupply}|{query.MetricQuantity},";
            }

            var pharmacyFilter = "";
            foreach (var query in pharmacyFilterQueries)
            {
                pharmacyFilter += $"{query.PharmacyID}|{query.PharmacyIDType}|{query.isMailOrder}";
            }
            var apiRoute = $"{DrugCompareUrlPortion}/Plans/{planId}/Drugs/Compare?drugFilters={drugFilter}&pharmacyFilters={pharmacyFilter}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            var drugOptions = JsonConvert.DeserializeObject<DrugToReplace[]>(json);
            return drugOptions;
        }

        public async Task<OrchestraDrugAlternativeResponse> PlanDrugAlternativeSingleSearchAsync(
        string planId,
        DrugAlternativeSearchQuery searchQuery,
        IEnumerable<DrugAlternativePharmacyFilterQuery> pharmacyFilterQueries)
        {
            var pharmacyFilter = "";
            foreach (var query in pharmacyFilterQueries)
            {
                pharmacyFilter += $"{query.PharmacyID}|{query.PharmacyIDType}|{query.isMailOrder}";
            }
            var apiRoute = $"{DrugCompareUrlPortion}/Plans/{planId}/Drugs/{searchQuery.NDC}/Compare?DaysOfSupply={searchQuery.DaysOfSupply}&MetricQuantity={searchQuery.MetricQuantity}&pharmacyFilters={pharmacyFilter}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<OrchestraDrugAlternativeResponse>(json);
        }
        
        public async Task<County> CountySearchAsync(string zip)
        {
            var apiRoute = $"{ApiToolsUrlPortion}/Counties/{zip}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<County>(json);
        }

        public async Task<OrchestraDosage> DrugDosageDetailAsync(string orchestraDrugId, string orchestraDosageId)
        {
            var apiRoute = $"{ApiToolsUrlPortion}/Drugs/{orchestraDrugId}/Dosages/{orchestraDosageId}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<OrchestraDosage>(json);
        }

        public async Task<DrugResponse> DrugSearchAsync(string query)
        {
            var apiRoute = $"{ApiToolsUrlPortion}/Drugs/Search?q={query}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url); ;

            //JsonConvert can't serialize directly to DrugResponse
            var drugArray = JsonConvert.DeserializeObject<OrchestraDrug[]>(json);
            var drugResponse = new DrugResponse { Drugs = drugArray };
            return drugResponse;
        }

        public async Task<DrugDetailResponse> DrugDetailAsync(string ndcOrOrchestraId)
        {
            var apiRoute = $"{ApiToolsUrlPortion}/Drugs/{ndcOrOrchestraId}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<DrugDetailResponse>(json);
        }

        public async Task<PharmacyResponse> PharmacySearchAsync(string zip, int radius)
        {
            var apiRoute = $"{ApiToolsUrlPortion}/Pharmacies/Search?zip={zip}&radius={radius}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<PharmacyResponse>(json);
        }

        public async Task<DrugToReplace> DrugAlternativeSingleSearchAsync(
            DrugAlternativeSearchQuery searchQuery)
        {
            var apiRoute = $"{DrugCompareUrlPortion}/Drugs/{searchQuery.NDC}/Alternatives?DaysOfSupply={searchQuery.DaysOfSupply}&MetricQuantity={searchQuery.MetricQuantity}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<DrugToReplace>(json);
        }

        //DrugDosageAlternativesAsync - Not documented but returns a result
        public async Task<DrugDetailResponse> DrugDosageAlternativesAsync(string orchestraDrugId)
        {
            var apiRoute = $"{ApiToolsUrlPortion}/Drugs/Alternatives?id={orchestraDrugId}";
            VerifyToken();
            var url = Config.BaseUrl + apiRoute;
            var json = await HttpClientWithHeaders.GetJsonStringAsync(url);
            return JsonConvert.DeserializeObject<DrugDetailResponse>(json);
        }


        private void VerifyToken()
        {
            if (Token == null)
            {
                Token = Authenticate();
            }
        }

        private HttpClient CreateHttpClientWithHeaders()
        {
            var httpClient = HttpClientFactory.Create(new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            });
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Token.access_token}");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", $"{AcceptHeader}");

            return httpClient;
        }
    }

    public static class OrchestraHttpHelpers
    {
        public static async Task<string> GetJsonStringAsync(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }

}
