using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using Serilog;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Communications;
using Traffk.Bal.Data;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using TraffkPortal.Models.CommunicationModels;
using TraffkPortal.Permissions;
using TraffkPortal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.Messaging)]
    [Route("Communications")]
    public class CommunicationsController : BasePageController
    {
        public const string Name = "Communications";
        public const string RemoveAttachmentScriptName = "removeCreativeAttachment";

        public static class ActionNames
        {
            public const string CommunicationsList = "Communications";
            public const string CommunicationCreate = "CommunicationCreate";
            public const string CommunicationDetails = "CommunicationDetails";
            public const string CommunicationSave = "CommunicationSave";

            public const string CommunicationSchedule = "CommunicationSchedule";
            public const string CommunicationScheduleSave = "CommunicationScheduleSave";

            public const string CommunicationCreative = "CommunicationCreative";
            public const string CommunicationCreativeSave = "CommunicationCreativeSave";

            public const string CommunicationBlasts = "CommunicationBlasts";
            public const string CommunicationBlast = "CommunicationBlast";
            /*
                        public const string CommunicationQuery = "CommunicationQuery";
                        */

            public const string CreativesList = "Creatives";
            public const string CreativeDetails = "CreativeDetails";
            public const string CreativeSave = "CreativeSave";
            public const string CreativeCreate = "CreativeCreate";
        }

        public static class ViewNames
        {
            public const string CommunicationBlasts = "CommunicationBlasts";
            public const string CommunicationBlast = "CommunicationBlast";
            public const string CommunicationsList = "Communications";
            public const string CommunicationDetails = "CommunicationDetails";
            public const string CommunicationSchedule = "CommunicationSchedule";
            public const string CommunicationCreative = "CommunicationCreative";
            public const string CreativesList = "Creatives";
            public const string CreativeDetails = "CreativeDetails";
        }

        private readonly BlobStorageServices Blobs;

        public CommunicationsController(
            BlobStorageServices blobs,
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger
            )
            : base(AspHelpers.MainNavigationPageKeys.Messaging, db, current, logger)
        {
            Blobs = blobs;
        }

        #region Communications

        public enum CommunicationPageKeys
        {
            Background,
            Schedule,
            Creative,
            Blasts,
            /*
                        Message,
                        Query,
            */
        }

        private void SetHeroLayoutViewData(Communication comm, CommunicationPageKeys pageKey)
        {
//            ViewData["JobStatus"] = blast.Job?.JobStatus;
//            ViewData[AspHelpers.ViewDataKeys.ParentEntityId] = blast.ParentCommunicationBlastId;
            SetHeroLayoutViewData(comm.CommunicationId, comm.CommunicationTitle, pageKey);
        }

        [ActionName(ActionNames.CommunicationsList)]
        [Route("")]
        public IActionResult Communications(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var items = from z in Rdb.Communications
                            where z.TenantId == TenantId && z.CommunicationTitle != null
                            select z;
            items = ApplyBrowse(
                items, sortCol ?? nameof(Communication.CommunicationTitle), sortDir,
                page, pageSize);
            return View(ViewNames.CommunicationsList, items);
        }

        private Task<Communication> FindCommunicationByIdAsync(int id) => 
            Rdb.Communications.Include(z => z.Creative).FirstOrDefaultAsync(z => z.CommunicationId == id);

        [Route("Create")]
        [ActionName(ActionNames.CommunicationCreate)]
        public IActionResult CommunicationCreate()
        {
            var comm = new Communication();
            SetHeroLayoutViewData(comm, CommunicationPageKeys.Background);
            return View(ViewNames.CommunicationDetails, comm);
        }

        [Route("{id}")]
        [ActionName(ActionNames.CommunicationDetails)]
        public async Task<IActionResult> CommunicationEdit(int id)
        {
            var comm = await FindCommunicationByIdAsync(id);
            if (comm == null) return NotFound();
            SetHeroLayoutViewData(comm, CommunicationPageKeys.Background);
            return View(ViewNames.CommunicationDetails, comm);
        }

        [HttpPost]
        [ActionName(ActionNames.CommunicationSave)]
        [ValidateAntiForgeryToken]
        [Route("{id}/Save")]
        public async Task<IActionResult> CommunicationSave(
            int id,
            [Bind(
            nameof(Communication.CommunicationTitle),
            nameof(Communication.CampaignName),
            nameof(Communication.TopicName)
            )]
            Communication model)
        {
            if (ModelState.IsValid)
            {
                var comm = await FindCommunicationByIdAsync(id);
                if (comm == null)
                {
                    comm = new Communication();
                    Rdb.Communications.Add(comm);
                }
                comm.CommunicationTitle = model.CommunicationTitle;
                comm.CampaignName = model.CampaignName;
                comm.TopicName = model.TopicName;
                await Rdb.SaveChangesAsync();
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToAction(ActionNames.CommunicationsList);
            }
            return await CommunicationEdit(id);
        }

        #endregion

        #region Schedule

        [Route("{id}/Schedule")]
        [ActionName(ActionNames.CommunicationSchedule)]
        public async Task<IActionResult> CommunicationSchedule(int id)
        {
            var item = await FindCommunicationByIdAsync(id);
            if (item == null) return NotFound();
            SetHeroLayoutViewData(item, CommunicationPageKeys.Schedule);
            return View(ViewNames.CommunicationSchedule, item.CommunicationSettings?.Recurrence ?? new RecurrenceSettings());
        }

        [ActionName(ActionNames.CommunicationScheduleSave)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id}/Schedule/Save")]
        public async Task<IActionResult> CommunicationScheduleSave(
            int id,
            RecurrenceSettings model)
        {
            var item = await FindCommunicationByIdAsync(id);
            if (item == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (model.DailyPattern.EveryWeekday)
                {
                    model.DailyPattern.EveryNDays = 0;
                }
                item.CommunicationSettings.Recurrence = model;
                Rdb.Communications.Update(item);
                await Rdb.AddNextScheduledBlasts(id, false, true);
                await Rdb.SaveChangesAsync();
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToAction(ActionNames.CommunicationDetails);
            }
            SetHeroLayoutViewData(item, CommunicationPageKeys.Schedule);
            return View(ViewNames.CommunicationSchedule, model);
        }

        #endregion

        #region Communication Creatives

        [Route("{id}/Creative")]
        [ActionName(ActionNames.CommunicationCreative)]
        public async Task<IActionResult> CommunicationCreative(int id)
        {
            var comm = await FindCommunicationByIdAsync(id);
            if (comm == null) return NotFound();
            SetHeroLayoutViewData(comm, CommunicationPageKeys.Creative);
            PopulateViewBagWithTemplateModelSelectListItems();
            return View(ViewNames.CommunicationCreative, new CreativeModel(comm.Creative, id, RemoveAttachmentScriptName));
        }

        [HttpPost]
        [ActionName(ActionNames.CommunicationCreativeSave)]
        [ValidateAntiForgeryToken]
        [Route("{id}/Creative/Save")]
        public async Task<IActionResult> CommunicationCreativeSave(
            int id,
            [Bind(
            nameof(Creative.CreativeId),
            nameof(Creative.CreativeTitle),
            nameof(Creative.TemplateEngineType),
            nameof(Creative.ModelTypeStringValue),
            nameof(Creative.CreativeSettings),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailSubject),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailHtmlBody),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailTextBody),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.TextMessageBody),
            nameof(CreativeModel.AttachmentAssets),
            nameof(CreativeModel.NewAttachments)
            )]
            CreativeModel model)
        {
            var comm = await FindCommunicationByIdAsync(id);
            if (comm == null) return NotFound();
            if (ModelState.IsValid)
            {
                Creative creative = await FindCreativeByIdAsync(model.CreativeId);
                if (creative == null)
                {
                    creative = new Creative();
                    comm.Creative = creative;
                    Rdb.Creatives.Add(creative);
                }
                await CreativeModelSaveAsync(model, creative);
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToAction(ActionNames.CommunicationDetails);
            }
            SetHeroLayoutViewData(comm, CommunicationPageKeys.Creative);
            PopulateViewBagWithTemplateModelSelectListItems();
            return View(ViewNames.CommunicationCreative, model);
        }


        #endregion

        private async Task CreativeModelSaveAsync(CreativeModel model, Creative creative)
        {
            creative.CreativeTitle = model.CreativeTitle;
            creative.ModelType = model.ModelType;
            creative.TemplateEngineType = model.TemplateEngineType == TemplateEngineTypes.Undefined ? TemplateEngineTypes.DollarString : model.TemplateEngineType;
            creative.CreativeSettings.EmailSubject = model.CreativeSettings.EmailSubject;
            creative.CreativeSettings.EmailHtmlBody = model.CreativeSettings.EmailHtmlBody;
            creative.CreativeSettings.EmailTextBody = model.CreativeSettings.EmailTextBody;
            creative.CreativeSettings.TextMessageBody = model.CreativeSettings.TextMessageBody;
            bool dirty = true;
            if (creative.CreativeId < 1)
            {
                await Rdb.SaveChangesAsync();
                dirty = false;
            }
            if (model.NewAttachments != null)
            {
                foreach (var newAttachment in model.NewAttachments)
                {
                    var pointer = await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, newAttachment, creative.AttachmentPrefix + Path.GetFileName(newAttachment.FileName), false);
                    creative.CreativeSettings.Attachments.Add(pointer);
                    dirty = true;
                }
            }
            if (dirty)
            {
                await Rdb.SaveChangesAsync();
            }
        }

        #region Creatives

        private void PopulateViewBagWithTemplateModelSelectListItems()
        {
            var items = Stuff.GetEnumValues<CommunicationModelTypes>().ConvertAll(cmt => new SelectListItem { Text = cmt.ToString(), Value = cmt.ToString() }).OrderBy(z => z.Text).ToList();
            items.Insert(0, new SelectListItem { Text = AspHelpers.NoneDropdownItemText, Value = AspHelpers.NoneDropdownItemValue });
            ViewBag.ModelListItems = items;
        }

        [ActionName(ActionNames.CreativesList)]
        [Route("Creatives")]
        public IActionResult Creatives(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var items = from cr in Rdb.Creatives
                        where cr.TenantId == TenantId && cr.CreativeTitle != null && cr.CreativeCommunicationBlasts.Count==0
                        select cr;
            items = ApplyBrowse(
                items, sortCol ?? nameof(Creative.CreativeTitle), sortDir,
                page, pageSize);
            return View(ViewNames.CreativesList, items);
        }

        private Task<Creative> FindCreativeByIdAsync(int id) => Rdb.Creatives.FindAsync(id);

        public enum CreativePageKeys
        {
            Background,
        }

        private void SetHeroLayoutViewData(Creative creative, CreativePageKeys pageKey)
        {
            PopulateViewBagWithTemplateModelSelectListItems();
            SetHeroLayoutViewData(creative.CreativeId, creative.CreativeTitle, pageKey);
        }

        [Route("Creatives/Create")]
        [ActionName(ActionNames.CreativeCreate)]
        public IActionResult CreativeCreate()
        {
            var creative = new Creative();
            SetHeroLayoutViewData(creative, CreativePageKeys.Background);
            return View(ViewNames.CreativeDetails, new CreativeModel(creative, null, RemoveAttachmentScriptName));
        }

        [Route("Creatives/{id}")]
        [ActionName(ActionNames.CreativeDetails)]
        public async Task<IActionResult> CreativeEdit(int id)
        {
            var creative = await FindCreativeByIdAsync(id);
            if (creative == null) return NotFound();
            SetHeroLayoutViewData(creative, CreativePageKeys.Background);
            return View(ViewNames.CreativeDetails, new CreativeModel(creative, null, RemoveAttachmentScriptName));
        }

        [HttpPost]
        [ActionName(ActionNames.CreativeSave)]
        [ValidateAntiForgeryToken]
        [Route("Creatives/{id}/Save")]
        public async Task<IActionResult> CreativeSave(
            int id,
            [Bind(
            nameof(Creative.CreativeTitle),
            nameof(Creative.TemplateEngineType),
            nameof(Creative.ModelTypeStringValue),
            nameof(Creative.CreativeSettings),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailSubject),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailHtmlBody),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailTextBody),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.TextMessageBody),
            nameof(CreativeModel.AttachmentAssets),
            nameof(CreativeModel.NewAttachments)
            )]
            CreativeModel model)
        {
            if (ModelState.IsValid)
            {
                var creative = await FindCreativeByIdAsync(id);
                if (creative==null)
                {
                    creative = new Creative();
                    Rdb.Creatives.Add(creative);
                }
                await CreativeModelSaveAsync(model, creative);
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToAction(ActionNames.CreativesList);
            }
            SetHeroLayoutViewData(model, CreativePageKeys.Background);
            return View(ViewNames.CreativeDetails, model);
        }

        #endregion

        #region Blasts

        [ActionName(ActionNames.CommunicationBlasts)]
        [Route("Communications/{communicationId}/Blasts")]
        public async Task<IActionResult> CommunicationBlasts(int communicationId, string sortCol, string sortDir, int? page, int? pageSize)
        {
            var communication = await FindCommunicationByIdAsync(communicationId);
            if (communication == null) return NotFound();

            var items = from z in Rdb.CommunicationBlasts.Include(z => z.Creative)//.Include(z => z.Job)
                        where z.TenantId == TenantId && z.CommunicationId == communicationId                        
                        select z;
            items = ApplyBrowse(
                items, sortCol ?? nameof(Traffk.Bal.Data.Rdb.TraffkTenantModel.CommunicationBlast.CommunicationBlastId), sortDir,
                page, pageSize);
            SetHeroLayoutViewData(communication, CommunicationPageKeys.Blasts);
            return View(ViewNames.CommunicationBlasts, items);
        }

        [ActionName(ActionNames.CommunicationBlast)]
        [Route("Communications/{communicationId}/Blasts/{communicationBlastId}")]
        public async Task<IActionResult> CommunicationBlast(int communicationId, int communicationBlastId)
        {
            var communication = await FindCommunicationByIdAsync(communicationId);
            if (communication == null) return NotFound();
            var item = await Rdb.CommunicationBlasts.FindAsync(communicationBlastId);
            if (item == null) return NotFound();
            SetHeroLayoutViewData(communication, CommunicationPageKeys.Blasts);
            return View(ViewNames.CommunicationBlast, item);
        }

        #endregion

        [HttpDelete]
        [Route("Creatives/{creativeId}/DeleteAttachment")]
        public async Task<IActionResult> RemovePortalAsset(int creativeId, string assetKey)
        {
            var creative = await FindCreativeByIdAsync(creativeId);
            if (creative == null) return NotFound();
            var a = creative.CreativeSettings.Attachments.FirstOrDefault(z => z.Path == assetKey);
            if (a == null) return NotFound();
            creative.CreativeSettings.Attachments.Remove(a);
            Rdb.Update(creative);
            await Rdb.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        [Route("Creatives/delete")]
        public Task<IActionResult> DeleteCreatives() => JsonDeleteFromRdbAsync<Creative, int>(Rdb.Creatives);

        [HttpDelete]
        [Route("Communications/delete")]
        public Task<IActionResult> DeleteCommunications() => JsonDeleteFromRdbAsync<Communication, int>(Rdb.Communications);
    }
}
