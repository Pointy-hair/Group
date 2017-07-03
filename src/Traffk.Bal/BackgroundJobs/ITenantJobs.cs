using Hangfire;
using Traffk.Bal.Settings;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Bal.BackgroundJobs
{
    public interface ITenantJobs
    {
        void ReconfigureFiscalYears(FiscalYearSettings settings);

        [Queue(BackgroundJobHelpers.QueueNames.TableauQueue)]
        void CreateTableauTenant(CreateTableauTenantRequest request);

        [Queue(BackgroundJobHelpers.QueueNames.TableauQueue)]
        void MigrateTableauDataset(TableauDataMigrationRequest request);

        [Queue(BackgroundJobHelpers.QueueNames.TableauQueue)]
        [AutomaticRetry(Attempts = 50)]
        void CreateTableauPdf(CreatePdfOptions options);

        [Queue(BackgroundJobHelpers.QueueNames.TableauQueue)]
        void DownloadTableauPdfContinuationJobAsync();

        [Queue(BackgroundJobHelpers.QueueNames.TableauQueue)]
        void ScheduleTableauPdfDownload(CreatePdfOptions options);

        void Schedule();
    }
}
