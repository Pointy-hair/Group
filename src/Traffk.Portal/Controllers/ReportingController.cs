using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TraffkPortal.Services;
using Microsoft.AspNetCore.Authorization;
using TraffkPortal.Permissions;
using RevolutionaryStuff.PowerBiToys.Objects;
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

        public static string CreateAnchorName(PowerBiEmbeddableResource er) => NameHelpers.GetName(er)?.Trim()?.ToUpperCamelCase() ?? "";
        public static string CreateAnchorName(SiteViewResource view) => NameHelpers.GetName(view)?.Trim()?.ToUpperCamelCase() + view.Id ?? "";

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

        public class FolderResource : PowerBiResource, IName
        {
            string IName.Name 
            {
                get
                {
                    return Id;
                }
            }

            public override string ToString() => Id;

            public FolderResource(string name)
            {
                Id = name;
            }
        }

        private TreeNode<PowerBiResource> GetRoot()
        {
            return Cacher.FindOrCreate("root", async key =>
            {
                var root = new TreeNode<PowerBiResource>(new FolderResource("Root"));
                var dashboards = (await Current.PowerBi.GetDatasets()).Values.OrderBy(d => d.Name).ToList();
                if (dashboards.Count > 0)
                {
                    var dashboardFolder = new TreeNode<PowerBiResource>(new FolderResource("Dashboards"));
                    foreach (var d in dashboards)
                    {
                        var tiles = (await Current.PowerBi.GetTiles(d.Id)).Values;
                        if (tiles != null)
                        {
                            var dn = dashboardFolder.Add(d);
                            dn.AddChildren(tiles);
                        }
                    }
                    if (dashboardFolder.HasChildren)
                    {
                        root.Add(dashboardFolder);
                    }
                }
                var reports = (await Current.PowerBi.GetReports()).Values.OrderBy(r => r.Name).ToList();
                if (reports.Count > 0)
                {
                    var reportsFolder = new TreeNode<PowerBiResource>(new FolderResource("Reports"));
                    reportsFolder.AddChildren(reports);
                    root.Add(reportsFolder);
                }
                /*
                            m.Root.Children.Add(Create("Datasets", await Current.PowerBi.GetDatasets()));
                            m.Root.Children.Add(Create("Groups", await Current.PowerBi.GetGroups()));
                */
                return new CacheEntry<TreeNode<PowerBiResource>>(root);
            }).Value;
        }

        private TreeNode<SiteViewResource> GetReportFolderTreeRoot()
        {
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

            return Cacher.FindOrCreate("root", async key => new CacheEntry<TreeNode<SiteViewResource>>(root)).Value;
        }

        private List<TreeNode<SiteViewResource>> GetWorkbookFolders()
        {
            var workbookFolders = new List<TreeNode<SiteViewResource>>();
            var workbooks = TableauRestService.DownloadWorkbooksList().Workbooks;
            foreach (var workbook in workbooks)
            {
                var newWorkbookFolder = new TreeNode<SiteViewResource>(new SiteViewFolderResource(workbook.Name, workbook.Id));
                workbookFolders.Add(newWorkbookFolder);
            }

            return workbookFolders;
        }

        [SetPowerBiBearer]
        [Route("/Reporting/{anchorName}")]
        [ActionName(ActionNames.ShowReport)]
        public IActionResult ShowReport(string anchorName)
        {
            var root = GetRoot();
            PowerBiEmbeddableResource e = null;
            root.Walk((node, depth) => {
                var pbi = node.Data as PowerBiEmbeddableResource;
                if (pbi == null) return;
                var urlFriendlyReportName = CreateAnchorName(pbi);
                if (anchorName == urlFriendlyReportName || pbi.Id == anchorName)
                {
                    e = pbi as PowerBiEmbeddableResource;
                }
            });
            if (e == null) return NotFound();
            return View(e);
        }

        [SetTableauTrustedTicket]
        [Route("/Reporting/Report")]
        [ActionName(ActionNames.Report)]
        public IActionResult Report(string anchorName)
        {
            var root = GetReportFolderTreeRoot();
            SiteViewEmbeddableResource matchingSiteViewResource = null;
            root.Walk((node, depth) =>
            {
                var siteViewResource = node.Data;
                if (siteViewResource == null) return;
                var urlFriendlyReportName = CreateAnchorName(siteViewResource);
                if (anchorName == urlFriendlyReportName)
                {
                    matchingSiteViewResource = siteViewResource as SiteViewEmbeddableResource;
                }
            });

            Log.Information(matchingSiteViewResource.Id);

            var viewModel = new SiteViewEmbeddableResource
            {
                WorkbookName = matchingSiteViewResource.WorkbookName,
                ViewName = matchingSiteViewResource.ViewName,
                Name = matchingSiteViewResource.Name
            };

            return View(viewModel);
        }

        [SetPowerBiBearer]
        [Route("/Reporting")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = GetReportFolderTreeRoot();
            return View(root);
        }
    }
}