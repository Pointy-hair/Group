using System;
using System.Threading;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using Serilog;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.BackgroundJobServer
{
    public class TenantedJobRunner : BaseJobRunner, ITenantJobs
    {
        private readonly TraffkRdbContext DB;
        private readonly CurrentTenantServices Current;
        private readonly ITableauAdminService TableauAdminService;
        private readonly ITableauVisualServices TableauVisualService;
        private readonly BlobStorageServices BlobStorageService;

        public TenantedJobRunner(TraffkRdbContext db, 
            TraffkGlobalsContext globalContext,
            JobRunnerProgram jobRunnerProgram,
            CurrentTenantServices current, 
            ITableauAdminService tableauAdminService,
            ILogger logger,
            ITableauVisualServices tableauVisualService,
            BlobStorageServices blobStorageService) : base(globalContext, 
                jobRunnerProgram, 
                logger)
        {
            DB = db;
            Current = current;
            TableauAdminService = tableauAdminService;
            TableauVisualService = tableauVisualService;
            BlobStorageService = blobStorageService;
        }

        void ITenantJobs.ReconfigureFiscalYears(FiscalYearSettings settings)
        {
            var existingSettings = Current.Tenant.TenantSettings.FiscalYearSettings ?? new FiscalYearSettings();
            if (existingSettings.FiscalYear != settings.FiscalYear ||
                existingSettings.CalendarYear != settings.CalendarYear ||
                existingSettings.CalendarMonth != settings.CalendarMonth)
            {
                //DB.FiscalYearsConfigureAsync(Current.TenantId, settings.FiscalYear, settings.CalendarYear, settings.CalendarMonth).ExecuteSynchronously();
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

        async void ITenantJobs.DownloadTableauPdfContinuationJob()
        {
            var awaitingState = await GlobalContext.JobStates.FirstAsync(j => j.JobId == JobId && j.Name == AwaitingState.StateName);

            if (awaitingState == null)
            {
                throw new Exception("No parent job found.");
            }

            var jobId = awaitingState.JobStateDetails.ParentId;

            const string pdfFileExtension = ".pdf";
            string resultData = null;
            do
            {
                var precedingJob = await GlobalContext.Job.AsNoTracking().FirstAsync(x => x.Id == jobId);
                resultData = precedingJob.ResultData;
                if (resultData == null)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            } while (resultData == null);

            var downloadOptions = JsonConvert.DeserializeObject<DownloadPdfOptions>(resultData);
            var pdfBytes = await TableauVisualService.DownloadPdfAsync(downloadOptions);
            var blob = BlobStorageService.StoreFileAsync(
                true, 
                BlobStorageServices.Roots.User, 
                new BasicFormFile(pdfBytes)
                {
                    ContentType = MimeType.Application.Pdf,
                    FileName = $"{downloadOptions.WorkbookName}{MimeType.Application.Pdf.PrimaryFileExtension}",
                    Name = $"{downloadOptions.WorkbookName}{MimeType.Application.Pdf.PrimaryFileExtension}",
                },
                $"{downloadOptions.WorkbookName.RemoveSpecialCharacters()}{pdfFileExtension}");
            PostResult(blob);
        }

        void ITenantJobs.Schedule()
        {
            Console.WriteLine("Recurring job");
        }
    }
}
