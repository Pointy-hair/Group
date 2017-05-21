using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Settings;
using TraffkPortal.Services;
using static TraffkPortal.AspHelpers;
using ILogger = Serilog.ILogger;

namespace TraffkPortal.Controllers
{
    public abstract class BasePageController : Controller
    {
        protected readonly CurrentContextServices Current;
        protected ILogger Logger;
        protected readonly TraffkRdbContext Rdb;
        protected readonly MainNavigationPageKeys MainNavPageKey;
        protected int TenantId { get { return Current.TenantId;  } }

        protected ActionResult RedirectToIndex()
            => RedirectToAction("Index");

        protected ActionResult RedirectToHome(object routeValues=null)
            => RedirectToAction(nameof(HomeController.Index), "Home", routeValues);

        protected ActionResult ErrorResult()
        {
            return RedirectToAction(ErrorController.ActionNames.Index, ErrorController.Name);
        }

        private static readonly IDictionary<string, string> NoMappings = new Dictionary<string, string>().AsReadOnly();

        protected IQueryable<T> ApplyPagination<T>(IQueryable<T> q, int? page = null, int? pageSize = null)
        {
            var rowsPerPageString = Request.Cookies["rowsPerPage"];
            var rowsPerPage = Parse.ParseInt32(rowsPerPageString, 10);
            var s = rowsPerPage;
            //var s = Stuff.Max(10, pageSize.GetValueOrDefault());
            //s = Stuff.Max(s, rowsPerPage);
            var p = Stuff.Max(1, page.GetValueOrDefault());
            ViewBag.PaginationSupported = true;
            ViewBag.PageNum = p;
            ViewBag.PageSize = s;
            return q.Skip((p - 1) * s).Take(s);
        }

        protected IQueryable<T> ApplySort<T>(IQueryable<T> q, string sortCol, string sortDir, IDictionary<string, string> colMapper = null)
        {
            var m = new Dictionary<string, string>(colMapper ?? NoMappings);
            if (!m.ContainsKey("CreatedAt"))
            {
                m["CreatedAt"] = "CreatedAtUtc";
            }
            sortCol = StringHelpers.TrimOrNull(sortCol);
            bool isAscending = AspHelpers.IsSortDirAscending(sortDir);
            ViewBag.SortCol = sortCol;
            ViewBag.SortDir = sortDir;
            if (sortCol != null)
            {
                sortCol = m.FindOrMissing(sortCol, sortCol);
                q = q.OrderByField(sortCol, isAscending);
            }
            return q;
        }

        protected IQueryable<T> ApplyBrowse<T>(IQueryable<T> q, string sortCol, string sortDir, int? page, int? pageSize, IDictionary<string, string> colMapper = null)
        {
            ViewBag.TotalItemCount = q.Count();
            q = ApplySort(q, sortCol, sortDir, colMapper);
            q = ApplyPagination(q, page, pageSize);
            return q;
        }

        protected IQueryable<T> ApplyFilters<T, TFNE>(IQueryable<T> q, params Expression<Func<TFNE, object>>[] fieldNameExpressions)
        {
            var filters = new List<KeyValuePair<string, string>>();
            foreach (var kvp in WebHelpers.ParseQueryParams(HttpContext.Request.QueryString.Value).AtomEnumerable)
            {
                if (string.IsNullOrEmpty(kvp.Key)) continue;
                var unmapped = Stuff.GetPathFromSerializedPath(typeof(TFNE), kvp.Key);
                if (unmapped == null)
                {
                    Logger.Debug($"Was not able to apply filter on field {kvp.Key} to type {typeof(TFNE)}");
                    continue;
                } 
                filters.Add(new KeyValuePair<string, string>(unmapped, kvp.Value));
            }
            q = q.ApplyFilters(filters);
            return q;
        }

        protected IQueryable<T> ApplyFilters<T>(IQueryable<T> q, params Expression<Func<T, object>>[] fieldNameExpressions)
            => ApplyFilters<T, T>(q, fieldNameExpressions);

        private readonly Stack<IDisposable> LogContextProperties = new Stack<IDisposable>();

