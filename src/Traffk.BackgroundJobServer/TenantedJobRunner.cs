using System;
using Hangfire;
using MimeKit;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using Serilog;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkGlobal;

namespace Traffk.BackgroundJobServer
{
    public class TenantedJobRunner : BaseJobRunner, ITenantJobs
    {
        private readonly TraffkTenantModelDbContext DB;
        private readonly CurrentTenantServices Current;
        private readonly ITableauAdminService TableauAdminService;
        private readonly ITableauVisualServices TableauVisualService;
        private readonly BlobStorageServices BlobStorageService;
        private readonly IBackgroundJobClient InnerBackgroundJobClient;

        public TenantedJobRunner(TraffkTenantModelDbContext db, 
            TraffkGlobalDbContext globalContext,
            JobRunnerProgram jobRunnerProgram,
            CurrentTenantServices current, 
            ITableauAdminService tableauAdminService,
            ILogger logger,
            ITableauVisualServices tableauVisualService,
            BlobStorageServices blobStorageService,
            IBackgroundJobClient innerBackgroundJobClient) : base(globalContext, 
                jobRunnerProgram, 
                logger)
        {
            DB = db;
            Current = current;
            TableauAdminService = tableauAdminService;
            TableauVisualService = tableauVisualService;
            BlobStorageService = blobStorageService;
            InnerBackgroundJobClient = innerBackgroundJobClient;
        }

        void ITenantJobs.ReconfigureFiscalYears(FiscalYearSettings settings)
        {
            var existingSettings = Current.Tenant.TenantSettings.FiscalYearSettings ?? new FiscalYearSettings();
            if (existingSettings.FiscalYear != settings.FiscalYear ||
                existingSettings.CalendarYear != settings.CalendarYear ||
                existingSettings.CalendarMonth != settings.CalendarMonth)
            {
                DB.FiscalYearsConfigureAsync(Current.TenantId, settings.FiscalYear, settings.CalendarYear, settings.CalendarMonth).ExecuteSynchronously();
                Current.Tenant.TenantSettings.FiscalYearSettings = settings;
                DB.SaveChanges();
                PostResult(Current.Tenant.TenantSettings.FiscalYearSettings);
            }

            Logger.Information("Completed ReconfigureFiscalYearsJob");
        }

        void ITenantJobs.CreateTableauTenant(CreateTableauTenantRequest request)
        {
            var siteInfo = TableauAdminService.CreateTableauTenant(request);
            Current.Tenant.TenantSettings.TableauTenantId = siteInfo.IdentifierForUrl;
            DB.SaveChanges();
        }

        void ITenantJobs.MigrateTableauDataset(TableauDataMigrationRequest request)
        {
            TableauAdminService.MigrateDataset(request);
        }

        void ITenantJobs.CreateTableauPdf(CreatePdfOptions options)
        {
            var downloadOptions = TableauVisualService.CreatePdfAsync(options).ExecuteSynchronously();
            PostResult(downloadOptions);
        }

        async void ITenantJobs.DownloadTableauPdfContinuationJobAsync()
        {
            string resultData = this.GetParentJobResultData();
            var downloadOptions = JsonConvert.DeserializeObject<DownloadPdfOptions>(resultData);
            var pdfBytes = await TableauVisualService.DownloadPdfAsync(downloadOptions);
            var dateTime = DateTime.UtcNow;
            var blob = await BlobStorageService.StoreFileAsync(
                true, 
                BlobStorageServices.Roots.User, 
                new BasicFormFile(pdfBytes)
                {
                    ContentDisposition = ContentDisposition.Attachment,
                    ContentType = MimeType.Application.Pdf,
                    FileName = $"{downloadOptions.WorkbookName}{MimeType.Application.Pdf.PrimaryFileExtension}",
                    Name = $"{downloadOptions.WorkbookName}{MimeType.Application.Pdf.PrimaryFileExtension}",
                },
                $"{downloadOptions.WorkbookName.RemoveSpecialCharacters()}-{dateTime.ToString().RemoveSpecialCharacters()}{MimeType.Application.Pdf.PrimaryFileExtension}");
            PostResult(blob);
        }

        void ITenantJobs.ScheduleTableauPdfDownload(CreatePdfOptions options)
        {
            var jobId = InnerBackgroundJobClient.Enqueue<ITenantJobs>(z => z.CreateTableauPdf(options));
            InnerBackgroundJobClient.ContinueWith<ITenantJobs>(jobId, y => y.DownloadTableauPdfContinuationJobAsync());
        }

        void ITenantJobs.Schedule()
        {
            Console.WriteLine("Recurring job");
        }
    }
}
