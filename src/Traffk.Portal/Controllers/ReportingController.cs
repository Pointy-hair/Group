using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using Hangfire;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Permissions;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Models.ReportingModels;
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
        protected readonly BlobStorageServices BlobStorageService;

        public static class ActionNames
        {
            public const string ShowReport = "ShowReport";
            public const string Index = "Index";
            public const string Report = "Report";
        }

        
        public ReportingController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILogger logger,
            ICacher cacher,
            IReportVisualService reportVisualService,
            IBackgroundJobClient backgrounder,
            BlobStorageServices blobStorageService
        )
            : base(AspHelpers.MainNavigationPageKeys.Reporting, db, current, logger, cacher)
        {
            ReportVisualService = reportVisualService;
            Backgrounder = backgrounder;
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
            var jobId = Backgrounder.Enqueue<ITenantJobs>(z => z.CreateTableauPdf(createPdfOptions));
            Backgrounder.ContinueWith<ITenantJobs>(jobId, y => y.DownloadTableauPdfContinuationJob(int.Parse(jobId)));

            Logger.Information("{@EventType} {@ReportId}", EventType.LoggingEventTypes.DownloadedReport.ToString(), reportVisual.Id.ToString());

            return NoContent(); //Placeholder - will redirect to a reportIndex page
        }

        //[Route("/Reporting/Downloads")]
        //public IActionResult Downloads()
        //{
        //    var blobs = BlobStorageService.GetFileInfosAsync(true, BlobStorageServices.Roots.User, )
        //}
    }
}