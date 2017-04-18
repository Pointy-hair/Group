using RevolutionaryStuff.Core;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraffkPortal.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TraffkPortal.Permissions;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal;
using Traffk.Bal.Permissions;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;
using Traffk.Bal.Services;
using TraffkPortal.Models.ApplicationModels;
using System.IO;
using RevolutionaryStuff.Core.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using Traffk.Bal.Communications;
using System;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.ManageTenants)]
    [Route("Applications")]
    public class ApplicationController : BasePageController
    {
        public const string Name = "Application";

        public class ActionNames
        {
            public const string Index = "Index";
            public const string ApplicationBasics = "Edit";
            public const string SystemCommunications = "SystemCommunications";
            public const string SystemCommunicationsSave = "SystemCommunicationsSave";
        }

        public static class ViewNames
        {
            public const string SystemCommunications = "SystemCommunications";
        }

        public enum PageKeys
        {
            Basics,
            PortalSettings,
            RegistrationSettings,
            SystemCommunications,
        }

        private void SetHeroLayoutViewData(App app, PageKeys pageKey)
        {
            SetHeroLayoutViewData(app.AppId, app.AppName, pageKey, app.AppType);
        }

        private readonly BlobStorageServices Blobs;

        // GET: Applications
        public ApplicationController(
            BlobStorageServices blobs,
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
        )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, loggerFactory)
        {
            Blobs = blobs;
        }

        [ActionName(ActionNames.Index)]
        public async Task<ActionResult> Index()
        {
            return View(await Rdb.Apps.Where(a => a.TenantId == Current.TenantId).ToListAsync());
        }

        private async Task<App> GetApplicationAsync(int appId)
        {
            return await Rdb.Apps.FirstOrDefaultAsync(a => a.AppId == appId && a.TenantId == TenantId);
        }

        private void PopulateViewBagWithCreativeSelectListItemsByCommunicationModel(int tenantId)
        {
            var m = new MultipleValueDictionary<CommunicationModelTypes, SelectListItem>();
            Stuff.GetEnumValues<CommunicationModelTypes>().ForEach(cm => m.Add(cm, new SelectListItem { Text = AspHelpers.NoneDropdownItemText, Value = AspHelpers.NoneDropdownItemValue }));

            var creatives = from cr in Rdb.Creatives.Include(z=>z.CreativeCommunicationBlasts)
                        where cr.TenantId == TenantId && cr.CreativeTitle != null && cr.CreativeCommunicationBlasts.Count == 0
                        select cr;
            foreach (var creative in creatives)
            {
                m.Add(creative.ModelType, new SelectListItem { Text = $"{creative.CreativeTitle} ({creative.CreativeId})", Value = creative.CreativeId.ToString() });
            }
            var d = m.ToDictionaryOfCollection();
            ViewBag.CreativeSelectListItemsByCommunicationModel = d;
        }

        [Route("{id}/SystemCommunications")]
        [ActionName(ActionNames.SystemCommunications)]
        public async Task<IActionResult> SystemCommunications(int id)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            PopulateViewBagWithCreativeSelectListItemsByCommunicationModel(app.TenantId);
            SetHeroLayoutViewData(app, PageKeys.SystemCommunications);
            var model = SystemCommunicationsModel.Create(app.AppSettings.CreativeIdBySystemCommunicationPurpose);
            return View(ViewNames.SystemCommunications, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}/SystemCommunications/Save")]
        [ActionName(ActionNames.SystemCommunicationsSave)]
        public async Task<IActionResult> SystemCommunicationsSave(int id)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();

            var model = SystemCommunicationsModel.Create(app.AppSettings.CreativeIdBySystemCommunicationPurpose);

            foreach (var key in this.Request.Form.Keys)
            {
                SystemCommunicationPurposes purpose;
                if (!Enum.TryParse(key.RightOf("CreativeIdFor_"), out purpose)) continue;
                var scm = model.FirstOrDefault(z => z.Purpose == purpose);
                if (scm == null) continue;
                int creativeId;
                int.TryParse(this.Request.Form[key], out creativeId);
                scm.CreativeId = creativeId;                
            }

            app.AppSettings.CreativeIdBySystemCommunicationPurpose = model.ToDictionary(z => z.Purpose, z => z.CreativeId);
            Rdb.Update(app);
            await Rdb.SaveChangesAsync();
            SetToast(AspHelpers.ToastMessages.Saved);
            return RedirectToIndex();
        }

        [Route("{id}/Details")]
        public ActionResult Details(int id)
        {
            return View();
        }

        [Route("Create")]
        // GET: Applications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToIndex();
            }
            catch
            {
                return View();
            }
        }

        [Route("{id}")]
        [ActionName(ActionNames.ApplicationBasics)]
        public async Task<ActionResult> Edit(int id)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.Basics);
            return View(new EditModel(app));
        }

        // POST: Applications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}")]
        public async Task<ActionResult> Edit(
            int id,
            [Bind(
            nameof(EditModel.ApplicationId),
            nameof(EditModel.ApplicationName),
            nameof(EditModel.EmailSenderAddress),
            nameof(EditModel.EmailSenderName)
            )]
            EditModel m)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.Basics);
            if (ModelState.IsValid)
            {
                app.AppName = m.ApplicationName;
                app.AppSettings.EmailSenderAddress = m.EmailSenderAddress;
                app.AppSettings.EmailSenderName = m.EmailSenderName;
                await Rdb.SaveChangesAsync();
                return RedirectToIndex();
            }
            return View(m);
        }

        // GET: Applications/Delete/5
        [Route("{id}/Delete")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Applications/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}/Delete")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToIndex();
            }
            catch
            {
                return View();
            }
        }

        [Route("{id}/RegistrationSettings")]
        public async Task<ActionResult> RegistrationSettings(int id)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.RegistrationSettings);
            var m = new RegistrationModel(app.AppSettings.Registration);
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}/RegistrationSettings")]
        public async Task<ActionResult> RegistrationSettings(
            int id,
            [Bind(
            nameof(RegistrationModel.SelfRegistrationMandatoryEmailAddressHostnamesString),
            nameof(RegistrationModel.UsersCanSelfRegister)
            )]
            RegistrationModel m)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.RegistrationSettings);
            if (ModelState.IsValid)
            {
                app.AppSettings.Registration.UsersCanSelfRegister = m.UsersCanSelfRegister;
                app.AppSettings.Registration.SelfRegistrationMandatoryEmailAddressHostnames = m.SelfRegistrationMandatoryEmailAddressHostnames;
                await Rdb.SaveChangesAsync();
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToAction(ActionNames.ApplicationBasics);
            }
            return View(m);
        }

        [Route("{id}/PortalSettings")]
        public async Task<IActionResult> PortalSettings(int id)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.PortalSettings);
            return View(new PortalOptionsModel(app.AppSettings.PortalOptions));
        }

        [HttpDelete]
        [Route("{applicationId}/RemovePortalAsset/{assetKey}")]
        public async Task<IActionResult> RemovePortalAsset(int applicationId, string assetKey)
        {
            var app = await GetApplicationAsync(applicationId);
            if (app == null) return NotFound();
            var po = app.AppSettings.PortalOptions ?? new PortalOptions();
            switch (assetKey)
            {
                case nameof(PortalOptions.JavascriptLink):
                    po.JavascriptLink = null;
                    break;
                case nameof(PortalOptions.CssLink):
                    po.CssLink = null;
                    break;
                case nameof(PortalOptions.LogoLink):
                    po.LogoLink = null;
                    break;
                case nameof(PortalOptions.FaviconLink):
                    po.FaviconLink = null;
                    break;
                default:
                    throw new UnexpectedSwitchValueException(assetKey);
            }
            Rdb.Update(app);
            await Rdb.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}/PortalSettings")]
        public async Task<IActionResult> PortalSettings(
            int id,
            //            [Bind("logoFile")]
            //           IFormFile logoFile,
            [Bind(
            nameof(PortalOptionsModel.RegisterMessage),
            nameof(PortalOptionsModel.LoginMessage),
            nameof(PortalOptionsModel.HomeMessage),
            nameof(PortalOptionsModel.CopyrightMessage),
            nameof(PortalOptionsModel.AboutMessage),
            nameof(PortalOptionsModel.CssFile),
            nameof(PortalOptionsModel.LogoFile),
            nameof(PortalOptionsModel.FaviconFile),
            nameof(PortalOptionsModel.JavascriptFile),
            nameof(PortalOptionsModel.PrimaryColor),
            nameof(PortalOptionsModel.SecondaryColor)
            )]
            PortalOptionsModel model)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.PortalSettings);

            if (ModelState.IsValid)
            {
                var po = app.AppSettings.PortalOptions ?? new PortalOptions();
                app.AppSettings.PortalOptions = po;
                po.RegisterMessage = model.RegisterMessage;
                po.LoginMessage = model.LoginMessage;
                po.HomeMessage = model.HomeMessage;
                po.CopyrightMessage = model.CopyrightMessage;
                po.AboutMessage = model.AboutMessage;
                po.PrimaryColor = model.PrimaryColor;
                po.SecondaryColor = model.SecondaryColor;
                if (model.LogoFile != null)
                {
                    po.LogoLink = (await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.LogoFile, "customlogo" + Path.GetExtension(model.LogoFile.FileName), true)).Uri;
                }
                if (model.FaviconFile != null)
                {
                    po.FaviconLink = (await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.FaviconFile, "customfavicon" + Path.GetExtension(model.FaviconFile.FileName), true)).Uri;
                }
                if (model.CssFile != null)
                {
                    po.CssLink = (await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.CssFile, "customcss" + Path.GetExtension(model.CssFile.FileName), true)).Uri;
                }
                if (model.JavascriptFile != null)
                {
                    po.JavascriptLink = (await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.JavascriptFile, "customjs" + Path.GetExtension(model.JavascriptFile.FileName), true)).Uri;
                }
                Rdb.Update(app);
                await Rdb.SaveChangesAsync();
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToAction(ActionNames.ApplicationBasics);
            }
            return View(model);
        }
    }
}