using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Tableau.VQL;

namespace Traffk.Tableau.REST
{
    public interface ITableauRestService
    {
        TableauServerSignIn Login { get; set; }
        //TODO: Remove {TableauServerUrls onlineUrls, string userName, string password} from the signature
        TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null);
        DownloadProjectsList DownloadProjectsList();
        DownloadViewsForSite DownloadViewsForSite();
        ICollection<SiteWorkbook> DownloadWorkbooksList();
        byte[] DownloadPreviewImageForView(string workbookId, string viewId);
        Task<UnderlyingDataTable> GetUnderlyingDataAsync(GetUnderlyingDataOptions options, string workbookName, string viewName);
        SiteinfoSite CreateSite(string tenantName, out string url);
        void AddUserToSite(string siteId, string userName);
        void RemoveUserFromSite(SiteUser userToRemove);
        ICollection<SiteWorkbook> DownloadWorkbooks(IEnumerable<SiteWorkbook> workbooksToDownload, string localSavePath,
            bool generateInfoFile);

        void UploadWorkbooks(string projectName, string datasourceUsername, string datasourcePassword,
            bool isEmbedded, string path);
        ICollection<SiteDatasource> DownloadDatasourceList();
        ICollection<SiteConnection> DownloadConnectionsForDatasource(string datasourceId);
        void DownloadDatasourceFiles(IEnumerable<SiteDatasource> datasources, string savePath);
        ICollection<SiteDatasource> UploadDatasourceFiles(string projectName, string datasourceUsername,
            string datasourcePassword, bool isEmbedded, string path);
        void UpdateDatasourceConnection(SiteDatasource datasourceToUpdate, SiteConnection connectionToUpdate,
            string newServerAddress);

        void CreateNewTableauTenant(CreateTableauTenantRequest request);
    }

    public class VisualId
    {
        [JsonProperty("worksheet")]
        public string Worksheet { get; set; }

        [JsonProperty("dashboard")]
        public string Dashboard { get; set; }

        [JsonProperty("flipboardZoneId")]
        public string FlipboardZoneId { get; set; }

        [JsonProperty("storyboard")]
        public string Storyboard { get; set; }

        [JsonProperty("storyPointId")]
        public string StoryPointId { get; set; }
    }

    public class GetUnderlyingDataOptions
    {
        [JsonProperty("ignoreAliases")]
        public bool IgnoreAliases { get; set; }

        [JsonProperty("ignoreSelection")]
        public bool IgnoreSelection { get; set; }

        [JsonProperty("includeAllColumns")]
        public bool IncludeAllColumns { get; set; }

        [JsonProperty("maxRows")]
        public int MaxRows { get; set; }

        [JsonProperty("visualId")]
        public VisualId VisualId { get; set; }

        /*
        Below are extra parameters I found from the .JS file from the function:
        https://traffk-my.sharepoint.com/personal/darren_traffk_com/Documents/tableau-2.1.0.js
        $addVisualIdToCommand: function WorksheetImpl$AddVisualIdToCommand(commandParameters)
         */
        [JsonProperty("worksheetName")]
        public string WorksheetName { get; set; }

        [JsonProperty("dashboardName")]
        public string DashboardName { get; set; }
    }
}