        protected ICacher Cacher { get; set; }

        protected ILogger GetEnrichedLogger(EventType.LoggingEventTypes eventType)
        {
            return GetEnrichedLogger(new ILogEventEnricher[]
            {
                new PropertyEnricher(typeof(EventType).Name, eventType.ToString())
            });
        }

        protected ILogger GetEnrichedLogger(ILogEventEnricher[] enrichers)
        {
            var logger = Logger.ForContext(enrichers);
            return logger;
        }

        protected BasePageController(MainNavigationPageKeys mainNavPageKey, 
            TraffkRdbContext db, 
            CurrentContextServices current, 
            ILogger logger, 
            ICacher cacher = null)
        {
            Requires.NonNull(db, nameof(db));
            Requires.NonNull(current, nameof(current));
            Requires.NonNull(logger, nameof(logger));

            MainNavPageKey = mainNavPageKey;
            Rdb = db;
            Current = current;

            Logger = logger.ForContext(new ILogEventEnricher[]
            {
                new PropertyEnricher(typeof(Type).Name, this.GetType().Name),
            });

            var u = current.User;
            if (cacher != null)
            {
                Cacher = cacher.CreateScope(GetType(), TenantId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            while (LogContextProperties.Count > 0)
            {
                LogContextProperties.Pop().Dispose();
            }
            base.Dispose(disposing);
        }

        protected StatusCodeResult StatusCode(System.Net.HttpStatusCode code) => StatusCode((int)code);

        protected void SetHeroLayoutViewData(object entityId, string entityTitle, object pageKey)
            => SetHeroLayoutViewData(entityId, entityTitle, pageKey, (string)null);

        protected void SetHeroLayoutViewData<TEntityType>(object entityId, string entityTitle, object pageKey, TEntityType entityType = default(TEntityType))
        {
            ViewData[ViewDataKeys.EntityId] = entityId;
            ViewData[ViewDataKeys.EntityTitle] = entityTitle;
            ViewData[ViewDataKeys.EntityType] = entityType;
            ViewData[ViewDataKeys.PageKey] = pageKey;
        }

        protected async Task<IActionResult> JsonDeleteFromRdbAsync<TEntity, TId>(DbSet<TEntity> col) where TEntity : class
        {
            var ids = Request.BodyAsJsonObject<TId[]>();
            if (ids != null)
            {
                int numDeleted = 0;
                foreach (var id in ids)
                {
                    var item = await col.FindAsync(id);
                    if (item == null) continue;
                    var tt = item as ITraffkTenanted;
                    if (tt != null && tt.TenantId != this.TenantId) continue;
                    ++numDeleted;
                    col.Remove(item);
                }
                if (numDeleted > 0)
                {
                    await Rdb.SaveChangesAsync();
                }
            }
            return NoContent();
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewData[ViewDataKeys.MainNavPageKey] = MainNavPageKey;
            ViewData[ViewDataKeys.TenantName] = Current.TenantId.ToString();
            ViewData[ViewDataKeys.ToastMessage] = TempData[ViewDataKeys.ToastMessage];

            return base.OnActionExecutionAsync(context, next);
            /*
            var setTenant = Task.Run(() =>
            {
                var t = DB.Tenants.FirstOrDefault(z => z.TenantId == TenantFinder.TenantId);
                if (t == null)
                {
                    this.ViewData["TenantName"] = TenantFinder.TenantId.ToString();
                }
                else
                {
                    this.ViewData["TenantName"] = $"{t.TenantName}({t.TenantId})";
                }
            });

            return setTenant.ContinueWith(_ => base.OnActionExecutionAsync(context, next));
            */
        }

        protected void SetToast(string toastMessage)
        {
            TempData[ViewDataKeys.ToastMessage] = toastMessage;
        }

        public static string CreateAnchorName(IReportResource reportResource) =>
            ReportVisualService.CreateAnchorName(reportResource);

        public static string CreateAnchorName(IReportVisual reportVisual) =>
            ReportVisualService.CreateAnchorName(reportVisual);
    }
}