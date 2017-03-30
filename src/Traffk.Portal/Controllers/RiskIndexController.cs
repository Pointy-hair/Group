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
        public const VisualContext ReportVisualContext = VisualContext.Tenant;

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
        }

        [Route("/RiskIndex")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var root = ReportVisualService.GetReportFolderTreeRoot(ReportVisualContext, ReportTagFilters.RiskIndex);
            return View(root);
        }

        [SetTableauTrustedTicket]
        [Route("/RiskIndex/Report/{id}/{anchorName}")]
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
    }
}