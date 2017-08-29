using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System.Linq;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Portal.Models.UniversalLoginViewModels;
using TraffkPortal.Services.TenantServices;

namespace Traffk.Portal.Controllers
{
    [AllowAnonymous]
    [Route("UniversalLogin")]
    public class UniversalLoginController : Controller
    {
        private readonly TenantFinderService.Config TenantConfig;
        private TraffkTenantShardsDbContext DB;

        public UniversalLoginController(IOptions<TenantFinderService.Config> tenantConfig,
            TraffkTenantShardsDbContext db)
        {
            TenantConfig = tenantConfig.Value;
            DB = db;
        }

        public static string Name = "UniversalLogin";

        public static class ActionNames
        {
            public const string UniversalLogin = "Index";
        }

        [HttpGet]
        [ActionName(ActionNames.UniversalLogin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UniversalLoginViewModel model)
        {
            var app = DB.AppFindByHostname(null, AppTypes.Portal, model.LoginDomain).ExecuteSynchronously().FirstOrDefault();

            if (app == null)
            {
                throw new TenantFinderServiceException(TenantServiceExceptionCodes.TenantNotFound,
                    $"host=[{model.LoginDomain}]");
            }

            return Redirect(@"http://" + app.PreferredHostname);
        }
    }
}
