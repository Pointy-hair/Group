using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Models.UserModels;
using TraffkPortal.Services;
using TraffkPortal.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TraffkPortal.Controllers
{
    public class UsersController : BasePageController
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> UserClaimsPrincipalFactory;
        private readonly IAuthorizationService AuthorizationService;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IEmailSender EmailSender;

        public UsersController(
            IEmailSender emailSender,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, 
            IAuthorizationService authorizationService, 
            UserManager<ApplicationUser> userManager,
            TraffkRdbContext db, 
            CurrentContextServices current, 
            ILoggerFactory loggerFactory)
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, loggerFactory)
        {
            EmailSender = emailSender;
            UserClaimsPrincipalFactory = userClaimsPrincipalFactory;
            AuthorizationService = authorizationService;
            UserManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            PopulateViewBagWithRoles().ExecuteSynchronously();
            base.OnActionExecuting(context);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<IDictionary<string, ApplicationRole>> GetRolesById()
        {
            return (await Rdb.Roles.ToArrayAsync()).ToDictionary(r => r.Id);
        }

        private async Task PopulateViewBagWithRoles()
        {
            ViewBag.RoleListItems = 
            (await GetRolesById()).Values.OrderBy(r=>r.Name).ConvertAll(r => new SelectListItem { Text = r.Name, Value = r.Id });
        }

        // GET: Users
        public async Task<IActionResult> Index(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var rolesById = await GetRolesById();
            var users = Rdb.Users.Include(z => z.Roles).Where(z => z.TenantId == this.TenantId);
            users = ApplyBrowse(
                users, sortCol??nameof(ApplicationUser.UserName), sortDir, 
                page, pageSize,
                new Dictionary<string, string> { { nameof(ApplicationUser.CreatedAt), nameof(ApplicationUser.CreatedAtUtc) } });
            var model = (await users.ToListAsync()).ConvertAll(z => new UserModel(z, rolesById));
            return View(model);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var rolesById = await GetRolesById();
            var p = await UserClaimsPrincipalFactory.CreateAsync(user);
            var model = new UserModel(user, rolesById, await p.GetCanAccessProtectedHealthInformationAsync(AuthorizationService));
            return View(model);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View(new CreeateUserModel());
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
            nameof(CreeateUserModel.AssignedRoleIds),
            nameof(CreeateUserModel.Emails),
            nameof(CreeateUserModel.SendInvitationEmail),
            nameof(CreeateUserModel.InvitationText),
            nameof(CreeateUserModel.TwoFactorEnabled)
            )]
            CreeateUserModel model)
        {
            if (ModelState.IsValid)
            {
                bool hasFatalErrors = false;
                foreach (var email in model.Emails.ToArrayFromHumanDelineatedString(true))
                {
                    Requires.EmailAddress(email, nameof(email));
                    var u = await UserManager.FindByEmailAsync(email);
                    Requires.Null(u, nameof(u));
                    u = await UserManager.FindByNameAsync(email);
                    Requires.Null(u, nameof(u));
                }
                var users = new List<ApplicationUser>();
                foreach (var email in model.Emails.ToArrayFromHumanDelineatedString(true))
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        TwoFactorEnabled = model.TwoFactorEnabled
                    };
                    var result = await UserManager.CreateAsync(user, "Jbt123)(*-"+Guid.NewGuid().ToString());
                    if (!result.Succeeded)
                    {
                        hasFatalErrors = true;
                        AddErrors(result);
                        break;
                    }
                    users.Add(user);
                    if (model.SendInvitationEmail)
                    {
                        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                        var callbackUrl = Url.Action(nameof(AccountController.AcceptInvitation), AccountController.ControllerName, new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        await EmailSender.SendEmailCommunicationAsync(
                            SystemCommunication.CommunicationPurposes.AcceptInvitation,
                            Template.ModelTypes.CreateCallbackUrlModel(callbackUrl, model.InvitationText),
                            email);
                    }
                }
                if (!hasFatalErrors)
                {
                    foreach (var rid in model.AssignedRoleIds)
                    {
                        foreach (var user in users)
                        {
                            Rdb.UserRoles.Add(new IdentityUserRole<string> { RoleId = rid, UserId = user.Id });
                        }
                    }
                    await Rdb.SaveChangesAsync();
                    return RedirectToIndex();
                }
            }
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var rolesById = await GetRolesById();
            var p = await UserClaimsPrincipalFactory.CreateAsync(user);
            var model = new UserModel(user, rolesById, await p.GetCanAccessProtectedHealthInformationAsync(AuthorizationService));
            return View(model);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id, 
            [Bind(
            nameof(UserModel.Id),
            nameof(UserModel.AccessFailedCount),
            nameof(UserModel.Email),
            nameof(UserModel.EmailConfirmed),
            nameof(UserModel.LockoutEnabled),
            nameof(UserModel.LockoutEnd),
            nameof(UserModel.PhoneNumber),
            nameof(UserModel.PhoneNumberConfirmed),
            nameof(UserModel.TwoFactorEnabled),
            nameof(UserModel.UserName),
            nameof(UserModel.AssignedRoleIds)
            )]
            UserModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            var user = await GetUserByIdAsync(id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    user.AccessFailedCount = model.AccessFailedCount;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;
                    user.LockoutEnabled = model.LockoutEnabled;
                    user.LockoutEnd = model.LockoutEnd;
                    user.PhoneNumber = model.PhoneNumber;
                    user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                    user.TwoFactorEnabled = model.TwoFactorEnabled;
                    user.UserName = model.UserName;
                    var existingRolesById = user.Roles.ToDictionary(ur => ur.RoleId);
                    foreach (var rid in model.AssignedRoleIds)
                    {
                        if (!existingRolesById.ContainsKey(rid))
                        {
                            user.Roles.Add(new IdentityUserRole<string> { RoleId = rid, UserId = user.Id });
                        }
                    }
                    foreach (var ur in existingRolesById.Values)
                    {
                        if (!model.AssignedRoleIds.Contains(ur.RoleId))
                        {
                            Rdb.UserRoles.Remove(ur);
                        }
                    }
                    Rdb.Update(user);
                    await Rdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(model.Id))
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
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var rolesById = await GetRolesById();
            var p = await UserClaimsPrincipalFactory.CreateAsync(user);
            var model = new UserModel(user, rolesById, await p.GetCanAccessProtectedHealthInformationAsync(AuthorizationService));
            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user==null) return NotFound();
            Rdb.Users.Remove(user);
            await Rdb.SaveChangesAsync();
            return RedirectToIndex();
        }

        private bool ApplicationUserExists(string id)
        {
            return Rdb.Users.Any(z => z.Id == id && z.TenantId == this.TenantId);
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await Rdb.Users.Include(z=>z.Roles).FirstOrDefaultAsync(z => z.Id == id && z.TenantId==this.TenantId);
        }
    }
}
