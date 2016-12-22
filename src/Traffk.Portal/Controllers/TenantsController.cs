using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TraffkPortal.Permissions;
using TraffkPortal.Models.TenantModels;
using Traffk.Bal.Settings;
using TraffkPortal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Permissions;
using System.Collections.Generic;
using System;
using Traffk.Bal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.ManageTenants)]
    [Route("Tenants")]
    public class TenantsController : BasePageController
    {
        public const string Name = "Tenants";

        public static class ViewNames
        {
            public const string TenantCreate = "Create";
            public const string TenantDelete = "Delete";
            public const string TenantDetails = "Details";
            public const string TenantEdit = "Edit";
            public const string TenantList = "Index";
            public const string DeerwalkSettings = "DeerwalkSettings";
            public const string PasswordSettings = "PasswordSettings";
            public const string SmtpSettings = "SmtpSettings";
            public const string ReusableValuesList = "ReusableValues";
            public const string ReusableValueEdit = "EditReusableValue";
        }

        public class ActionNames
        {
            public const string PasswordSettings = "PasswordSettings";
            public const string PasswordSettingsSubmission = "PasswordSettingsSubmission";
            public const string ReusableValueEditSubmission = "ReusableValueEditSubmission";
            public const string ReusableValuesList = "ReusableValuesList";
            public const string ReusableValueEdit = "ReusableValuesEdit";
            public const string ReusableValueDelete = "ReusableValueDelete";
            public const string ReusableValueCreate = "CreateReusableValue";
            public const string TenantDelete = "Delete";
            public const string TenantCreateSubmission = "Create";
            public const string TenantCreate = "TenantCreate";
            public const string TenantEdit = "Edit";
            public const string TenantDetails = "Details";
            public const string TenantsList = "Index";
            public const string SmtpSettings = "SmtpSettings";
            public const string SmtpSettingsSubmission = "SmtpSettingSubmission";
            public const string DeerwalkSettings = "DeerwalkSettings";
            public const string DeerwalkSettingsSubmission = "DeerwalkSettingsSubmission";
        }

        public enum PageKeys
        {
            Basics,
            Applications,
            DeerwalkSettings,
            PasswordSettings,
            ReusableValues,
            SmtpSettings,
        }

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly BlobStorageServices Blobs;

        public TenantsController(
            BlobStorageServices blobs,
            UserManager<ApplicationUser> userManager,
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
        )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, loggerFactory)
        {
            UserManager = userManager;
            Blobs = blobs;
        }

        [ActionName(ActionNames.TenantsList)]
        public async Task<IActionResult> Index()
        {
            return View(ViewNames.TenantList, await Rdb.Tenants.Where(t=>t.TenantId==TenantId || t.ParentTenantId==TenantId).OrderBy(t=>t.ParentTenantId).ThenBy(t=>t.TenantName).ToListAsync());
        }

        [Route("{tenantId}/Details")]
        [ActionName(ActionNames.TenantDetails)]
        public async Task<IActionResult> Details(int tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant==null) return NotFound();

            return View(ViewNames.TenantDetails, tenant);
        }

        [Route("Create")]
        [ActionName(ActionNames.TenantCreate)]
        public async Task<IActionResult> Create(int? parentTenantId)
        {
            var parentTenant = await GetTenantAsync(parentTenantId);
            if (parentTenant == null && parentTenantId.HasValue) return NotFound();
            return View(ViewNames.TenantCreate, new CreateTenantModel() { ParentTenantId = parentTenant?.TenantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [ActionName(ActionNames.TenantCreateSubmission)]
        public async Task<IActionResult> Create(
            [Bind(
            nameof(CreateTenantModel.ParentTenantId),
            nameof(CreateTenantModel.ConfirmPassword),
            nameof(CreateTenantModel.Email),
            nameof(CreateTenantModel.LoginDomain),
            nameof(CreateTenantModel.Password),
            nameof(CreateTenantModel.TenantName)
            )]
            CreateTenantModel model)
        {
            if (ModelState.IsValid)
            {
                var t = new Tenant
                {
                    ParentTenantId = model.ParentTenantId.GetValueOrDefault(TenantId),
                    LoginDomain = model.LoginDomain,
                    TenantName = model.TenantName
                };
                Rdb.Tenants.Add(t);
                Rdb.Applications.Add(new Application
                {
                    Tenant = t,
                    ApplicationType = Application.ApplicationTypes.Portal,
                    ApplicationName = "Portal"
                });
                await Rdb.SaveChangesAsync();
                var role = ApplicationRole.CreateConfigurationMasterRole(t.TenantId);
                Rdb.Roles.Add(role);
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, TenantId = t.TenantId, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, model.Password);
                await Rdb.SaveChangesAsync();
                Rdb.UserRoles.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>
                {
                     RoleId = role.Id,
                     UserId = user.Id
                });
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ActionNames.TenantsList);
            }
            return View(ViewNames.TenantEdit, model);
        }

        [Route("{tenantId}")]
        [ActionName(ActionNames.TenantEdit)]
        public async Task<IActionResult> Edit(int tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.Basics);
            return View(new TenantSettingsModel(tenant));
        }

        private void SetHeroLayoutViewData(Tenant tenant, PageKeys pageKey)
        {
            ViewData["TenantId"] = tenant.TenantId;
            ViewData["TenantName"] = tenant.TenantName;
            SetHeroLayoutViewData(tenant.TenantId, tenant.TenantName, pageKey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{tenantId}")]
        public async Task<IActionResult> Edit(
            int tenantId, 
            [Bind(
            nameof(TenantSettingsModel.TenantId),
            nameof(TenantSettingsModel.TenantName), 
            nameof(TenantSettingsModel.LoginDomain),
            nameof(TenantSettingsModel.SenderAddress),
            nameof(TenantSettingsModel.SenderName),
            nameof(TenantSettingsModel.RequiresEmailAccountValidation),
            nameof(TenantSettingsModel.RequiresTwoFactorAuthentication),
            nameof(TenantSettingsModel.ProtectedHealthInformationViewableByEmailAddressHostnamesString)
            )]
            TenantSettingsModel model)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.Basics);

            if (ModelState.IsValid)
            {
                try
                {
                    tenant.TenantName = model.TenantName;
                    tenant.TenantSettings.EmailSenderAddress = model.SenderAddress;
                    tenant.TenantSettings.EmailSenderName = model.SenderName;
                    tenant.TenantSettings.RequiresEmailAccountValidation = model.RequiresEmailAccountValidation;
                    tenant.TenantSettings.RequiresTwoFactorAuthentication = model.RequiresTwoFactorAuthentication;
                    tenant.TenantSettings.ProtectedHealthInformationViewableByEmailAddressHostnames = model.ProtectedHealthInformationViewableByEmailAddressHostnames;
                    await Rdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.TenantId))
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
            return View(tenant);
        }

        [Route("{tenantId}/Delete")]
        public IActionResult Delete(int tenantId)
        {
            var tenant = GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            throw new NotImplementedException();
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName(ActionNames.TenantDelete)]
        [ValidateAntiForgeryToken]
        [Route("{tenantId}/Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tenant = await Rdb.Tenants.SingleOrDefaultAsync(m => m.TenantId == id);
            Rdb.Tenants.Remove(tenant);
            await Rdb.SaveChangesAsync();
            return RedirectToIndex();
        }

        private bool TenantExists(int id)
        {
            return Rdb.Tenants.Any(e => e.TenantId == id);
        }

        [Route("{tenantId}/PasswordSettings")]
        [ActionName(ActionNames.PasswordSettings)]
        public async Task<IActionResult> PasswordSettings(int tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.PasswordSettings);
            return View(ViewNames.PasswordSettings, new PasswordSettingsModel(tenant.TenantSettings.Password));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{tenantId}/PasswordSettings")]
        [ActionName(ActionNames.PasswordSettingsSubmission)]
        public async Task<IActionResult> PasswordSettings(
            int tenantId,
            [Bind(
                nameof(PasswordSettingsModel.RequireDigit),
                nameof(PasswordSettingsModel.RequiredLength),
                nameof(PasswordSettingsModel.RequireLowercase),
                nameof(PasswordSettingsModel.RequireNonAlphanumeric),
                nameof(PasswordSettingsModel.RequireUppercase),
                nameof(PasswordSettingsModel.UseDefaultProhibitedPasswordList),
                nameof(PasswordSettingsModel.ProhibitedPasswords),
                nameof(PasswordSettingsModel.ProhibitedPasswordsTextArea),
                nameof(PasswordSettingsModel.PasswordUnallowedWordListTextArea),
                nameof(PasswordSettingsModel.UseDefaultPasswordUnallowedWordList),
                nameof(PasswordSettingsModel.PasswordUnallowedWordList)
            )]
            PasswordSettingsModel model)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.PasswordSettings);

            if (ModelState.IsValid)
            {
                try
                {
                    tenant.TenantSettings.Password.Copy(model);
                    Rdb.Update(tenant);
                    await Rdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.TenantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(ActionNames.TenantEdit);
            }
            return View(ViewNames.PasswordSettings, model);
        }

        [Route("{tenantId}/DeerwalkSettings")]
        [ActionName(ActionNames.DeerwalkSettings)]
        public async Task<IActionResult> DeerwalkSettings(int? tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.DeerwalkSettings);
            return View(new DeerwalkSettingsModel(tenant.TenantSettings.Deerwalk));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{tenantId}/DeerwalkSettings")]
        [ActionName(ActionNames.DeerwalkSettingsSubmission)]
        public async Task<IActionResult> DeerwalkSettings(
            int tenantId, 
            [Bind(
            nameof(DeerwalkSettingsModel.FtpHost),
            nameof(DeerwalkSettingsModel.FtpFolder),
            nameof(DeerwalkSettingsModel.FtpPassword),
            nameof(DeerwalkSettingsModel.FtpPort),
            nameof(DeerwalkSettingsModel.FtpUser),
            nameof(DeerwalkSettingsModel.ConfirmFtpPassword)
            )]
            DeerwalkSettingsModel model)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.DeerwalkSettings);

            if (ModelState.IsValid)
            {
                try
                {
                    tenant.TenantSettings.Deerwalk.FtpFolder = model.FtpFolder;
                    tenant.TenantSettings.Deerwalk.FtpHost = model.FtpHost;
                    tenant.TenantSettings.Deerwalk.FtpPort = model.FtpPort;
                    tenant.TenantSettings.Deerwalk.FtpUser = model.FtpUser;
                    if (model.FtpPassword != AspHelpers.UntouchedPassword)
                    {
                        tenant.TenantSettings.Deerwalk.FtpPassword = model.FtpPassword;
                    }
                    Rdb.Update(tenant);
                    await Rdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.TenantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(ActionNames.TenantEdit);
            }
            return View(model);
        }

        [Route("{tenantId}/SmtpSettings")]
        [ActionName(ActionNames.SmtpSettings)]
        public async Task<IActionResult> SmtpSettings(int tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.SmtpSettings);
            return View(new SmtpSettingsModel(tenant.TenantSettings.Smtp));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{tenantId}/SmtpSettings")]
        [ActionName(ActionNames.SmtpSettingsSubmission)]
        public async Task<IActionResult> SmtpSettings(
            int tenantId,
            [Bind(
            nameof(SmtpSettingsModel.SmtpHost),
            nameof(SmtpSettingsModel.SmtpPassword),
            nameof(SmtpSettingsModel.SmtpPort),
            nameof(SmtpSettingsModel.SmtpUser),
            nameof(SmtpSettingsModel.ConfirmSmtpPassword)
            )]
            SmtpSettingsModel m)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.SmtpSettings);

            if (ModelState.IsValid)
            {
                try
                {
                    tenant.TenantSettings.Smtp.SmtpHost = m.SmtpHost;
                    if (m.SmtpPassword!=AspHelpers.UntouchedPassword)
                    {
                        tenant.TenantSettings.Smtp.SmtpPassword = m.SmtpPassword;
                    }
                    tenant.TenantSettings.Smtp.SmtpPort = m.SmtpPort;
                    tenant.TenantSettings.Smtp.SmtpUser = m.SmtpHost;
                    tenant.TenantSettings.Smtp = new SmtpOptions(m);
                    Rdb.Update(tenant);
                    await Rdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.TenantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(ActionNames.TenantEdit);
            }
            return View(m);
        }

        protected Task<Tenant> GetTenantAsync(int? tenantId)
        {
            if (tenantId == null) return null;
            return Rdb.Tenants.SingleOrDefaultAsync(z => z.TenantId == tenantId.GetValueOrDefault(this.TenantId));
        }


        #region Reusable Values

        [Route("{tenantId}/ReusableValues")]
        [ActionName(ActionNames.ReusableValuesList)]
        public async Task<IActionResult> ReusableValues(int tenantId, string sortCol, string sortDir, int? startAt)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.ReusableValues);
            var q = tenant.TenantSettings.ReusableValues.AsQueryable();
            q = ApplySort(q, sortCol ?? nameof(ReusableValue.Key), sortDir);
            return View(ViewNames.ReusableValuesList, q);
        }

        public const string NewReusableValueKey = "new";
        [Route("{tenantId}/CreateReusableValue/{type}")]
        [ActionName(ActionNames.ReusableValueCreate)]
        public IActionResult CreateReusableValue(int tenantId, ReusableValueTypes type)
        {
            var rv = new ReusableValue { ResourceType = type };
            return EditReusableValue(Current.Tenant, rv);
        }

        [Route("{tenantId}/ReusableValue/{key}")]
        [ActionName(ActionNames.ReusableValueEdit)]
        public async Task<IActionResult> EditReusableValue(int tenantId, string key)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            var rv = tenant.TenantSettings.ReusableValues.FirstOrDefault(z => z.Key == key);
            return EditReusableValue(tenant, rv);
        }

        private IActionResult EditReusableValue(Tenant tenant, ReusableValue rv)
        {
            if (rv == null || tenant == null) return NotFound();
            SetHeroLayoutViewData(tenant, PageKeys.ReusableValues);
            return View(ViewNames.ReusableValueEdit, rv);
        }

        [Route("{tenantId}/ReusableValue/{key}/Delete")]
        [ActionName(ActionNames.ReusableValueDelete)]
        public async Task<IActionResult> ReusableValueDelete(int tenantId, string key)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return NotFound();
            var rv = tenant.TenantSettings.ReusableValues.FirstOrDefault(z => z.Key == key);
            if (rv != null)
            {
                tenant.TenantSettings.ReusableValues.Remove(rv);
                await Rdb.SaveChangesAsync();
            }
            return RedirectToAction(ActionNames.ReusableValuesList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{tenantId}/ReusableValue/{key}")]
        [ActionName(ActionNames.ReusableValueEditSubmission)]
        public async Task<IActionResult> EditReusableValue(
            int tenantId,
            string key,
            [Bind(
            nameof(ReusableValue.Key),
            nameof(ReusableValue.ResourceType),
            nameof(ReusableValue.Value)
            )]
            ReusableValue model)
        {
            var tenant = await GetTenantAsync(tenantId);
            SetHeroLayoutViewData(tenant, PageKeys.ReusableValues);

            if (ModelState.IsValid)
            {
                if (tenant.TenantSettings.ReusableValues == null)
                {
                    tenant.TenantSettings.ReusableValues = new List<ReusableValue>();
                }
                var rv = tenant.TenantSettings.ReusableValues.FirstOrDefault(z => z.Key == key);
                if (rv==null)
                {
                    rv = new ReusableValue() { Key = model.Key, ResourceType = model.ResourceType };
                    tenant.TenantSettings.ReusableValues.Add(rv);
                }
                rv.Value = model.Value;
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ActionNames.ReusableValuesList);
            }
            return View(ViewNames.ReusableValueEdit, model);
        }

        #endregion
    }
}
