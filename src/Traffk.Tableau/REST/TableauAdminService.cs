using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public class TableauAdminService : TableauBaseService, ITableauAdminService
    {
        private readonly TableauAdminCredentials TableauAdminCredentials;

        public TableauAdminService(IOptions<TableauSignInOptions> options,
            TableauAdminCredentials adminCredentials) : base(options, adminCredentials)
        {
            TableauAdminCredentials = adminCredentials;
        }

        void ITableauAdminService.CreateTableauTenant(CreateTableauTenantRequest request)
        {
            var masterRestService = this as ITableauAdminService;

            //Create the site
            var newSite = CreateSite(request.TenantName);

            //Migrate the data
            var migrateDataRequest = new TableauDataMigrationRequest(request);
            masterRestService.MigrateDataset(migrateDataRequest);
        }

        void ITableauAdminService.MigrateDataset(TableauDataMigrationRequest request)
        {
            base.Options.UpdateForTenant(request.DestTableauTenantId);
            var destinationSiteSignInOptions = ConfigurationHelpers.CreateOptions(Options);
            var destinationAdminService = new TableauAdminService(destinationSiteSignInOptions, TableauAdminCredentials);
            var destinationSiteInfo = destinationAdminService.GetSiteInfo();
            
            var sourceAdminService = this; //not normal convention, but using this to make it clearer to future devs

            //Download datasources from master site
            var dataSources = sourceAdminService.DownloadDatasourceList();
            sourceAdminService.DownloadDatasourceFiles(dataSources, request.TempFilePath);

            foreach (var datasource in Directory.GetFiles(request.TempFilePath + @"\Default"))
            {
                TableauFileEditor.UpdateDatasourceDatabaseName(datasource, request.DestDbName, request.TempFilePath);
            }

            //Upload datasources to new site
            var uploadedDataSources = destinationAdminService.UploadDatasourceFiles("Default", request.DestDbUsername, request.DestDbPassword, true, request.TempFilePath);

            //Download workbooks from master site
            var workbooksToDownload = sourceAdminService.DownloadWorkbookList();
            sourceAdminService.DownloadWorkbookFiles(workbooksToDownload, request.TempFilePath, false);

            foreach (var workbook in Directory.GetFiles(request.TempFilePath))
            {
                TableauFileEditor.UpdateWorkbookFileSiteReferences(workbook, destinationSiteInfo);
            }

            //Upload workbooks to new site
            destinationAdminService.UploadWorkbooks("Default", request.DestDbUsername, request.DestDbPassword, true, request.TempFilePath);
        }

        void ITableauAdminService.AddUserToSite(string siteId, string userName)
        {
            var addUserToSite = new AddUserToSite(Urls, Login);
            addUserToSite.ExecuteRequest(siteId, userName);
        }

        void ITableauAdminService.RemoveUserFromSite(SiteUser userToRemove)
        {
            var removeUserRequest = new RemoveUserFromSite(Urls, Login);
            removeUserRequest.ExecuteRequest(userToRemove);
        }

        protected SiteInfo CreateSite(string tenantName)
        {
            var addSite = new CreateSite(Urls, Login);
            var siteInfo = addSite.ExecuteRequest(tenantName);
            return siteInfo;
        }

        protected SiteInfo GetSiteInfo()
        {
            var getSiteInfoRequest = new DownloadSiteInfo(Urls, Login);
            return getSiteInfoRequest.ExecuteRequest();
        }

        protected ICollection<SiteWorkbook> DownloadWorkbookFiles(IEnumerable<SiteWorkbook> workbooksToDownload,
            string localSavePath,
            bool generateInfoFile)
        {
            var downloadWorkbooksRequest = new DownloadWorkbooks(Urls, Login, workbooksToDownload, localSavePath, generateInfoFile);
            var downloadedWorkbooks = downloadWorkbooksRequest.ExecuteRequest();
            return downloadedWorkbooks;
        }

        protected void UploadWorkbooks(string projectName, string datasourceUsername, string datasourcePassword, bool isEmbedded, string path)
        {
            var credentialManager = new CredentialManager();
            foreach (var thisFilePath in Directory.GetFiles(path))
            {
                credentialManager.AddWorkbookCredential(Path.GetFileName(thisFilePath), projectName, datasourceUsername, datasourcePassword, isEmbedded);
            }
            var uploadWorkbooksRequest = new UploadWorkbooks(Urls, Login, credentialManager, path);
            uploadWorkbooksRequest.ExecuteRequest();
        }

        protected ICollection<SiteDatasource> DownloadDatasourceList()
        {
            var downloadDataSourceListRequest = new DownloadDatasourcesList(Urls, Login);
            downloadDataSourceListRequest.ExecuteRequest();
            return downloadDataSourceListRequest.Datasources;
        }

        protected ICollection<SiteConnection> DownloadConnectionsForDatasource(string datasourceId)
        {
            var downloadConnectionRequest = new DownloadDatasourceConnections(Urls, Login, datasourceId);
            downloadConnectionRequest.ExecuteRequest();
            return downloadConnectionRequest.Connections;
        }

        protected void DownloadDatasourceFiles(IEnumerable<SiteDatasource> datasources, string savePath)
        {
            var projectRequest = new DownloadProjectsList(Urls, Login);
            projectRequest.ExecuteRequest();
            var downloadDatasources = new DownloadDatasources(Urls, Login, datasources, savePath, projectRequest, false, new KeyedLookup<SiteUser>(new List<SiteUser>()));
            downloadDatasources.ExecuteRequest();
        }

        protected ICollection<SiteDatasource> UploadDatasourceFiles(string projectName, string datasourceUsername, string datasourcePassword, bool isEmbedded, string path)
        {
            var credentialManager = new CredentialManager();
            foreach (var thisFilePath in Directory.GetFiles(path))
            {
                credentialManager.AddDatasourceCredential(Path.GetFileName(thisFilePath), projectName, datasourceUsername, datasourcePassword, isEmbedded);
            }

            var uploadRequest = new UploadDatasources(Urls, Login, credentialManager, path, false, new List<SiteUser>());
            uploadRequest.ExecuteRequest();
            return uploadRequest.UploadeDatasources;
        }

        protected ICollection<SiteWorkbook> DownloadWorkbookList() => base.DownloadWorkbooksList();
    }
}
