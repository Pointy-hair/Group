using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Traffk.Portal.Models.UniversalLoginViewModels;
using TraffkPortal.Services.TenantServices;


namespace Traffk.Portal.Controllers
{
    public class UniversalLoginController : Controller
    {
        private readonly TenantFinderService.Config TenantConfig;

        public UniversalLoginController(IOptions<TenantFinderService.Config> tenantConfig)
        {
            TenantConfig = tenantConfig.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(UniversalLoginViewModel model)
        {
            var tenantedUrl = @"http://" +
                              TenantConfig.DefaultTenantFinderHostPattern.Replace(@"{0}", model.TenantName);
            return Redirect(tenantedUrl);
        }
    }
}
