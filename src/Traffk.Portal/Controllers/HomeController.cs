using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TraffkPortal.Services;
using TraffkPortal.Permissions;
using Traffk.Bal.Permissions;
using Traffk.Tableau;
using ILogger = Serilog.ILogger;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace TraffkPortal.Controllers
{
    [Authorize]
    public class HomeController : BasePageController
    {
        public const string Name = "Home";

        public static class ActionNames
        {
            public const string Index = "Index";
            public const string PrivacyPolicy = "PrivacyPolicy";
            public const string TermsConditions = "Terms";
            public const string Releases = "Releases";
            public const string Contact = "Contact";
            public const string About = "About";
        }

        public HomeController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger
            )
            : base(AspHelpers.MainNavigationPageKeys.Main, db, current, logger)
        { }

        [ActionName(ActionNames.Index)]
        [SetTableauTrustedTicket]
        public IActionResult Index()
        {
            return View();
        }

        [ActionName(ActionNames.About)]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            System.Diagnostics.Trace.WriteLine(Current.Application.AppSettings);
            System.Diagnostics.Trace.WriteLine(Current.Tenant.TenantSettings);
            Rdb.SaveChanges();
            return View();
        }

        [ActionName(ActionNames.Contact)]
        [AllowAnonymous]
        public IActionResult Contact()
        {
            return Redirect("https://www.traffk.com");
        }

        [PermissionAuthorize(PermissionNames.ReleaseLog)]
        [ActionName(ActionNames.Releases)]
        [Route("Releases")]
        public async Task<IActionResult> Releases(string sortCol, string sortDir, int? startAt)
        {
            var releases = (IQueryable<Release>) Rdb.Releases;
            releases = ApplySort(releases, sortCol ?? nameof(Release.ReleaseDate), sortDir??AspHelpers.SortDirDescending);
            return View(await releases.ToListAsync());
        }

        [ActionName(ActionNames.PrivacyPolicy)]
        [AllowAnonymous]
        [Route("PrivacyPolicy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [ActionName(ActionNames.TermsConditions)]
        [AllowAnonymous]
        [Route("Terms")]
        public IActionResult Terms()
        {
            return View();
        }
    }
}
