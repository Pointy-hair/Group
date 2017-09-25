using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using System.Linq;
using Traffk.Bal.Caches;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.ExternalApis;
using Traffk.Tableau.REST;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Services;

namespace Traffk.Portal.Controllers
{
    [Route("/ping")]
    public class PingdomController : BasePageController
    {
        protected readonly ITableauStatusService TableauStatusService;
        protected readonly OrchestraApiService OrchestraApiService;
        protected readonly IRedisCache Redis;

        private const string SuccessString = "Success";

        public PingdomController( 
            TraffkTenantModelDbContext db, 
            CurrentContextServices current, 
            ILogger logger,
            ITableauStatusService tableauStatusService,
            OrchestraApiService orchestraApiService,
            IRedisCache redis,
            ICacher cacher = null) 
            : base(AspHelpers.MainNavigationPageKeys.NotSpecified, db, current, logger, cacher)
        {
            TableauStatusService = tableauStatusService;
            OrchestraApiService = orchestraApiService;
            Redis = redis;
        }

        [Route("db")]
        public IActionResult Index()
        {
            var contact = Rdb.Contacts.FirstOrDefault();

            if (contact != null)
            {
                return Content(SuccessString);
            }

            return NoContent();
        }

        [Route("tableau")]
        public IActionResult PingTableau()
        {
            var isTableauOnline = TableauStatusService.IsOnline;
            if (isTableauOnline)
            {
                return Content(SuccessString);
            }

            return NoContent();
        }

        [Route("pharmacyApi")]
        public IActionResult PingPharmacyApi()
        {
            var pharmacy = OrchestraApiService.PharmacyTestSearch();
            if (pharmacy != null)
            {
                return Content(SuccessString);
            }

            return NoContent();
        }

        [Route("redis")]
        public IActionResult PingRedis()
        {
            var isRedisOnline = Redis.Connection.IsConnected;

            if (isRedisOnline)
            {
                return Content(SuccessString);
            }

            return NoContent();
        }
    }
}
