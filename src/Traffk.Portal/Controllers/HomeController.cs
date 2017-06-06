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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            System.Diagnostics.Trace.WriteLine(Current.Application.AppSettings);
            System.Diagnostics.Trace.WriteLine(Current.Tenant.TenantSettings);
            Rdb.SaveChanges();
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [PermissionAuthorize(PermissionNames.ReleaseLog)]
        public async Task<IActionResult> Releases(string sortCol, string sortDir, int? startAt)
        {
            var releases = (IQueryable<Release>) Rdb.Releases;
            releases = ApplySort(releases, sortCol ?? nameof(Release.ReleaseDate), sortDir??AspHelpers.SortDirDescending);
            return View(await releases.ToListAsync());
        }
    }
}
