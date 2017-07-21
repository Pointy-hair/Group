using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Portal.Models.ReportingModels;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Permissions;
using TraffkPortal.Services;
using ILogger = Serilog.ILogger;

namespace Traffk.Portal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.BasicReporting)]
    public class ReportingController : BasePageController
    {
        public const string Name = "Reporting";
        public const VisualContext ReportVisualContext = VisualContext.Tenant;

        protected readonly IReportVisualService ReportVisualService;
        protected readonly IBackgroundJobClient Backgrounder;
        protected readonly ITraffkRecurringJobManager RecurringJobManager;
        protected readonly BlobStorageServices BlobStorageService;

        public enum PageKeys
        {
            ScheduledReportDetails,
            ScheduledReportHistory
        }

        public static class ActionNames
        {
            public const string ShowReport = "ShowReport";
            public const string Index = "Index";
            public const string Report = "Report";
            public const string DownloadedReports = "Downloads";
            public const string ScheduledReports = "ScheduledReportIndex";
            public const string ScheduleReportSave = "ScheduleReportSave";
            public const string ScheduledReportDetail = "ScheduledReportDetail";
        }

        public static class ViewNames
        {
            public const string DownloadList = "Downloads";
        }

        public ReportingController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger,
            ICacher cacher,
            IReportVisualService reportVisualService,
            IBackgroundJobClient backgrounder,
            ITraffkRecurringJobManager recurringJobManager,
            BlobStorageServices blobStorageService
        )
            : base(AspHelpers.MainNavigationPageKeys.Reporting, db, current, logger, cacher)
        {
            ReportVisualService = reportVisualService;
            Backgrounder = backgrounder;
            RecurringJobManager = recurringJobManager;
        }

        [Route("/Reporting")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var reportSearchCriteria = new ReportSearchCriteria
            {
                VisualContext = ReportVisualContext
            };
            var root = ReportVisualService.GetReportFolderTreeRoot(reportSearchCriteria);
            return View(root);
        }

        [SetTableauTrustedTicket]
        [Route("/Reporting/Report/{id}/{anchorName}")]
        [ActionName(ActionNames.Report)]
        public IActionResult Report(string id, string anchorName)
        {
            var reportSearchCriteria = new ReportSearchCriteria
            {
                VisualContext = ReportVisualContext,
                ReportId = Parse.ParseInt32(id)
            };
            var reportVisual = ReportVisualService.GetReportVisual(reportSearchCriteria);
            if (reportVisual == null)
            {
                return RedirectToAction(ActionNames.Index);
            }

            Logger.Information("{@EventType} {@ReportId}", EventType.LoggingEventTypes.ViewedReport.ToString(), reportVisual.Id.ToString());

            var tableauReportViewModel = new TableauReportViewModel(reportVisual);
            return View(tableauReportViewModel);
        }

        private static readonly DateTime StartedAtUtc = DateTime.UtcNow;

        [Route("/Reporting/PreviewImage/{workbookId}/{viewId}")]
        public IActionResult PreviewImage(string workbookId, string viewId)
        {
            var etag = $"\"{TenantId}.{workbookId}.{viewId}\"";
            var stringValues = this.Request.Headers.FindOrDefault(WebHelpers.HeaderStrings.IfNoneMatch);
            if (stringValues.Count == 1 && stringValues[0] == etag)
            {
                return StatusCode(System.Net.HttpStatusCode.NotModified);
            }
            stringValues = this.Request.Headers.FindOrDefault(WebHelpers.HeaderStrings.IfModifiedSince);
            if (stringValues.Count == 1 && stringValues[0] == etag)
            {
                DateTime dt;
                if (DateTime.TryParse(stringValues[0], out dt) && dt >= StartedAtUtc)
                {
                    return StatusCode(System.Net.HttpStatusCode.NotModified);
                }
            }
            Response.Headers.Add(WebHelpers.HeaderStrings.ETag, etag);
            if (Response.Headers.ContainsKey(WebHelpers.HeaderStrings.CacheControl))
            {
                Response.Headers.Remove(WebHelpers.HeaderStrings.CacheControl);
            }
            Response.Headers.Add(WebHelpers.HeaderStrings.LastModified, StartedAtUtc.ToRfc7231());
            Response.Headers.Add(WebHelpers.HeaderStrings.CacheControl, "public");
            return Cacher.FindOrCreate(Cache.CreateKey(workbookId, viewId), key =>
            {                
                try
                {
                    byte[] previewImagesBytes = ReportVisualService.DownloadPreviewImageForTableauVisual(workbookId, viewId);
                    if (previewImagesBytes != null)
                    {
                        var fileResult = new FileContentResult(previewImagesBytes, "image/png");
                        return new CacheEntry<FileContentResult>(fileResult);
                    }
                }
                catch (Exception) { }
                return new CacheEntry<FileContentResult>(null);
            }).Value;
        }

        [Route("/Reporting/Report/Download/{id}/{anchorName}")]
        public IActionResult DownloadReport(string id, string anchorName)
        {
            var reportSearchCriteria = new ReportSearchCriteria
            {
                VisualContext = ReportVisualContext,
                ReportId = Parse.ParseInt32(id)
            };
            var reportVisual = ReportVisualService.GetReportVisual(reportSearchCriteria);
            if (reportVisual == null)
            {
                return RedirectToAction(ActionNames.Index);
            }

            var tableauReportViewModel = new TableauReportViewModel(reportVisual);

            var createPdfOptions = new CreatePdfOptions(tableauReportViewModel.WorkbookName, tableauReportViewModel.ViewName, tableauReportViewModel.WorksheetName);
            QueueReportDownload(createPdfOptions);

            Logger.Information("{@EventType} {@ReportId}", EventType.LoggingEventTypes.DownloadedReport.ToString(), reportVisual.Id.ToString());

            return NoContent(); //Placeholder - will redirect to a reportIndex page
        }

        [Route("/Reporting/Report/Downloads")]
        [ActionName(ActionNames.DownloadedReports)]
        public IActionResult DownloadedReportIndex(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var currentUserContactId = Current.User.ContactId;
            var items = Rdb.Job.Where(j => j.TenantId == TenantId 
                && j.ContactId == currentUserContactId
                && j.HangfireJobDetails.Method.Contains("Download",true));

            items = ApplyBrowse(items, sortCol ?? nameof(Job.CreatedAt),
                sortDir ?? AspHelpers.SortDirDescending, page, pageSize);

            return View(ViewNames.DownloadList, items);
        }

        [HttpGet]
        [Route("/Reporting/Report/Schedule/{id}/{anchorName}")]
        public IActionResult CreateScheduledReport(string id, string anchorName)
        {
            try
            {
                var reportSearchCriteria = new ReportSearchCriteria
                {
                    VisualContext = ReportVisualContext,
                    ReportId = Parse.ParseInt32(id)
                };
                var reportVisual = ReportVisualService.GetReportVisual(reportSearchCriteria);
                if (reportVisual == null)
                {
                    return RedirectToAction(ActionNames.Index);
                }
                var tableauReportViewModel = new TableauReportViewModel(reportVisual);
                var scheduleReportViewModel = new CreateScheduledReportViewModel(tableauReportViewModel,
                    new RecurrenceSettings());

                return View(scheduleReportViewModel);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(ActionNames.ScheduleReportSave)]
        public IActionResult CreateScheduledReport(
            [FromForm]CreateScheduledReportViewModel createScheduledReportViewModel)
        {
            try
            {
                var tableauReportViewModel = createScheduledReportViewModel.TableauReportViewModel;
                var createPdfOptions = new CreatePdfOptions(tableauReportViewModel.WorkbookName, tableauReportViewModel.ViewName, tableauReportViewModel.WorksheetName);
                var cronString = createScheduledReportViewModel.RecurrenceSettings.ConvertToCronString();
                var recurringJobId = RecurringJobManager.Add(Hangfire.Common.Job.FromExpression<ITenantJobs>(x => x.ScheduleTableauPdfDownload(createPdfOptions)), cronString);

                var campaignName = "Scheduled Report: " + tableauReportViewModel.ViewName;
                
                var communication = new Communication
                {
                    
                    CampaignName = campaignName.ToTitleFriendlyString(),
                    CommunicationTitle = tableauReportViewModel.ViewName.ToTitleFriendlyString(),
                    CommunicationSettings = new CommunicationSettings
                    {
                        Recurrence = createScheduledReportViewModel.RecurrenceSettings,
                        RecurringJobId = recurringJobId,
                        ReportId = createScheduledReportViewModel.TableauReportViewModel.Id.ToString(),
                        ReportName = tableauReportViewModel.ViewName
                    }
                };
                Rdb.Communications.Add(communication);
                Rdb.SaveChanges();
                return RedirectToAction(ActionNames.ScheduledReports);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Route("/Reporting/ScheduledReports")]
        [ActionName(ActionNames.ScheduledReports)]
        public IActionResult ScheduledReportIndex()
        {
            var userRecurringJobs = RecurringJobManager.GetUserRecurringJobs();
            var viewModels = new List<ScheduledReportViewModel>();
            foreach (var job in userRecurringJobs)
            {
                var communication = Rdb.Communications.FirstOrDefault(x => x.CommunicationSettings.RecurringJobId == job.Id);
                if (communication != null)
                {
                    var viewModel = new ScheduledReportViewModel
                    {
                        Communication = communication,
                        RecurringJob = job
                    };
                    //Throwing errors when trying to create title friendly string in view

                    viewModel.Communication.CommunicationSettings.ReportName = viewModel.Communication
                        .CommunicationSettings.ReportName.ToTitleFriendlyString();

                    viewModels.Add(viewModel);
                }
            }
            return View(viewModels);
        }

        [Route("/Reporting/ScheduledReports/{id}")]
        public IActionResult ScheduledReportDetail(int id)
        {
            var communication = Rdb.Communications.FirstOrDefault(x => x.CommunicationId == id);
            var recurringJob = RecurringJobManager.GetRecurringJobById(communication.CommunicationSettings.RecurringJobId);
            var viewModel = new ScheduledReportViewModel
            {
                Communication = communication,
                RecurringJob = recurringJob
            };

            SetHeroLayoutViewData(communication.CommunicationId, communication.CampaignName, PageKeys.ScheduledReportDetails);

            return View(viewModel);
        }

        [Route("/Reporting/ScheduledReports/{id}/History")]
        public IActionResult ScheduledReportHistory(int id)
        {
            var communication = Rdb.Communications.FirstOrDefault(x => x.CommunicationId == id);
            var pastRuns = Rdb.Job.Where(x => x.RecurringJobId == communication.CommunicationSettings.RecurringJobId);
            var viewModel = new ScheduledReportHistoryViewModel
            {
                Communication = communication,
                Jobs = pastRuns
            };
            SetHeroLayoutViewData(communication.CommunicationId, communication.CampaignName, PageKeys.ScheduledReportHistory);

            return View(viewModel);
        }

        private void QueueReportDownload(CreatePdfOptions options)
        {
            var jobId = Backgrounder.Enqueue<ITenantJobs>(z => z.CreateTableauPdf(options));
            Backgrounder.ContinueWith<ITenantJobs>(jobId, y => y.DownloadTableauPdfContinuationJobAsync());
        }
    }
}