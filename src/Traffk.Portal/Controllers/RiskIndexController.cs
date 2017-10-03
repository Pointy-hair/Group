using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Settings;
using Traffk.Portal.Controllers;
using Traffk.Portal.Models.ReportingModels;
using Traffk.Tableau;
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
        protected bool IsOnline;

        public static class ActionNames
        {
            public const string Index = "Index";
            public const string Report = "Report";
        }

        public RiskIndexController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger,
            ICacher cacher,
            IReportVisualService reportVisualService
        )
            : base(AspHelpers.MainNavigationPageKeys.RiskIndex, db, current, logger, cacher)
        {
            ReportVisualService = reportVisualService;
            IsOnline = ReportVisualService.IsOnline;
        }

        [Route("/RiskIndex")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            if (!IsOnline)
            {
                return RedirectToAction(ReportingController.ActionNames.Offline, ReportingController.Name);
            }

            var reportSearchCriteria = new ReportSearchCriteria
            {
                VisualContext = ReportVisualContext,
                TagFilter = new List<string>{ReportTagFilters.RiskIndex}
            };
            var root = ReportVisualService.GetReportFolderTreeRoot(reportSearchCriteria);
            return View(root);
        }

        [SetTableauTrustedTicket]
        [Route("/RiskIndex/Report/{id}/{anchorName}")]
        [ActionName(ActionNames.Report)]
        public IActionResult Report(string id, string anchorName)
        {
            if (!IsOnline)
            {
                return RedirectToAction(ReportingController.ActionNames.Offline, ReportingController.Name);
            }

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
    }
}