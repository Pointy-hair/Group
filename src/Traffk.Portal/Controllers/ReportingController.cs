using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TraffkPortal.Services;
using Microsoft.AspNetCore.Authorization;
using TraffkPortal.Permissions;
using RevolutionaryStuff.Core.Collections;
using Traffk.Bal.Permissions;
using Traffk.Bal.Data.Rdb;
using System.Linq;
using System.Text.RegularExpressions;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;
using TraffkPortal.Models.ReportingModels;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.BasicReporting)]
    public class ReportingController : BasePageController
    {
        public const string Name = "Reporting";
        public const VisualContext ReportVisualContext = VisualContext.Tenant;

        public IReportVisualService ReportVisualService { get; }

        public static class ActionNames
        {
            public const string ShowReport = "ShowReport";
            public const string Index = "Index";
            public const string Report = "Report";
        }

        public ReportingController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory,
            ICacher cacher,
            IReportVisualService reportVisualService
        )
            : base(AspHelpers.MainNavigationPageKeys.Reporting, db, current, loggerFactory, cacher)
        {
            ReportVisualService = reportVisualService;
            AttachLogContextProperty(typeof(EventType).Name, EventType.LoggingEventTypes.Report.ToString());
        }

        [Route("/Reporting")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = ReportVisualService.GetReportFolderTreeRoot(ReportVisualContext);
            return View(root);
        }

        [SetTableauTrustedTicket]
        [Route("/Reporting/Report/{id}/{anchorName}")]
        [ActionName(ActionNames.Report)]
        public IActionResult Report(string id, string anchorName)
        {
            var reportVisual = ReportVisualService.GetReportVisual(ReportVisualContext, Parse.ParseInt32(id));
            if (reportVisual == null)
            {
                return RedirectToAction(ActionNames.Index);
            }
            Log.Information(reportVisual.Id.ToString());
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
    }
}