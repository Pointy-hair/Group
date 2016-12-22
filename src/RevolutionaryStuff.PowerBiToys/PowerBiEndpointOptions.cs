using System;

namespace RevolutionaryStuff.PowerBiToys
{
    public class PowerBiEndpointOptions
    {
        public string RestApiResourceUrl { get; set; } = "https://analysis.windows.net/powerbi/api";
        public string RestApiBaseEndpoint { get; set; } = "https://api.powerbi.com/v1.0/myorg/";

        public Uri GetDashboardsUrl()
        {
            return new Uri($"{RestApiBaseEndpoint}Dashboards");
        }

        public Uri GetDatasetsUrl()
        {
            return new Uri($"{RestApiBaseEndpoint}datasets");
        }
        public Uri GetGroupsUrl()
        {
            return new Uri($"{RestApiBaseEndpoint}Groups");
        }
        public Uri GetReportsUrl()
        {
            return new Uri($"{RestApiBaseEndpoint}Reports");
        }
        public Uri GetTilesUrl(string dashboardId)
        {
            return new Uri($"{RestApiBaseEndpoint}Dashboards/{dashboardId}/Tiles");
        }
    }
}
