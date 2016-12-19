using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraffkPortal.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TraffkPortal.Permissions;
using Traffk.Bal.Permissions;
using RevolutionaryStuff.Core;
using TraffkPortal.Models.MessagingModels;
using Traffk.Bal.Data.Rdb;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System;
using Traffk.Bal.Data;
using Traffk.Bal.Data.Query;
using Traffk.Bal.Data.Ddb.Crm;
using TraffkPortal.Models;
using System.IO;
using Traffk.Bal.Services;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.Messaging)]
    [Route("Messaging")]
    public class MessagingController : BasePageController
    {
        public const string Name = "Messaging";

        public static class ActionNames
        {
            public const string BlastMessageInstances = "BlastMessageInstances";
            public const string BlastMessageSchedule = "BlastMessageSchedule";
            public const string BlastMessageScheduleSubmission = "BlastMessageScheduleSubmission";
            public const string BlastMessageJob = "BlastMessageJob";
            public const string BlastMessageQuery = "BlastMessageQuery";
            public const string BlastSave = "BlastSave";
            public const string BlastMessageSave = "BlastMessageSave";
        }

        public static class ViewNames
        {
            public const string MessageList = nameof(MessagingController.Messages);
            public const string Message = "EditMessage";
            public const string TemplateList = nameof(MessagingController.Templates);
            public const string Template = "EditTemplate";
            public const string BlastList = nameof(MessagingController.Blasts);
            public const string Blast = "Blast";
            public const string BlastMessage = "BlastMessage";
            public const string BlastSchedule = "BlastSchedule";
            public const string BlastJob = "BlastJob";
            public const string BlastInstancesList = "BlastInstancesList";
            public const string BlastQuery = "BlastQuery";
        }

        private readonly BlobStorageServices Blobs;
        private readonly CrmDdbContext Crm;

        public MessagingController(
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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            PopulateViewBagWithTemplateModelSelectListItems();
            base.OnActionExecuting(context);
        }

        private void PopulateViewBagWithTemplateModelSelectListItems()
        {
            var items = Template.ModelTypes.All.OrderBy().ConvertAll(mt => new SelectListItem { Text = mt, Value = mt }).ToList();
            items.Insert(0, new SelectListItem { Text = AspHelpers.NoneDropdownItemText, Value = AspHelpers.NoneDropdownItemValue });
            ViewBag.ModelListItems = items;
        }

        public IActionResult Index()
        {
            return Messages();
        }

        #region Messages

        [Route("Messages")]
        public IActionResult Messages(string sortCol=null, string sortDir=null, int? page=null, int? pageSize=null)
        {
            var messages = Rdb.MessageTemplates
                .Include(w => w.SubjectTemplate).Include(w => w.HtmlBodyTemplate).Include(w => w.TextBodyTemplate)
                .Where(z => z.TenantId == TenantId && z.MessageTemplateTitle != null && z.MessageTemplateTitle != "");
            messages = ApplyBrowse(messages, sortCol??nameof(MessageTemplate.MessageTemplateTitle), sortDir, page, pageSize);
            var model = messages.ConvertAll(z => new MessageItem
            {
                MessageTemplateId = z.MessageTemplateId,
                MessageTemplateTitle = z.MessageTemplateTitle,
                CreatedAtUtc = z.CreatedAtUtc,
                SubjectTemplateId = z.SubjectTemplateId,
                SubjectModelType = z.SubjectTemplate?.ModelType,
                HtmlBodyTemplateId = z.HtmlBodyTemplateId,
                HtmlBodyModelType = z.HtmlBodyTemplate?.ModelType,
                TextBodyTemplateId = z.TextBodyTemplateId,
                TextBodyModelType = z.TextBodyTemplate?.ModelType,
            });
            return View(ViewNames.MessageList, model);
        }

        [Route("Messages/Create")]
        public IActionResult CreateMessage()
        {
            return View(ViewNames.Message, new MessageTemplate());
        }

        [Route("Messages/{id}")]
        public async Task<IActionResult> EditMessage(int id)
        {
            var messageItem = await FindMessageByIdAsync(id);
            if (messageItem == null) return NotFound();
            return View(ViewNames.Message, messageItem);
        }

        private async Task<Template> TemplateMaterialzeThenSet(Template other)
        {
            var code = StringHelpers.TrimOrNull(other.Code);
            if (code == null) return null;
            Template t;
            if (other.TemplateId > 0)
            {
                t = await Rdb.Templates.FirstAsync(z => z.TenantId == TenantId && z.TemplateId == other.TemplateId);
            }
            else
            {
                t = new Template();
                Rdb.Templates.Add(t);                
            }
            if (!string.IsNullOrEmpty(other.TemplateName))
            {
                t.TemplateName = other.TemplateName;
            }
            t.Code = other.Code;
            t.ModelType = other.ModelType == AspHelpers.NoneDropdownItemValue ? null : other.ModelType;
            t.TemplateEngineType = other.TemplateEngineType ?? Template.TemplateEngineTypes.Default;
            return t;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Messages/{id}")]
        public async Task<IActionResult> EditMessage(
            int id,
            [Bind(
            nameof(MessageTemplate.MessageTemplateId),
            nameof(MessageTemplate.MessageTemplateTitle),
            nameof(MessageTemplate.SubjectTemplate),
            nameof(MessageTemplate.HtmlBodyTemplate),
            nameof(MessageTemplate.TextBodyTemplate),
            nameof(MessageTemplate.SubjectTemplate)+"."+nameof(Template.TemplateId),
            nameof(MessageTemplate.SubjectTemplate)+"."+nameof(Template.TemplateName),
            nameof(MessageTemplate.SubjectTemplate)+"."+nameof(Template.Code),
            nameof(MessageTemplate.SubjectTemplate)+"."+nameof(Template.ModelType),
            nameof(MessageTemplate.HtmlBodyTemplate)+"."+nameof(Template.TemplateId),
            nameof(MessageTemplate.HtmlBodyTemplate)+"."+nameof(Template.TemplateName),
            nameof(MessageTemplate.HtmlBodyTemplate)+"."+nameof(Template.Code),
            nameof(MessageTemplate.HtmlBodyTemplate)+"."+nameof(Template.ModelType),
            nameof(MessageTemplate.TextBodyTemplate)+"."+nameof(Template.TemplateId),
            nameof(MessageTemplate.TextBodyTemplate)+"."+nameof(Template.TemplateName),
            nameof(MessageTemplate.TextBodyTemplate)+"."+nameof(Template.Code),
            nameof(MessageTemplate.TextBodyTemplate)+"."+nameof(Template.ModelType)
            )]
            MessageTemplate model)
        {
            if (id != model.MessageTemplateId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                MessageTemplate messageItem;
                if (id > 0)
                {
                    messageItem = await FindMessageByIdAsync(id);
                    if (messageItem == null) return NotFound();
                }
                else
                {
                    messageItem = new MessageTemplate();
                    Rdb.MessageTemplates.Add(messageItem);
                }
                messageItem.MessageTemplateTitle = model.MessageTemplateTitle;
                messageItem.SubjectTemplate = await TemplateMaterialzeThenSet(model.SubjectTemplate);
                messageItem.HtmlBodyTemplate = await TemplateMaterialzeThenSet(model.HtmlBodyTemplate);
                messageItem.TextBodyTemplate = await TemplateMaterialzeThenSet(model.TextBodyTemplate);
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ViewNames.MessageList);
            }
            return View(ViewNames.Message, model);
        }

        private async Task<MessageTemplate> FindMessageByIdAsync(int? id)
        {
            if (id == null) return null;
            return await
                (from z in Rdb.MessageTemplates.Include(w => w.SubjectTemplate).Include(w => w.HtmlBodyTemplate).Include(w => w.TextBodyTemplate)
                 where z.TenantId == this.TenantId && z.MessageTemplateId==id.Value
                 select z).FirstOrDefaultAsync();
        }

        #endregion

        #region Templates


        [Route("Templates")]
        public async Task<IActionResult> Templates(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var templates = from z in Rdb.Templates
                            where z.TenantId == this.TenantId && z.TemplateName != null
                            select z;
            templates = ApplyBrowse(
                templates, sortCol ?? nameof(Template.TemplateName), sortDir, 
                page, pageSize,
                new Dictionary<string, string> { { nameof(Template.CreatedAt), nameof(Template.CreatedAtUtc) } });
            var model = (await templates.ToArrayAsync()).ConvertAll(w => new TemplateModel(w));
            return View(ViewNames.TemplateList, model);
        }

        private async Task<Template> FindTemplateByIdAsync(int? id)
        {
            if (id == null) return null;
            return await
                (from z in Rdb.Templates
                 where z.TenantId == this.TenantId && z.TemplateId == id.Value
                 select z).FirstOrDefaultAsync();
        }

        [Route("Templates/Create")]
        public IActionResult CreateTemplate()
        {
            return View(ViewNames.Template, new TemplateModel(new Template()));
        }

        [Route("Templates/{id}")]
        public async Task<IActionResult> EditTemplate(int id)
        {
            var template = await FindTemplateByIdAsync(id);
            if (template == null) return NotFound();
            return View(ViewNames.Template, new TemplateModel(template));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Templates/{id}")]
        public async Task<IActionResult> EditTemplate(
            int id,
            [Bind(
            nameof(TemplateModel.TemplateName),
            nameof(TemplateModel.TemplateId),
            nameof(TemplateModel.Code),
            nameof(TemplateModel.ModelType)
            )]
            TemplateModel model)
        {
            if (id != model.TemplateId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var t = await TemplateMaterialzeThenSet(model);
                if (t.TemplateId == 0)
                {
                    Rdb.Templates.Add(t);
                }
                await Rdb.SaveChangesAsync();
                return RedirectToAction(nameof(Templates));
            }
            return View(ViewNames.Template, model);
        }

        #endregion

        #region Blasts

        public enum BlastPageKeys
        {
            Background,
            Message,
            Schedule,
            Instances,
            Job,
            Query,
        }


        private void SetHeroLayoutViewData(ZCommunicationBlast blast, BlastPageKeys pageKey)
        {
            ViewData["JobStatus"] = blast.Job?.JobStatus;
            ViewData[AspHelpers.ViewDataKeys.ParentEntityId] = blast.ParentCommunicationBlastId;
            SetHeroLayoutViewData(blast.CommunicationBlastId, blast.CommunicationBlastTitle, pageKey);
        }

        [Route("Blasts/{id}/Instances")]
        [ActionName(ActionNames.BlastMessageInstances)]
        public Task<IActionResult> BlastMessageInstances(int id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            var blasts = Rdb.ZCommunicationBlasts.Include(z => z.Job).Where(z => z.ParentCommunicationBlastId == id).AsQueryable();
            blasts = ApplyBrowse(
                blasts, 
                sortCol ?? nameof(ZCommunicationBlast.CreatedAtUtc), sortDir, 
                page, pageSize,
                new Dictionary<string, string> { { nameof(ZCommunicationBlast.CreatedAt), nameof(ZCommunicationBlast.CreatedAtUtc) } });
            return BlastEntityPage(id, BlastPageKeys.Instances, ViewNames.BlastInstancesList,z=>blasts);
        }

        [Route("Blasts")]
        public IActionResult Blasts(string sortCol, string sortDir, int? startAt, int? page, int? pageSize)
        {
            var blasts = Rdb.ZCommunicationBlasts.Include(z => z.Job).Where(z=>z.ParentCommunicationBlast==null).AsQueryable();
            blasts = ApplyBrowse(
                blasts, 
                sortCol ?? nameof(ZCommunicationBlast.CreatedAtUtc), sortDir, 
                page, pageSize,
                new Dictionary<string, string> { { nameof(ZCommunicationBlast.CreatedAt), nameof(ZCommunicationBlast.CreatedAtUtc) } });
            return View(ViewNames.BlastList, blasts);
        }

        [Route("Blasts/Create")]
        public IActionResult CreateBlast()
        {
            var model = new BlastModel();
            SetHeroLayoutViewData(model.Blast, BlastPageKeys.Background);
            return View(ViewNames.Blast, model);
        }

        [Route("Blasts/{id}")]
        public Task<IActionResult> Blast(int id)
        {
            return BlastEntityPage(id, BlastPageKeys.Background, ViewNames.Blast);
        }

        [Route("Blasts/{id}/Message")]
        public Task<IActionResult> BlastMessage(int id)
        {
            return BlastEntityPage(id, BlastPageKeys.Message, ViewNames.BlastMessage, b=> 
            {
                var model = new MessageTemplateModel(b.MessageTemplate);
                var cloubBlobs = b.MessageTemplate.GetFileAttachmentInfosAsync(Blobs).ExecuteSynchronously();
                model.AttachmentAssets = cloubBlobs.ConvertAll(cb => new AssetPreviewModel(cb, "removeAttachment"));
                return model;
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Blasts/{id}/Message")]
        [ActionName(ActionNames.BlastMessageSave)]
        public async Task<IActionResult> BlastMessageSave(
            int id,
            [Bind(
            nameof(MessageTemplateModel.TextBodyCode),
            nameof(MessageTemplateModel.SubjectCode),
            nameof(MessageTemplateModel.HtmlBodyCode),
            nameof(MessageTemplateModel.TextBodyCode),
            nameof(MessageTemplateModel.ModelType),
            nameof(MessageTemplateModel.AttachmentAssets),
            nameof(MessageTemplateModel.NewAttachments)
            )]
            MessageTemplateModel model)
        {
            var blast = await FindBlastByIdAsync(id);
            if (blast == null) return NotFound();
            SetHeroLayoutViewData(blast, BlastPageKeys.Background);
            if (ModelState.IsValid)
            {
                blast.MessageTemplate.SubjectTemplate = blast.MessageTemplate.SubjectTemplate ?? new Template();
                blast.MessageTemplate.HtmlBodyTemplate = blast.MessageTemplate.HtmlBodyTemplate ?? new Template();
                blast.MessageTemplate.TextBodyTemplate = blast.MessageTemplate.TextBodyTemplate ?? new Template();
                blast.MessageTemplate.SubjectTemplate.TemplateEngineType = Template.TemplateEngineTypes.Default;
                blast.MessageTemplate.SubjectTemplate.ModelType = model.ModelType;
                blast.MessageTemplate.SubjectTemplate.Code = model.SubjectCode;
                blast.MessageTemplate.HtmlBodyTemplate.TemplateEngineType = Template.TemplateEngineTypes.Default;
                blast.MessageTemplate.HtmlBodyTemplate.ModelType = model.ModelType;
                blast.MessageTemplate.HtmlBodyTemplate.Code = model.HtmlBodyCode;
                blast.MessageTemplate.TextBodyTemplate.TemplateEngineType = Template.TemplateEngineTypes.Default;
                blast.MessageTemplate.TextBodyTemplate.ModelType = model.ModelType;
                blast.MessageTemplate.TextBodyTemplate.Code = model.TextBodyCode;
                if (model.NewAttachments != null)
                {
                    foreach (var newAttachment in model.NewAttachments)
                    {
                        await Blobs.StoreFileAsync(false, BlobStorageServices.Roots.Portal, newAttachment, blast.MessageTemplate.AttachmentPrefix + Path.GetFileName(newAttachment.FileName), false);
                    }
                }
                await Rdb.SaveChangesAsync();
                ViewBag.ShowAlertBlastMessageSaved = true;
                return RedirectToAction(ViewNames.Blast);
            }
            return View(ViewNames.BlastMessage, model);
        }

        [HttpDelete]
        [Route("Blasts/{id}/Message/DeleteAttachment")]
        public async Task<IActionResult> RemovePortalAsset(int id, string assetKey)
        {
            var blast = await FindBlastByIdAsync(id);
            if (blast == null) return NotFound();
            var cloubBlob = Blobs.GetFileInfosAsync(false, BlobStorageServices.Roots.Portal, blast.MessageTemplate.AttachmentPrefix).ExecuteSynchronously().FirstOrDefault(b => b.Name.EndsWith(assetKey));
            if (cloubBlob == null) return NotFound();
            await cloubBlob.DeleteAsync();
            return NoContent();
        }

        [Route("Blasts/{id}/Job")]
        [ActionName(ActionNames.BlastMessageJob)]
        public Task<IActionResult> BlastJob(int id)
        {
            return BlastEntityPage(id, BlastPageKeys.Message, ViewNames.BlastJob, b => b.Job);
        }

        [Route("Blasts/{id}/Query")]
        [ActionName(ActionNames.BlastMessageQuery)]
        public async Task<IActionResult> BlastQuery(int id)
        {
            ContactQueryModel.PopulateViewBagWithOperatorSelectListItems(ViewBag);
            var model = new ContactQueryModel();
            await model.Populate(Rdb, Crm, false, CollectionNames.Contacts, CollectionNames.Eligibility, CollectionNames.Scores, CollectionNames.Demographics, CollectionNames.Pcp);
            return await BlastEntityPage(id, BlastPageKeys.Query, ViewNames.BlastQuery, b => model);
        }

        private async Task<IActionResult> BlastEntityPage(int id, BlastPageKeys pageKey, string viewName, Func<ZCommunicationBlast, object> createModel=null)
        {
            var blast = await FindBlastByIdAsync(id);
            if (blast == null) return NotFound();
            SetHeroLayoutViewData(blast, pageKey);
            var model = createModel?.Invoke(blast) ?? new BlastModel(blast);
            return View(viewName, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Blasts/{id}")]
        [ActionName(ActionNames.BlastSave)]
        public async Task<IActionResult> BlastSave(
            int id,
            [Bind(
            nameof(BlastModel.Blast),
            nameof(BlastModel.Blast)+"."+nameof(ZCommunicationBlast.CommunicationBlastId),
            nameof(BlastModel.Blast)+"."+nameof(ZCommunicationBlast.CampaignName),
            nameof(BlastModel.Blast)+"."+nameof(ZCommunicationBlast.CommunicationBlastTitle),
            nameof(BlastModel.Blast)+"."+nameof(ZCommunicationBlast.CommunicationMedium),
            nameof(BlastModel.Blast)+"."+nameof(ZCommunicationBlast.TopicName),
            nameof(BlastModel.TextBodyCode)
            )]
            BlastModel model)
        {
            if (id != model.Blast.CommunicationBlastId)
            {
                return NotFound();
            }
            SetHeroLayoutViewData(model.Blast, BlastPageKeys.Background);
            if (ModelState.IsValid)
            {
                ZCommunicationBlast blast;
                if (id > 0)
                {
                    blast = await FindBlastByIdAsync(id);
                    if (blast == null) return NotFound();
                }
                else
                {
                    blast = new ZCommunicationBlast
                    {
                        MessageTemplate = new MessageTemplate
                        {
                            MessageTemplateTitle = ""
                        }
                    };
                    Rdb.ZCommunicationBlasts.Add(blast);
                }
                blast.CampaignName = model.Blast.CampaignName;
                blast.CommunicationBlastTitle = model.Blast.CommunicationBlastTitle;
                blast.CommunicationMedium = model.Blast.CommunicationMedium;
                blast.TopicName = model.Blast.TopicName;
                await Rdb.SaveChangesAsync();
                ViewBag.ShowAlertBlastSaved = true;
                return RedirectToAction(ViewNames.Blast);
            }
            return View(ViewNames.Blast, model);
        }

        [Route("Blasts/{id}/Schedule")]
        [ActionName(ActionNames.BlastMessageSchedule)]
        public Task<IActionResult> BlastSchedule(int id)
        {
            return BlastEntityPage(id, BlastPageKeys.Schedule, ViewNames.BlastSchedule, b => (b.CommunicationBlastSettings?.Recurrence ?? new RecurrenceSettings()));
        }

        [ActionName(ActionNames.BlastMessageScheduleSubmission)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Blasts/{id}/Schedule")]
        public async Task<IActionResult> BlastMessageSchedule(
            int id,
            RecurrenceSettings model)
        {
            var blast = await FindBlastByIdAsync(id);
            if (blast == null) return NotFound();
            SetHeroLayoutViewData(blast, BlastPageKeys.Background);
            if (ModelState.IsValid)
            {
                blast.CommunicationBlastSettings.Recurrence = model;
                await Rdb.AddNextScheduledBlasts(id, false, true);
                await Rdb.SaveChangesAsync();
                return RedirectToAction(ViewNames.Blast);
            }
            return View(ViewNames.BlastSchedule, model);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Blasts/{id}/Message")]
        public async Task<IActionResult> BlastMessage(
            int id,
            [Bind(
            nameof(BlastModel.Blast),
            nameof(BlastModel.Blast)+"."+nameof(CommunicationBlast.CommunicationBlastId),
            nameof(BlastModel.ModelType),
            nameof(BlastModel.SubjectCode),
            nameof(BlastModel.TextBodyCode),
            nameof(BlastModel.HtmlBodyCode)
            )]
            BlastModel model)
        {
            if (id != model.Blast.CommunicationBlastId)
            {
                return NotFound();
            }
            SetHeroLayoutViewData(model.Blast, BlastPageKeys.Background);
            if (ModelState.IsValid)
            {
                var blast = await FindBlastByIdAsync(id);
                if (blast == null) return NotFound();
                var mt = blast.MessageTemplate;
                mt.SubjectTemplate = mt.SubjectTemplate ?? new Template();
                mt.HtmlBodyTemplate = mt.HtmlBodyTemplate ?? new Template();
                mt.TextBodyTemplate = mt.TextBodyTemplate ?? new Template();
                mt.SubjectTemplate.TemplateEngineType = Template.TemplateEngineTypes.Default;
                mt.SubjectTemplate.ModelType = model.ModelType;
                mt.SubjectTemplate.Code = model.SubjectCode;
                mt.HtmlBodyTemplate.TemplateEngineType = Template.TemplateEngineTypes.Default;
                mt.HtmlBodyTemplate.ModelType = model.ModelType;
                mt.HtmlBodyTemplate.Code = model.HtmlBodyCode;
                mt.TextBodyTemplate.TemplateEngineType = Template.TemplateEngineTypes.Default;
                mt.TextBodyTemplate.ModelType = model.ModelType;
                mt.TextBodyTemplate.Code = model.TextBodyCode;
                await RDB.SaveChangesAsync();
                return RedirectToAction(ViewNames.BlastList);
            }
            return View(ViewNames.Blast, model);
        }
        */

        private async Task<ZCommunicationBlast> FindBlastByIdAsync(int? id)
        {
            if (id == null) return null;
            return await
                (from z in Rdb.ZCommunicationBlasts
                    .Include(z => z.MessageTemplate)
                    .Include(z => z.Job)
                    .Include(z => z.MessageTemplate.SubjectTemplate)
                    .Include(z => z.MessageTemplate.HtmlBodyTemplate)
                    .Include(z => z.MessageTemplate.TextBodyTemplate)
                 where z.TenantId == this.TenantId && z.CommunicationBlastId == id.Value
                 select z).FirstOrDefaultAsync();
        }


        #endregion
    }
}
