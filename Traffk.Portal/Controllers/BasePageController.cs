using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Services;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using static TraffkPortal.AspHelpers;
using Serilog.Context;
using RevolutionaryStuff.Core.Caching;

namespace TraffkPortal.Controllers
{
    public abstract class BasePageController : Controller
    {
        protected readonly CurrentContextServices Current;
        protected readonly ILogger Logger;
        protected readonly TraffkRdbContext Rdb;
        protected readonly MainNavigationPageKeys MainNavPageKey;
        protected int TenantId { get { return Current.TenantId;  } }

        protected ActionResult RedirectToIndex()
        {
            return RedirectToAction("Index");
        }
        protected ActionResult RedirectToHome(object routeValues=null)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home", routeValues);
        }

        protected ActionResult ErrorResult()
        {
            return ErrorResult();
        }

        protected Task<Tenant> CurrentTenant()
        {
            return Rdb.Tenants.SingleOrDefaultAsync(z => z.TenantId == this.TenantId);
        }

        private static readonly IDictionary<string, string> NoMappings = new Dictionary<string, string>().AsReadOnly();

        protected IQueryable<T> ApplyPagination<T>(IQueryable<T> q, int? page = null, int? pageSize = null)
        {
            var s = Stuff.Max(10, pageSize.GetValueOrDefault());
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
                var unmapped = Stuff.GetPathFromSerializedPath(typeof(T), kvp.Key);
                if (unmapped == null) continue;
                filters.Add(new KeyValuePair<string, string>(unmapped, kvp.Value));
            }
            q = q.ApplyFilters(filters);
            return q;
        }

        protected IQueryable<T> ApplyFilters<T>(IQueryable<T> q, params Expression<Func<T, object>>[] fieldNameExpressions)
            => ApplyFilters<T, T>(q, fieldNameExpressions);

        private readonly Stack<IDisposable> LogContextProperties = new Stack<IDisposable>();

        protected void AttachLogContextProperty(string name, object val) => LogContextProperties.Push(LogContext.PushProperty(name, val));

        protected ICacher Cacher { get; set; }

        protected BasePageController(MainNavigationPageKeys mainNavPageKey, TraffkRdbContext db, CurrentContextServices current, ILoggerFactory loggerFactory, ICacher cacher = null)
        {
            Requires.NonNull(db, nameof(db));
            Requires.NonNull(current, nameof(current));
            Requires.NonNull(loggerFactory, nameof(loggerFactory));

            MainNavPageKey = mainNavPageKey;
            Rdb = db;
            Current = current;
            Logger = loggerFactory.CreateLogger(this.GetType());
            AttachLogContextProperty("TenantId", current.TenantId);
            var u = current.User;
            if (u != null)
            {
                AttachLogContextProperty("UserName", current.User.UserName);
                AttachLogContextProperty("UserId", current.User.Id);
            }
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

        protected void SetHeroLayoutViewData(object entityId, string entityTitle, object pageKey, string entityType=null)
        {
            ViewData[ViewDataKeys.EntityId] = entityId;
            ViewData[ViewDataKeys.EntityTitle] = entityTitle;
            ViewData[ViewDataKeys.EntityType] = entityType;
            ViewData[ViewDataKeys.PageKey] = pageKey;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewData[ViewDataKeys.MainNavPageKey] = MainNavPageKey;
            ViewData[ViewDataKeys.TenantName] = Current.TenantId.ToString();
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
    }
}