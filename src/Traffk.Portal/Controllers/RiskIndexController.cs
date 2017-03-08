using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.Core.Collections;
using Serilog;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Permissions;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;
using TraffkPortal.Permissions;
using TraffkPortal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.BasicReporting)]
    public class RiskIndexController : BasePageController
    {
        public ITableauRestService TableauRestService { get; }
        public const string Name = "RiskIndex";

        private ICollection<string> RiskIndexReportNames;

        public static class ActionNames
        {
            public const string Index = "Index";
            public const string Report = "Report";
        }

        public RiskIndexController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory,
            ICacher cacher,
            ITableauRestService tableauRestService
        )
            : base(AspHelpers.MainNavigationPageKeys.RiskIndex, db, current, loggerFactory, cacher)
        {
            TableauRestService = tableauRestService;
            AttachLogContextProperty(typeof(EventType).Name, EventType.LoggingEventTypes.Report.ToString());

            RiskIndexReportNames = new Collection<string>();
            var reportNames = new List<string> {"tables", "age", "urbanicity", "occupations", "diseaseincidence"};
            foreach (var name in reportNames)
            {
                RiskIndexReportNames.Add((Name + name).ToLower());
            }
        }

        [Route("/RiskIndex")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = GetReportFolderTreeRoot("riskindex");
            return View("Index", root);
        }

        //[SetTableauTrustedTicket]
        //[Route("/RiskIndex/{id}/{anchorName}")]
        //[ActionName(RiskIndexController.ActionNames.Report)]
        //public IActionResult Report(string id, string anchorName)
        //{
        //    var root = GetReportFolderTreeRoot("riskindex");
        //    TableauEmbeddableResource matchingTableauResource = null;
        //    root.Walk((node, depth) =>
        //    {
        //        var siteViewResource = node.Data;
        //        if (siteViewResource == null) return;
        //        var urlFriendlyReportName = CreateAnchorName(siteViewResource.Name);
        //        if (anchorName == urlFriendlyReportName && id == siteViewResource.Id)
        //        {
        //            matchingTableauResource = siteViewResource as TableauEmbeddableResource;
        //        }
        //    });

        //    if (matchingTableauResource == null)
        //    {
        //        RedirectToAction(RiskIndexController.ActionNames.Index);
        //    }

        //    Log.Information(matchingTableauResource.Id);

        //    var viewModel = new TableauEmbeddableResource
        //    {
        //        WorkbookName = matchingTableauResource.WorkbookName,
        //        ViewName = matchingTableauResource.ViewName,
        //        Name = matchingTableauResource.Name
        //    };

        //    return View(viewModel);
        //}

        private string CreateAnchorName(string name) => name.Trim()?.ToUpperCamelCase()?.RemoveSpecialCharacters() ?? "";

        private TreeNode<TableauResource> GetReportFolderTreeRoot(string filter)
        {
            return Cacher.FindOrCreate(filter, key =>
            {
                var root = new TreeNode<TableauResource>(new TableauFolder(filter));
                var views = TableauRestService.DownloadViewsForSite().Views;

                if (views.Count() > 0)
                {
                    views = views.OrderBy(r => r.Name);

                    foreach (var view in views)
                    {
                        var urlFriendlyWorkbookName = CreateAnchorName(view.WorkbookName).ToLower();
                        if (RiskIndexReportNames.Contains(urlFriendlyWorkbookName))
                        {
                            root.Add(view);
                        }
                    }
                }
                return new CacheEntry<TreeNode<TableauResource>>(root);
            }).Value;
        }
    }
}