using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TraffkPortal.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TraffkPortal.Permissions;
using Traffk.Bal.Permissions;
using Traffk.Bal.Data.Rdb;
using System.Collections.Generic;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using RevolutionaryStuff.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.Messaging)]
    [Route("Communications")]
    public class CommunicationsController : BasePageController
    {
        public const string Name = "Communications";

        public static class ActionNames
        {
            public const string CommunicationsList = "Communications";
            public const string CommunicationCreate = "CommunicationCreate";
            public const string CommunicationDetails = "CommunicationDetails";
            public const string CommunicationSave = "CommunicationSave";
            public const string CommunicationCreative = "CommunicationCreative";
            public const string CommunicationQuery = "CommunicationQuery";
            public const string CommunicationSchedule = "CommunicationSchedule";
            public const string CommunicationBlasts = "CommunicationBlasts";

            public const string CreativesList = "Creatives";
            public const string CreativeDetails = "CreativeDetails";
            public const string CreativeSave = "CreativeSave";
            public const string CreativeCreate = "CreativeCreate";
        }

        public static class ViewNames
        {
            public const string CommunicationsList = "Communications";
            public const string CommunicationDetails = "CommunicationDetails";
            public const string CreativesList = "Creatives";
            public const string CreativeDetails = "CreativeDetails";
        }

        private readonly BlobStorageServices Blobs;
        private readonly CrmDdbContext Crm;

        public CommunicationsController(
            BlobStorageServices blobs,
            CrmDdbContext crm,
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
            )
            : base(AspHelpers.MainNavigationPageKeys.Messaging, db, current, loggerFactory)
        {
            Blobs = blobs;
            Crm = crm;
        }

        #region Communications

        public enum CommunicationPageKeys
        {
            Background,
            Message,
            Schedule,
            Blasts,
            Query,
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

        private Task<Communication> FindCommunicationByIdAsync(int id) => Rdb.Communications.FindAsync(id);




        [Route("Create")]
        [ActionName(ActionNames.CommunicationCreate)]
        public IActionResult CommunicationCreate()
        {
            var item = new Communication();
            SetHeroLayoutViewData(item, CommunicationPageKeys.Background);
            return View(ViewNames.CommunicationDetails, item);
        }

        [Route("{id}")]
        [ActionName(ActionNames.CommunicationDetails)]
        public async Task<IActionResult> CommunicationEdit(int id)
        {
            var item = await FindCommunicationByIdAsync(id);
            if (item == null) return NotFound();
            SetHeroLayoutViewData(item, CommunicationPageKeys.Background);
            return View(ViewNames.CommunicationDetails, item);
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
                Communication communication = await FindCommunicationByIdAsync(id);
                if (communication == null)
                {
                    communication = new Communication();
                    Rdb.Communications.Add(communication);
                }
                communication.CommunicationTitle = model.CommunicationTitle;
                communication.CampaignName = model.CampaignName;
                communication.TopicName = model.TopicName;
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ActionNames.CreativesList);
            }
            return await CommunicationEdit(id);
        }

        #endregion

        #region Creatives

        private void PopulateViewBagWithTemplateModelSelectListItems()
        {
            var items = Template.ModelTypes.All.OrderBy().ConvertAll(mt => new SelectListItem { Text = mt, Value = mt }).ToList();
            items.Insert(0, new SelectListItem { Text = AspHelpers.NoneDropdownItemText, Value = AspHelpers.NoneDropdownItemValue });
            ViewBag.ModelListItems = items;
        }

        [ActionName(ActionNames.CreativesList)]
        [Route("Creatives")]
        public IActionResult Creatives(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var items = from z in Rdb.Creatives
                            where z.TenantId == TenantId && z.CreativeTitle != null
                            select z;
            items = ApplyBrowse(
                items, sortCol ?? nameof(Creative.CreativeTitle), sortDir,
                page, pageSize);
            return View(ViewNames.CreativesList, items);
        }

        private Task<Creative> FindCreativeByIdAsync(int id) => Rdb.Creatives.FindAsync(id);

        [Route("Creatives/Create")]
        [ActionName(ActionNames.CreativeCreate)]
        public IActionResult CreativeCreate()
        {
            PopulateViewBagWithTemplateModelSelectListItems();
            return View(ViewNames.CreativeDetails, new Creative());
        }

        [Route("Creatives/{id}")]
        [ActionName(ActionNames.CreativeDetails)]
        public async Task<IActionResult> CreativeEdit(int id)
        {
            var creative = await FindCreativeByIdAsync(id);
            if (creative == null) return NotFound();
            PopulateViewBagWithTemplateModelSelectListItems();
            return View(ViewNames.CreativeDetails, creative);
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
            nameof(Creative.ModelType),
            nameof(Creative.CreativeSettings),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailSubject),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailHtmlBody),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.EmailTextBody),
            nameof(Creative.CreativeSettings)+"."+nameof(CreativeSettings.TextMessageBody)
            )]
            Creative model)
        {
            if (ModelState.IsValid)
            {
                Creative creative = await FindCreativeByIdAsync(id);
                if (creative==null)
                {
                    creative = new Creative();
                    Rdb.Creatives.Add(creative);
                }
                creative.CreativeTitle = model.CreativeTitle;
                creative.ModelType = model.ModelType;
                creative.TemplateEngineType = model.TemplateEngineType;
                creative.CreativeSettings.EmailSubject = model.CreativeSettings.EmailSubject;
                creative.CreativeSettings.EmailHtmlBody = model.CreativeSettings.EmailHtmlBody;
                creative.CreativeSettings.EmailTextBody = model.CreativeSettings.EmailTextBody;
                creative.CreativeSettings.TextMessageBody = model.CreativeSettings.TextMessageBody;
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ActionNames.CreativesList);
            }
            PopulateViewBagWithTemplateModelSelectListItems();
            return View(ViewNames.CreativeDetails, model);
        }

        #endregion
    }
}
