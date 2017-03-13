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
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;
using TraffkPortal.Models.ReportingModels;
using TraffkPortal.Permissions;
using TraffkPortal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.BasicReporting)]
    public class RiskIndexController : BasePageController
    {
        public IReportVisualService ReportVisualService { get; }
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
            IReportVisualService reportVisualService
        )
            : base(AspHelpers.MainNavigationPageKeys.RiskIndex, db, current, loggerFactory, cacher)
        {
            ReportVisualService = reportVisualService;
            AttachLogContextProperty(typeof(EventType).Name, EventType.LoggingEventTypes.Report.ToString());

            //RiskIndexReportNames = new Collection<string>();
            //var reportNames = new List<string> {"tables", "age", "urbanicity", "occupations", "diseaseincidence"};
            //foreach (var name in reportNames)
            //{
            //    RiskIndexReportNames.Add((Name + name).ToLower());
            //}
        }

        [Route("/RiskIndex")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = GetReportFolderTreeRoot();
            return View("Index", root);
        }

        private TreeNode<IReportResource> GetReportFolderTreeRoot()
        {
            return Cacher.FindOrCreate("riskindexreportroot", key => ReportVisualService.GetReportFolderTreeRoot(VisualContext.Tenant, ReportTagFilters.RiskIndex)).Value;
        }

        [SetTableauTrustedTicket]
        [Route("/RiskIndex/Report/{id}/{anchorName}")]
        [ActionName(ActionNames.Report)]
        public IActionResult Report(string id, string anchorName)
        {
            var root = GetReportFolderTreeRoot();
            TableauReportViewModel tableauReportViewModel = null;
            root.Walk((node, depth) =>
            {
                var matchingReportVisual = node.Data as ReportVisual;
                if (matchingReportVisual == null) return;
                var urlFriendlyReportName = CreateAnchorName(matchingReportVisual);
                if (anchorName == urlFriendlyReportName && (id == matchingReportVisual.Id || id == matchingReportVisual.ParentId))
                {
                    tableauReportViewModel = new TableauReportViewModel(matchingReportVisual);

                    Log.Information(matchingReportVisual.Id);
                }
            });

            if (tableauReportViewModel == null)
            {
                RedirectToAction(ActionNames.Index);
            }

            return View(tableauReportViewModel);
        }
    }
}