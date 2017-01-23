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
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.BasicReporting)]
    public class ReportingController : BasePageController
    {
        public const string Name = "Reporting";
        public ITableauRestService TableauRestService { get; set; }

        public static string CreateAnchorName(PowerBiEmbeddableResource er) => NameHelpers.GetName(er)?.Trim()?.ToUpperCamelCase() ?? "";

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

        private TreeNode<SiteViewResource> GetRoot(ITableauRestService service)
        {
            var root = new TreeNode<SiteViewResource>(new SiteViewFolderResource("Root"));
            var views = service.DownloadViewsForSite().Views;
            if (views.Count() > 0)
            {
                views = views.OrderBy(r => r.Name);
                var viewFolder = new TreeNode<SiteViewResource>(new SiteViewFolderResource("Reports"));
                foreach (var view in views)
                {
                    viewFolder.AddChildren(view);
                }

                root.Add(viewFolder);
            }

            return Cacher.FindOrCreate("root", async key => new CacheEntry<TreeNode<SiteViewResource>>(root)).Value;
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
        public IActionResult Report(string workbook, string viewname)
        {
            //var root = GetRoot(TableauRestService);
            //PowerBiEmbeddableResource e = null;
            //root.Walk((node, depth) => {
            //    var pbi = node.Data as PowerBiEmbeddableResource;
            //    if (pbi == null) return;
            //    var urlFriendlyReportName = CreateAnchorName(pbi);
            //    if (anchorName == urlFriendlyReportName || pbi.Id == anchorName)
            //    {
            //        e = pbi as PowerBiEmbeddableResource;
            //    }
            //});
            var viewModel = new SiteViewViewModel
            {
                WorkbookName = workbook,
                ViewName = viewname,
                Name = viewname
            };
            return View(viewModel);
        }

        [SetPowerBiBearer]
        [Route("/Reporting")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = GetRoot(TableauRestService);
            return View(root);
        }
    }
}