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

        private string CreateAnchorName(string name) => name.Trim()?.ToUpperCamelCase()?.RemoveSpecialCharacters() ?? "";

        private TreeNode<IReportResource> GetReportFolderTreeRoot()
        {
            return Cacher.FindOrCreate("root", key => {
                var root = new TreeNode<IReportResource>(new ReportVisualFolder("Root"));
                var views = ReportVisualService.GetReportVisuals(VisualContext.Tenant, ReportTagFilters.RiskIndex);

                if (views.Any())
                {
                    views = views.OrderBy(r => r.Title);
                    //var workbookFolders = GetWorkbookFolders();

                    foreach (var view in views)
                    {
                        root.AddChildren(view);
                    }
                }
                return new CacheEntry<TreeNode<IReportResource>>(root);
            }).Value;
        }
    }
}