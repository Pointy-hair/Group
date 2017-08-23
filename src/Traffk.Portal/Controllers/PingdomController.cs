using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.ExternalApis;
using Traffk.Bal.ReportVisuals;
using Traffk.Tableau.REST;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Services;

namespace Traffk.Portal.Controllers
{
    public class PingdomController : BasePageController
    {
        protected readonly ITableauStatusService TableauStatusService;
        protected readonly OrchestraApiService OrchestraApiService;
        public PingdomController( 
            TraffkTenantModelDbContext db, 
            CurrentContextServices current, 
            ILogger logger,
            ITableauStatusService tableauStatusService,
            OrchestraApiService orchestraApiService,
            ICacher cacher = null) 
            : base(AspHelpers.MainNavigationPageKeys.NotSpecified, db, current, logger, cacher)
        {
            TableauStatusService = tableauStatusService;
            OrchestraApiService = orchestraApiService;
        }

        [Route("/ping")]
        public IActionResult Index()
        {
            var contact = Rdb.Contacts.FirstOrDefault();
            var isOnline = TableauStatusService.IsOnline;
            var pharmacy = OrchestraApiService.PharmacySearch("90254", 2);

            if (contact != null && isOnline && pharmacy != null)
            {
                return Content("Success");
            }

            return Content("Fail");
        }
    }
}
