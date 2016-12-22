using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.PowerBiToys.Objects;
using System.Net.Http;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;

namespace RevolutionaryStuff.PowerBiToys
{
    public class PowerBiServices
    {
        private readonly IOptions<PowerBiEndpointOptions> EndpointOptions;
        private readonly IBearerGetter BearerGetter;

        public Task<string> GetBearer()
        {
            return BearerGetter.GetBearer();
        }

        public PowerBiServices(IOptions<PowerBiEndpointOptions> endpointOptions, IBearerGetter bearerGetter)
        {
            Requires.NonNull(endpointOptions, nameof(endpointOptions));
            Requires.NonNull(bearerGetter, nameof(bearerGetter));

            EndpointOptions = endpointOptions;
            BearerGetter = bearerGetter;
        }

        public async Task<TPowerBiGetResult> Get<TPowerBiGetResult>(Uri uri)
            where TPowerBiGetResult : PowerBiGetResultBase
        {
            var serializer = SerializationHelpers.GetJsonSerializer<TPowerBiGetResult>();

            var client = new HttpClient();
            var bearer = await GetBearer();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);
            var response = await client.GetAsync(uri);
            using (var st = await response.Content.ReadAsStreamAsync())
            {
                return (TPowerBiGetResult) serializer.ReadObject(st);
            }
        }

        public Task<PowerBiGetDashboardsResult> GetDashboards()
        {
            return Get<PowerBiGetDashboardsResult>(EndpointOptions.Value.GetDashboardsUrl());
        }

        public Task<PowerBiGetDatasetsResult> GetDatasets()
        {
            return Get<PowerBiGetDatasetsResult>(EndpointOptions.Value.GetDatasetsUrl());
        }

        public Task<PowerBiGetGroupsResult> GetGroups()
        {
            return Get<PowerBiGetGroupsResult>(EndpointOptions.Value.GetGroupsUrl());
        }

        public Task<PowerBiGetReportsResult> GetReports()
        {
            return Get<PowerBiGetReportsResult>(EndpointOptions.Value.GetReportsUrl());
        }

        public Task<PowerBiGetTilesResult> GetTiles(string dashboardId)
        {
            return Get<PowerBiGetTilesResult>(EndpointOptions.Value.GetTilesUrl(dashboardId));
        }
    }
}
