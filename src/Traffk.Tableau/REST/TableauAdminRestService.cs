using System.IO;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public class TableauAdminRestService : TableauRestService
    {
        private readonly TableauAdminCredentials TableauAdminCredentials;

        public TableauAdminRestService(IOptions<TableauSignInOptions> options,
            TableauAdminCredentials adminCredentials) : base(options, adminCredentials)
        {
            TableauAdminCredentials = adminCredentials;
        }

        public void MigrateDataset(TableauDataMigrationRequest request)
        {
            base.Options.UpdateForTenant(request.DestTableauTenantId);
            var destinationSiteSignInOptions = ConfigurationHelpers.CreateOptions(Options);
            var destinationRestService =
                new TableauRestService(destinationSiteSignInOptions, TableauAdminCredentials) as ITableauRestService;
            var destinationSiteInfo = destinationRestService.GetSiteInfo();
            
            var sourceRestService = this as ITableauRestService;

            //Download datasources from master site
            var dataSources = sourceRestService.DownloadDatasourceList();
            sourceRestService.DownloadDatasourceFiles(dataSources, request.TempFilePath);

            foreach (var datasource in Directory.GetFiles(request.TempFilePath + @"\Default"))
            {
                TableauFileEditor.UpdateDatasourceDatabaseName(datasource, request.DestDbName, request.TempFilePath);
            }

            //Upload datasources to new site
            var uploadedDataSources = destinationRestService.UploadDatasourceFiles("Default", request.DestDbUsername, request.DestDbPassword, true, request.TempFilePath);

            //Download workbooks from master site
            var workbooksToDownload = sourceRestService.DownloadWorkbooksList();
            sourceRestService.DownloadWorkbooks(workbooksToDownload, request.TempFilePath, false);

            foreach (var workbook in Directory.GetFiles(request.TempFilePath))
            {
                TableauFileEditor.UpdateWorkbookFileSiteReferences(workbook, destinationSiteInfo);
            }

            //Upload workbooks to new site
            destinationRestService.UploadWorkbooks("Default", request.DestDbUsername, request.DestDbPassword, true, request.TempFilePath);
        }
    }
}
