using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using TraffkPortal.Models.RoleModels;
using TraffkPortal.Permissions;
using TraffkPortal.Services;
using static TraffkPortal.AspHelpers;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.ManageRoles)]
    public class RolesController : BasePageController
    {
        public const string Name = "Roles";

        public static class ActionNames
        {
            public const string Index = "Index";
        }

        private static class ViewNames
        {
            public const string Index = "Roles";
        }

        public RolesController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger
        )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, logger)
        { }

        [ActionName(ActionNames.Index)]
        public async Task<IActionResult> Index(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var roles = Rdb.Roles.Where(z => z.TenantId == TenantId);
            roles = ApplyBrowse(roles, sortCol??nameof(ApplicationRole.Name), sortDir, page, pageSize);
            return View(ViewNames.Index, await roles.ToListAsync());
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
                SetToast(AspHelpers.ToastMessages.Saved);
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
                    SetToast(AspHelpers.ToastMessages.Saved);
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


        private bool ApplicationRoleExists(string id)
        {
            return Rdb.Roles.Any(e => e.Id == id);
        }

        [HttpDelete]
        public Task<IActionResult> Delete(bool showToast = false)
        {
            if (showToast)
            {
                SetToast(ToastMessages.Deleted);
            }
            return JsonDeleteFromRdbAsync<ApplicationRole, string>(Rdb.Roles);
        } 
    }
}

