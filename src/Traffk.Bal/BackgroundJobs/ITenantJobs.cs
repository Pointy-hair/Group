using Hangfire;
using Traffk.Bal.Settings;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Bal.BackgroundJobs
{
    public interface ITenantJobs
    {
        void ReconfigureFiscalYears(FiscalYearSettings settings);

        void CreateTableauTenant(CreateTableauTenantRequest request);

        void MigrateTableauDataset(TableauDataMigrationRequest request);

        [AutomaticRetry(Attempts = 50)]
        void CreateTableauPdf(CreatePdfOptions options);

        void DownloadTableauPdfContinuationJobAsync();

        void ScheduleTableauPdfDownload(CreatePdfOptions options);

        void Schedule();
    }
}
