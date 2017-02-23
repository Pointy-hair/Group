using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Services;
using TraffkPortal.Permissions;
using Traffk.Bal.Permissions;
using Traffk.Tableau;

namespace TraffkPortal.Controllers
{
    [Authorize]
    public class HomeController : BasePageController
    {
        public const string Name = "Home";

        public static class ActionNames
        {
            public const string Index = "Index";
        }

        public HomeController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
            )
            : base(AspHelpers.MainNavigationPageKeys.Main, db, current, loggerFactory)
        { }

        [ActionName(ActionNames.Index)]
        [SetTableauTrustedTicket]
        [SetPowerBiBearer]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            System.Diagnostics.Trace.WriteLine(Current.Application.ApplicationSettings);
            System.Diagnostics.Trace.WriteLine(Current.Tenant.TenantSettings);
            Rdb.SaveChanges();
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Route("/Error")]
        public IActionResult Error()
        {
            return View();
        }

        [PermissionAuthorize(PermissionNames.ReleaseLog)]
        public async Task<IActionResult> Releases(string sortCol, string sortDir, int? startAt)
        {
            var releases = (IQueryable<Release>) Rdb.Releases.Include(r => r.ReleaseReleaseChanges);
            releases = ApplySort(releases, sortCol ?? nameof(Release.ReleaseDate), sortDir??AspHelpers.SortDirDescending);
            return View(await releases.ToListAsync());
        }
    }
}
