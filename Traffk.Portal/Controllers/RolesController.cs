using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TraffkPortal.Models.RoleModels;
using RevolutionaryStuff.Core;
using TraffkPortal.Permissions;
using Microsoft.AspNetCore.Authorization;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Permissions;
using TraffkPortal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.ManageRoles)]
    public class RolesController : BasePageController
    {
        public const string Name = "Roles";

        public RolesController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
        )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, loggerFactory)
        { }

        // GET: Roles
        public async Task<IActionResult> Index(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var roles = Rdb.Roles.Where(z => z.TenantId == TenantId);
            roles = ApplyBrowse(roles, sortCol??nameof(ApplicationRole.Name), sortDir, page, pageSize);
            return View(await roles.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await Rdb.Roles.Include(lo => lo.Claims).SingleOrDefaultAsync(lo => lo.Id == id && lo.TenantId == TenantId);
            if (applicationRole == null)
            {
                return NotFound();
            }
            return View(new RoleDetailViewModel(applicationRole));
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(nameof(ApplicationRole.Name))] ApplicationRole m)
        {
            if (ModelState.IsValid)
            {
                Rdb.Add(ApplicationRole.Create(m.Name));
                await Rdb.SaveChangesAsync();
                return RedirectToIndex();
            }
            return View(m);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await Rdb.Roles.Include(lo=>lo.Claims).SingleOrDefaultAsync(lo => lo.Id == id && lo.TenantId == TenantId);
            if (applicationRole == null)
            {
                return NotFound();
            }
            return View(new RoleDetailViewModel(applicationRole));
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind(nameof(RoleDetailViewModel.Id), nameof(RoleDetailViewModel.SelectedPermissions), nameof(RoleDetailViewModel.RoleName), nameof(RoleDetailViewModel.InitialPermissionList))] RoleDetailViewModel m)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var applicationRole = await Rdb.Roles.Include(lo => lo.Claims).SingleOrDefaultAsync(lo => lo.Id == id && lo.TenantId == TenantId);
                if (applicationRole == null)
                {
                    return NotFound();
                }
                try
                {
                    applicationRole.Name = m.RoleName;
                    applicationRole.NormalizedName = m.RoleName.ToLower();
                    applicationRole.ConcurrencyStamp = Guid.NewGuid().ToString();
                    applicationRole.Claims.Clear();
                    foreach (var p in m.SelectedPermissions.ConvertAll(s => Parse.ParseEnum<PermissionNames>(s)))
                    {
                        applicationRole.Claims.Add(PermissionHelpers.CreateIdentityRoleClaim(p));
                    }
                    Rdb.Update(applicationRole);
                    await Rdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationRoleExists(applicationRole.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToIndex();
            }
            return View(m);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await Rdb.Roles.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationRole == null)
            {
                return NotFound();
            }

            return View(new RoleDetailViewModel(applicationRole));
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationRole = await Rdb.Roles.SingleOrDefaultAsync(m => m.Id == id);
            Rdb.Roles.Remove(applicationRole);
            await Rdb.SaveChangesAsync();
            return RedirectToIndex();
        }

        private bool ApplicationRoleExists(string id)
        {
            return Rdb.Roles.Any(e => e.Id == id);
        }
    }
}

