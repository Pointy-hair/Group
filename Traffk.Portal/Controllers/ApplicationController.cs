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
using Traffk.Bal.Permissions;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Models.ApplicationModels;
using Traffk.Bal.Settings;
using System.IO;
using Traffk.Bal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.ManageTenants)]
    [Route("Applications")]
    public class ApplicationController : BasePageController
    {
        public class ActionNames
        {
            public const string ApplicationBasics = "Edit";
        }

        public enum PageKeys
        {
            Basics,
            CommunicationSettings,
            PortalSettings,
            RegistrationSettings,
        }

        private void SetHeroLayoutViewData(Application app, PageKeys pageKey)
        {
            SetHeroLayoutViewData(app.ApplicationId, app.ApplicationName, pageKey, app.ApplicationType);
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

        public async Task<ActionResult> Index()
        {
            return View(await Rdb.Applications.Where(a => a.TenantId == Current.TenantId).ToListAsync());
        }

        private async Task<Application> GetApplicationAsync(int applicationId)
        {
            return await Rdb.Applications.FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.TenantId == TenantId);
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

        [Route("{id}/CommunicationSettings")]
        public async Task<ActionResult> Communications(int id)
        {
            var app = await GetApplicationAsync(id);
            if (app == null) return NotFound();
            SetHeroLayoutViewData(app, PageKeys.CommunicationSettings);
            var definitions = SystemCommunication.Definitions.GetByApplicationType(app.ApplicationType);
            var communications = await Rdb.SystemCommunications.Where(z => z.TenantId == this.TenantId && z.ApplicationId == id || z.ApplicationId==null).ToListAsync();
            var messageTemplates = await Rdb.MessageTemplates.Where(z => z.TenantId == this.TenantId).ToListAsync();
            var templateInfoByTemplateId =
                (
                await 
                (
                from z in Rdb.Templates
                where z.TenantId == this.TenantId
                select new Template { TemplateId = z.TemplateId, ModelType = z.ModelType, TemplateName = z.TemplateName, TemplateEngineType = z.TemplateEngineType }
                ).ToListAsync()
                ).ToDictionary(z=>z.TemplateId);
            return View(new CommunicationsModel(communications, definitions, messageTemplates, templateInfoByTemplateId));
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
                app.ApplicationName = m.ApplicationName;
                app.ApplicationSettings.EmailSenderAddress = m.EmailSenderAddress;
                app.ApplicationSettings.EmailSenderName = m.EmailSenderName;
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
            var m = new RegistrationModel(app.ApplicationSettings.Registration);
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
                app.ApplicationSettings.Registration.UsersCanSelfRegister = m.UsersCanSelfRegister;
                app.ApplicationSettings.Registration.SelfRegistrationMandatoryEmailAddressHostnames = m.SelfRegistrationMandatoryEmailAddressHostnames;
                await Rdb.SaveChangesAsync();
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
            return View(new PortalOptionsModel(app.ApplicationSettings.PortalOptions));
        }

        [HttpDelete]
        [Route("{applicationId}/RemovePortalAsset/{assetKey}")]
        public async Task<IActionResult> RemovePortalAsset(int applicationId, string assetKey)
        {
            var app = await GetApplicationAsync(applicationId);
            if (app == null) return NotFound();
            var po = app.ApplicationSettings.PortalOptions ?? new PortalOptions();
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
                var po = app.ApplicationSettings.PortalOptions ?? new PortalOptions();
                app.ApplicationSettings.PortalOptions = po;
                po.RegisterMessage = model.RegisterMessage;
                po.LoginMessage = model.LoginMessage;
                po.HomeMessage = model.HomeMessage;
                po.CopyrightMessage = model.CopyrightMessage;
                po.AboutMessage = model.AboutMessage;
                po.PrimaryColor = model.PrimaryColor;
                po.SecondaryColor = model.SecondaryColor;
                if (model.LogoFile != null)
                {
                    po.LogoLink = await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.LogoFile, "customlogo" + Path.GetExtension(model.LogoFile.FileName), true);
                }
                if (model.CssFile != null)
                {
                    po.CssLink = await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.CssFile, "customcss" + Path.GetExtension(model.CssFile.FileName), true);
                }
                if (model.JavascriptFile != null)
                {
                    po.JavascriptLink = await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, model.JavascriptFile, "customjs" + Path.GetExtension(model.JavascriptFile.FileName), true);
                }
                Rdb.Update(app);
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ActionNames.ApplicationBasics);
            }
            return View(model);
        }
    }
}