using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Permissions;
using TraffkPortal.Services;
using DataSource = Traffk.Bal.Data.Rdb.TraffkGlobal.DataSource;

namespace Traffk.Portal.Controllers
{
    [PermissionAuthorize(PermissionNames.DataSourceView)]
    public class DataController : BasePageController
    {
        public const string Name = "Data";

        protected readonly TraffkGlobalDbContext GDB;

        public enum PageKeys
        {
            DataSourceDetails,
            DataSourceHistory
        }

        public static class ViewNames
        {
            public const string DataSourceDetail = "DataSourceDetail";
            public const string DataSourceCreate = "DataSourceCreate";
        }

        public DataController(TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger,
            ICacher cacher,
            TraffkGlobalDbContext gdb) : base(AspHelpers.MainNavigationPageKeys.Manage, db, current, logger, cacher)
        {
            GDB = gdb;
        }

        [Route("/Data")]
        public async Task<IActionResult> Index(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var tenant = await Current.GetTenantAsync();
            var sources = GDB.DataSources.Where(x => x.TenantId == tenant.TenantId);
            foreach (var source in sources)
            {
                var fetches = GDB.DataSourceFetches.Where(x => x.DataSourceId == source.DataSourceId);
                source.DataSourceDataSourceFetches = fetches.ToList();
            }

            sources = ApplyBrowse(
                sources, sortCol ?? nameof(DataSource.DataSourceId), sortDir,
                page, pageSize,
                new Dictionary<string, string> { { nameof(DataSource.CreatedAt), nameof(DataSource.CreatedAtUtc) } });
            return View(sources);
        }

        [HttpGet]
        [Route("/Data/DataSource/Create")]
        [PermissionAuthorize(PermissionNames.DataSourceCreate)]
        public IActionResult DataSourceCreate()
        {
            SetHeroLayoutViewData(null, null, PageKeys.DataSourceDetails);

            return View(new DataSource()); 
        }

        [HttpPost]
        [Route("/Data/DataSource/Create")]
        [PermissionAuthorize(PermissionNames.DataSourceCreate)]
        public async Task<IActionResult> DataSourceCreate(DataSource dataSource)
        {
            var tenant = await Current.GetTenantAsync();
            dataSource.TenantId = tenant.TenantId;
            GDB.DataSources.Add(dataSource);
            await GDB.SaveChangesAsync();

            SetToast(AspHelpers.ToastMessages.Saved);

            return RedirectToAction("Index");
        }

        [Route("/Data/DataSource/{id}")]
        [PermissionAuthorize(PermissionNames.DataSourceEdit)]
        public async Task<IActionResult> DataSourceDetail(int id)
        {
            var tenant = await Current.GetTenantAsync();
            var dataSource = GDB.DataSources.FirstOrDefault(x => x.TenantId == tenant.TenantId && x.DataSourceId == id);

            SetHeroLayoutViewData(dataSource.DataSourceId, dataSource.DataSourceId.ToString(), PageKeys.DataSourceDetails);

            return View(dataSource);
        }

        [Route("/Data/DataSource/{id}/History")]
        public async Task<IActionResult> DataSourceFetches(int id)
        {
            var tenant = await Current.GetTenantAsync();
            var dataSource = GDB.DataSources.FirstOrDefault(x => x.TenantId == tenant.TenantId && x.DataSourceId == id);
            if (dataSource == null)
            {
                throw new InvalidOperationException();
            }
            SetHeroLayoutViewData(dataSource.DataSourceId, dataSource.DataSourceId.ToString(), PageKeys.DataSourceHistory);
            var fetches = GDB.DataSourceFetches.Where(x => x.DataSourceId == id);
            return View(fetches);
        }

        [Route("/Data/DataSource/{id}/Fetches/{fetchId}")]
        public async Task<IActionResult> DataSourceFetchItems(int id, int fetchId)
        {
            var tenant = await Current.GetTenantAsync();
            var dataSource = GDB.DataSources.FirstOrDefault(x => x.TenantId == tenant.TenantId && x.DataSourceId == id);
            if (dataSource == null)
            {
                throw new InvalidOperationException();
            }
            var dataSourceFetch = GDB.DataSourceFetches.FirstOrDefault(x => x.DataSourceId == dataSource.DataSourceId && x.DataSourceFetchId == fetchId);
            if (dataSourceFetch == null)
            {
                throw new InvalidOperationException();
            }

            SetHeroLayoutViewData(dataSource.DataSourceId, "Fetch " + dataSource.DataSourceId.ToString(), PageKeys.DataSourceHistory);

            var fetchItems =
                GDB.DataSourceFetchItems.Where(x => x.DataSourceFetchId == dataSourceFetch.DataSourceFetchId);
            return View(fetchItems);
        }
    }
}
