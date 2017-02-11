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
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.BasicReporting)]
    public class ReportingController : BasePageController
    {
        public const string Name = "Reporting";

        public ITableauRestService TableauRestService { get; set; }

        public static string CreateAnchorName(SiteViewResource view) => NameHelpers.GetName(view)?.Trim()?.ToUpperCamelCase()?.RemoveSpecialCharacters() ?? "";

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
            ITableauRestService tableauRestService
        )
            : base(AspHelpers.MainNavigationPageKeys.Reporting, db, current, loggerFactory, cacher)
        {
            TableauRestService = tableauRestService;
            AttachLogContextProperty("EventType", LoggingEventTypes.Report.ToString());
        }

        private TreeNode<SiteViewResource> GetReportFolderTreeRoot()
        {
            return Cacher.FindOrCreate("root", key=>{
                var root = new TreeNode<SiteViewResource>(new SiteViewFolderResource("Root"));
                var views = TableauRestService.DownloadViewsForSite().Views;

                if (views.Count() > 0)
                {
                    views = views.OrderBy(r => r.Name);
                    var workbookFolders = GetWorkbookFolders();

                    foreach (var view in views)
                    {
                        var workbookName = view.WorkbookName;
                        var workbookId = view.WorkbookId;
                        var parentWorkbookFolder =
                            workbookFolders.Find(x => x.Data.Id == workbookId);

                        if (parentWorkbookFolder == null)
                        {
                            var newWorkbookFolder = new TreeNode<SiteViewResource>(new SiteViewFolderResource(workbookName, workbookId));
                            newWorkbookFolder.AddChildren(view);
                            workbookFolders.Add(newWorkbookFolder);
                        }
                        else
                        {
                            parentWorkbookFolder.AddChildren(view);
                        }
                    }

                    foreach (var folder in workbookFolders)
                    {
                        root.Add(folder);
                    }
                }
                return new CacheEntry<TreeNode<SiteViewResource>>(root);
            }).Value;
        }

        private List<TreeNode<SiteViewResource>> GetWorkbookFolders()
        {
            var workbookFolders = new List<TreeNode<SiteViewResource>>();
            var workbooks = TableauRestService.DownloadWorkbooksList().Workbooks.OrderBy(x => x.Name);
            foreach (var workbook in workbooks)
            {
                var newWorkbookFolder = new TreeNode<SiteViewResource>(new SiteViewFolderResource(workbook.Name, workbook.Id));
                workbookFolders.Add(newWorkbookFolder);
            }

            return workbookFolders;
        }

        [SetTableauTrustedTicket]
        [Route("/Reporting/Report/{id}/{anchorName}")]
        [ActionName(ActionNames.Report)]
        public IActionResult Report(string id, string anchorName)
        {
            var root = GetReportFolderTreeRoot();
            SiteViewEmbeddableResource matchingSiteViewResource = null;
            root.Walk((node, depth) =>
            {
                var siteViewResource = node.Data;
                if (siteViewResource == null) return;
                var urlFriendlyReportName = CreateAnchorName(siteViewResource);
                if (anchorName == urlFriendlyReportName && id == siteViewResource.Id)
                {
                    matchingSiteViewResource = siteViewResource as SiteViewEmbeddableResource;
                }
            });

            if (matchingSiteViewResource == null)
            {
                RedirectToAction(ActionNames.Index);
            }

            Log.Information(matchingSiteViewResource.Id);

            var viewModel = new SiteViewEmbeddableResource
            {
                WorkbookName = matchingSiteViewResource.WorkbookName,
                ViewName = matchingSiteViewResource.ViewName,
                Name = matchingSiteViewResource.Name
            };

            return View(viewModel);
        }

        [Route("/Reporting")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = GetReportFolderTreeRoot();
            return View(root);
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
                    byte[] previewImagesBytes = TableauRestService.DownloadPreviewImageForView(workbookId, viewId);
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