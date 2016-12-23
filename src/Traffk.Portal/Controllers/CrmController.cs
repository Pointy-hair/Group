using RevolutionaryStuff.Core;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TraffkPortal.Permissions;
using Traffk.Bal.Permissions;
using TraffkPortal.Services;
using Microsoft.Extensions.Logging;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Data.Rdb;
using System.Threading.Tasks;
using TraffkPortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TraffkPortal.Models.CrmModels;
using System;
using Traffk.Bal.Data.Ddb;
using System.Collections.Generic;
using Traffk.Bal;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.CustomerRelationshipData)]
    [Route("Crm")]
    public class CrmController : BasePageController
    {
        public const string Name = "Crm";

        public enum PageKeys
        {
            Background,
            Messages,
            Notes,
            Relationships,
            #region Health
            CareAlerts,
            Demographics,
            Eligibility,
            HighCostDiagnosis,
            HistoricalScores,
            MedicalClaims,
            MemberPCP,
            Participation,
            Pharmacy,
            QualityMetrics,
            Scores,
            Visit,
            #endregion
        }

        public static class ViewNames
        {
            public static class Health
            {
                public const string CareAlert = "ContactCareAlert";
                public const string CareAlertsList = "ContactCareAlertsList";
                public const string Demographic = "ContactDemographic";
                public const string Eligiblity = "ContactEligiblity";
                public const string HighCostDiagnosis = "ContactHighCostDiagnosis";
                public const string HistoricalScore = "ContactHistoricalScore";
                public const string HistoricalScoreList = "ContactHistoricalScoresList";
                public const string MedicalClaim = "ContactMedicalClaimDetail";
                public const string MedicalClaimsList = "ContactMedicalClaimsList";
                public const string MemberPcp = "ContactMemberPcp";
                public const string Participation = "ContactParticipation";
                public const string ParticipationList = "ContactParticipationList";
                public const string Pharmacy = "ContactPharmacyItemDetail";
                public const string PharmacyList = "ContactPharmacyList";
                public const string QualityMetric = "ContactQualityMetric";
                public const string QualityMetricsList = "ContactQualityMetricsList";
                public const string Score = "ContactScore";
                public const string Visit = "ContactVisit";
                public const string VisitList = "ContactVisitsList";
            }
        }

        public static class ActionNames
        {
            public const string ContactList = "Index";
            public const string ContactDelete = "Delete";
            public const string ContactCreate = "Create";
            public const string ContactBackground = "ContactBackground";
            public const string ContactNotes = "ContactNotes";
            public const string ContactMessages = "ContactMessages";
            public const string ContactRelationships = "ContactRelationships";
            public const string CreateNote = "CreateNote";
            public const string SendDirectMessage = "SendDirectMessage";

            public static class Health
            {
                public const string CareAlert = "ContactCareAlertDetail";
                public const string CareAlertsList = "ContactCareAlertsList";
                public const string Demographic = "ContactDemographicDetail";
                public const string Eligiblity = "ContactEligiblityDetail";
                public const string HighCostDiagnosis = "ContactHighCostDiagnosisDetail";
                public const string HistoricalScore = "ContactHistoricalScoreDetail";
                public const string HistoricalScoreList = "ContactHistoricalScoreList";
                public const string MedicalClaim = "ContactMedicalClaimDetail";
                public const string MedicalClaimsList = "ContactMedicalClaimsList";
                public const string MemberPcp = "ContactMemberPcpDetail";
                public const string Participation = "ContactParticipationDetail";
                public const string ParticipationList = "ContactParticipationList";
                public const string Pharmacy = "ContactPharmacyItemDetail";
                public const string PharmacyList = "ContactPharmacyList";
                public const string QualityMetric = "ContactQualityMetricDetail";
                public const string QualityMetricsList = "ContactQualityMetricsList";
                public const string Score = "ContactScoreDetail";
                public const string Visit = "ContactVisitDetail";
                public const string VisitList = "ContactVisitsList";
            }
        }

        private void SetHeroLayoutViewData(Contact contact, PageKeys pageKey)
        {
            ViewData["ContactId"] = contact.ContactId;
            ViewData["ContactName"] = contact.FullName;
            ViewData["ContactType"] = contact.ContactType;
            ViewData["ContactEmailAddress"] = contact.PrimaryEmail;
            SetHeroLayoutViewData(contact.ContactId, contact.FullName, pageKey, contact.ContactType);
        }

        private readonly CrmDdbContext Crm;
        private readonly IEmailSender EmailSender;

        public CrmController(
            IEmailSender emailSender,
            CrmDdbContext crm,
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
            )
            : base(AspHelpers.MainNavigationPageKeys.CRM, db, current, loggerFactory)
        {
            EmailSender = emailSender;
            Crm = crm;
        }

        [Route("Contacts")]
        [ActionName(ActionNames.ContactList)]
        public async Task<IActionResult> Index(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var contacts = Rdb.Contacts.AsQueryable().Where(c => c.TenantId == this.TenantId);
            contacts = ApplyFilters<Contact, ContactAllFields>(contacts, c => c.ContactType, c => c.Gender);
            var counts = await Rdb.GetFieldCountsAsync<ContactAllFields>(c => c.ContactType, c => c.Gender);
            ViewBag.ListingFilters = new ListingFilters(counts, HttpContext.Request.Path.ToUriComponent(), HttpContext.Request.QueryString.Value);
            contacts = ApplyBrowse(contacts, sortCol ?? nameof(Contact.FullName), sortDir, page, pageSize);
            return View(contacts);
        }

        [Route("Contacts/{id}/Crm")]
        public async Task<IActionResult> Details(long id)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();

            return View(contact);
        }

        // GET: Users/Delete/5
        [Route("Contacts/{id}/Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();

            return View(contact);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ActionName(ActionNames.ContactDelete)]
        [ValidateAntiForgeryToken]
        [Route("Contacts/{id}/Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            Rdb.Contacts.Remove(contact);
            await Rdb.SaveChangesAsync();
            return RedirectToIndex();
        }

        [ActionName(ActionNames.ContactCreate)]
        [Route("Contacts/Create/{contactTypeTemplate}")]
        public IActionResult Create(string contactTypeTemplate)
        {
            Contact contact;
            switch (contactTypeTemplate)
            {
                case Contact.ContactTypes.Person:
                case null:
                case "":
                    contact = new Person();
                    break;
                case Contact.ContactTypes.Organization:
                    contact = new Organization();
                    break;
                default:
                    throw new UnexpectedSwitchValueException(contactTypeTemplate);
            }
//            var contact = new Contact { ContactType = contactType };
            return View(nameof(ContactBackground), new ContactModel(contact));
        }

        private async Task SetViewBagEmailTemplateItems()
        {
            var communications = await
            Rdb.SystemCommunications.Include(z => z.MessageTemplate).Where(z =>
              z.TenantId == this.TenantId && z.CommunicationPurpose == SystemCommunication.CommunicationPurposes.DirectMessage && z.CommunicationMedium == SystemCommunication.CommunicationMediums.Email).OrderBy(z => z.MessageTemplate.MessageTemplateTitle).ToListAsync();
            ViewBag.EmailTemplateItems = communications.ConvertAll(c => new SelectListItem { Text = c.MessageTemplate.MessageTemplateTitle, Value = c.SystemCommunicationId.ToString() });
        }

        [HttpPost]
        [PermissionAuthorize(PermissionNames.DirectMessaging)]
        [ActionName(ActionNames.SendDirectMessage)]
        [Route("Contacts/{id}/SendDirectMessage")]
        public async Task<IActionResult> SendDirectMessage(
            long id,
            int templateId,
            string subject,
            string body)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            var model = Template.ModelTypes.CreateSimpleContentModel(subject, body);
            await EmailSender.SendEmailCommunicationAsync(templateId, model, contact.PrimaryEmail, contact.FullName, contact.ContactId);
            return Ok();
        }

        [HttpPost]
        [PermissionAuthorize(PermissionNames.DirectMessaging)]
        [ActionName(ActionNames.CreateNote)]
        [Route("Contacts/{id}/CreateNote")]
        public async Task<IActionResult> CreateNote(
            long id,
            string parentNoteId,
            string content)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            var note = new Note { Content = content, CreatedAtUtc = DateTime.UtcNow, CreatedByUserId = Current.User.Id };
            if (parentNoteId == null)
            {
                contact.ContactDetails.Notes.Add(note);
            }
            else
            {
                var parent = contact.FindNoteById(parentNoteId);
                if (parent == null) return NotFound();
                parent.Children = parent.Children ?? new List<Note>();
                parent.Children.Add(note);
            }
            Rdb.Update(contact);
            await Rdb.SaveChangesAsync();
            return Ok();
        }

        [ActionName(ActionNames.ContactBackground)]
        [Route("Contacts/{id}/Background")]
        public Task<IActionResult> ContactBackground(long id)
        {
            return RawContactRecordInfo(id, PageKeys.Background, nameof(ContactBackground));
        }

        [ActionName(ActionNames.ContactNotes)]
        [Route("Contacts/{id}/Notes")]
        public Task<IActionResult> ContactNotes(long id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            return RawContactRecordInfo(id, PageKeys.Notes, nameof(ContactNotes), m =>
            {
                m.Contact.ContactDetails.Notes = ApplyBrowse(m.Contact.ContactDetails.Notes.AsQueryable(), sortCol ?? nameof(Note.CreatedAtUtc), sortDir, page, pageSize).ToList();
            });
        }

#if false
        [ActionName(ActionNames.ContactRelationships)]
        [Route("Contacts/{id}/Relationships")]
        public async Task<IActionResult> ContactRelationships(string id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, PageKeys.Relationships);
            var relatedContactIds = contact.Relations.ToSet(z => z.RelatedContactId);
            var relatedContactById = Crm.Contacts.Where(z => relatedContactIds.Contains(z.Id)).ToDictionary(z => z.Id);
            var model = new ContactModel(contact);
            await SetViewBagEmailTemplateItems();
            return View(model);
        }
#endif

        [ActionName(ActionNames.ContactMessages)]
        [Route("Contacts/{id}/Messages")]
        public Task<IActionResult> ContactMessages(long id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            return RawContactRecordInfo(id, PageKeys.Messages, nameof(ContactMessages), m =>
            {
                var sid = id.ToString();
                var communicationLogs = Crm.CommunicationLogs.AsQueryable().Where(z => z.TenantId == TenantId && z.RecipientContactId == sid);
                communicationLogs = ApplyBrowse(communicationLogs, sortCol ?? nameof(CommunicationLog.CreatedAtUtc), sortDir, page, pageSize);
                m.CommunicationLogs = communicationLogs.ToArray();
            });
        }

        private async Task<IActionResult> RawContactRecordInfo(long id, PageKeys pageKey, string viewName, Action<ContactModel> preView = null)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, pageKey);
            var model = new ContactModel(contact);
            if (contact.ContactId == 0)
            {
                ViewData.SetTitleAndHeading($"Create {model.Contact.ContactType.RightOf("/", true)}");
            }
            preView?.Invoke(model);
            await SetViewBagEmailTemplateItems();
            return View(viewName, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(ActionNames.ContactBackground)]
        [Route("Contacts/{id}/Background")]
        public async Task<IActionResult> ContactBackground(
            long id,
            [Bind(
            nameof(ContactModel.Contact),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactId),
            nameof(ContactModel.Contact)+"."+nameof(Contact.FullName),
            nameof(ContactModel.Contact)+"."+nameof(Contact.PrimaryEmail),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactType),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactDetails),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactDetails)+"."+nameof(Contact.ContactDetails_.Tags),
            nameof(ContactModel.Contact)+"."+nameof(Person.DateOfBirth),
            nameof(ContactModel.Contact)+"."+nameof(Person.Gender),
            nameof(ContactModel.Contact)+"."+nameof(Person.Prefix),
            nameof(ContactModel.Contact)+"."+nameof(Person.FirstName),
            nameof(ContactModel.Contact)+"."+nameof(Person.MiddleName),
            nameof(ContactModel.Contact)+"."+nameof(Person.LastName),
            nameof(ContactModel.Contact)+"."+nameof(Person.Suffix),
            nameof(ContactModel.CommunicationLogs),
            nameof(ContactModel.Optings)
            )]
            ContactModel model)
        {
            if (id != model.Contact.ContactId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var modelContact = model.Contact;
                Contact contact;
                if (id>0)
                {
                    contact = await FindContactByIdAsync(id);
                    if (contact == null) return NotFound();
                    Rdb.Update(contact);
                }
                else
                {
                    contact = Contact.Create(modelContact.ContactType);
                    Rdb.Contacts.Add(contact);
                }
                contact.FullName = modelContact.FullName;
                contact.PrimaryEmail = modelContact.PrimaryEmail;
                contact.ContactType = modelContact.ContactType;
                contact.ContactDetails.Tags = modelContact.ContactDetails.Tags;
                if (modelContact.IsPerson)
                {
                    contact.AsPerson.Gender = modelContact.AsPerson.Gender;
                    contact.AsPerson.DateOfBirth = modelContact.AsPerson.DateOfBirth;
                    contact.AsPerson.Prefix = modelContact.AsPerson.Prefix;
                    contact.AsPerson.FirstName = modelContact.AsPerson.FirstName;
                    contact.AsPerson.MiddleName = modelContact.AsPerson.MiddleName;
                    contact.AsPerson.LastName = modelContact.AsPerson.LastName;
                    contact.AsPerson.Suffix = modelContact.AsPerson.Suffix;
                }
                else if (modelContact.IsOrganization)
                {
                    Stuff.Noop();
                }
                await Rdb.SaveChangesAsync();
                return RedirectToIndex();
            }
            await SetViewBagEmailTemplateItems();
            return View(model);
        }

        private async Task<IActionResult> ContactHealthItemDetail<THealthItem>(long contactId, PageKeys pageKey, string viewName, Func<Contact, IQueryable<THealthItem>> createModel) where THealthItem : ITraffkTenanted
        {
            var contact = await FindContactByIdAsync(contactId);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, pageKey);
            var items = createModel(contact);
            items = items.Where(i => i.TenantId == this.TenantId);
            return View(viewName, items.FirstOrDefault());
        }

        private Task<IActionResult> ContactHealthItemDetail<THealthItem>(long contactId, PageKeys pageKey, string viewName, Func<long, IQueryable<THealthItem>> createModel) where THealthItem : ITraffkTenanted
            => ContactHealthItemDetail(contactId, pageKey, viewName, c => createModel(c.ContactId));

        private async Task<IActionResult> ContactHealthItemList<THealthItem>(long contactId, string sortCol, string sortDir, int? page, int? pageSize, PageKeys pageKey, string viewName, Func<Contact, IQueryable<THealthItem>> createModel, string defaultSortCol = null) where THealthItem : ITraffkTenanted
        {
            var contact = await FindContactByIdAsync(contactId);
            if (contact == null) return NotFound();

            SetHeroLayoutViewData(contact, pageKey);
            var items = createModel(contact);
            items = items.Where(i => i.TenantId == this.TenantId);
            items = ApplyBrowse(items, sortCol ?? defaultSortCol, sortDir, page, pageSize);
            return View(viewName, items);
        }

        private Task<IActionResult> ContactHealthItemList<THealthItem>(long contactId, string sortCol, string sortDir, int? page, int? pageSize, PageKeys pageKey, string viewName, Func<long, IQueryable<THealthItem>> createModel, string defaultSortCol = null) where THealthItem : ITraffkTenanted
            => ContactHealthItemList<THealthItem>(contactId, sortCol, sortDir, page, pageSize, pageKey, viewName, c => createModel(c.ContactId), defaultSortCol);

        [ActionName(ActionNames.Health.CareAlert)]
        [Route("Contacts/{id}/CareAlerts/{recordId}")]
        public Task<IActionResult> ContactCareAlert(long id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.CareAlerts, ViewNames.Health.CareAlert, mid => Rdb.CareAlerts.Where(z => z.ContactId == mid && z.dw_record_id == recordId));

        [ActionName(ActionNames.Health.CareAlertsList)]
        [Route("Contacts/{id}/CareAlerts")]
        public Task<IActionResult> ContactCareAlertsList(long id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.CareAlerts, ViewNames.Health.CareAlertsList, mid => Rdb.CareAlerts.Where(z => z.ContactId == mid), nameof(CareAlert.care_alert_startDate));

        [ActionName(ActionNames.Health.HistoricalScore)]
        [Route("Contacts/{id}/HistoricalScores/{recordId}")]
        public Task<IActionResult> ContactHistoricalScore(long id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.HistoricalScores, ViewNames.Health.HistoricalScore, mid => Rdb.HistoricalScores.Where(z => z.ContactId == mid && z.dw_record_id==recordId));

        [ActionName(ActionNames.Health.HistoricalScoreList)]
        [Route("Contacts/{id}/HistoricalScores")]
        public Task<IActionResult> ContactHistoricalScoreList(long id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.HistoricalScores, ViewNames.Health.HistoricalScoreList, mid => Rdb.HistoricalScores.Where(z => z.ContactId == mid), nameof(HistoricalScore.score_end_date));

        [ActionName(ActionNames.Health.Demographic)]
        [Route("Contacts/{id}/Demographics")]
        public Task<IActionResult> ContactDemographics(long id)
            => ContactHealthItemDetail(id, PageKeys.Demographics, ViewNames.Health.Demographic, mid => Rdb.Demographics.Where(z => z.ContactId == mid));

        [ActionName(ActionNames.Health.Score)]
        [Route("Contacts/{id}/Scores")]
        public Task<IActionResult> ContactScores(long id)
            => ContactHealthItemDetail(id, PageKeys.Scores, ViewNames.Health.Score, mid => Rdb.Scores.Where(z => z.ContactId == mid));

        [ActionName(ActionNames.Health.MemberPcp)]
        [Route("Contacts/{id}/PcP")]
        public Task<IActionResult> ContactPcp(long id)
            => ContactHealthItemDetail(id, PageKeys.MemberPCP, ViewNames.Health.MemberPcp, mid => Rdb.MemberPCP.Where(z => z.ContactId == mid));

        [ActionName(ActionNames.Health.Eligiblity)]
        [Route("Contacts/{id}/Eligibility")]
        public Task<IActionResult> ContactEligiblity(long id)
            => ContactHealthItemDetail(id, PageKeys.Eligibility, ViewNames.Health.Eligiblity, mid => Rdb.Eligibility.Where(z => z.ContactId == mid));

        [ActionName(ActionNames.Health.HighCostDiagnosis)]
        [Route("Contacts/{id}/HighCostDiagnosis")]
        public Task<IActionResult> ContactHighCostDiagnosis(long id)
            => ContactHealthItemDetail(id, PageKeys.HighCostDiagnosis, ViewNames.Health.HighCostDiagnosis, mid => Rdb.HighCostDiagnosis.Where(z => z.ContactId == mid));

        [ActionName(ActionNames.Health.Participation)]
        [Route("Contacts/{id}/Participation/{recordId}")]
        public Task<IActionResult> ContactParticipation(long id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.Participation, ViewNames.Health.Participation, mid => Rdb.Participation.Where(z => z.ContactId == mid && z.dw_record_id == recordId));

        [ActionName(ActionNames.Health.ParticipationList)]
        [Route("Contacts/{id}/Participation")]
        public Task<IActionResult> ContactParticipationList(long id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Participation, ViewNames.Health.ParticipationList, mid => Rdb.Participation.Where(z => z.ContactId == mid), nameof(Participation.program_end_date));

        [ActionName(ActionNames.Health.Visit)]
        [Route("Contacts/{id}/Visits/{recordId}")]
        public Task<IActionResult> ContactVisit(long id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.Visit, ViewNames.Health.Visit, mid => Rdb.Visits.Where(z => z.ContactId == mid && z.dw_record_id == recordId));

        [ActionName(ActionNames.Health.VisitList)]
        [Route("Contacts/{id}/Visits")]
        public Task<IActionResult> ContactVisitsList(long id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Visit, ViewNames.Health.VisitList, mid => Rdb.Visits.Where(z => z.ContactId == mid), nameof(Visit.mbr_end_date));

        [ActionName(ActionNames.Health.QualityMetric)]
        [Route("Contacts/{id}/QualityMetrics/{recordId}")]
        public Task<IActionResult> ContactQualityMetric(long id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.QualityMetrics, ViewNames.Health.QualityMetric, mid => Rdb.QualityMetrics.Where(z => z.ContactId==mid));

        [ActionName(ActionNames.Health.QualityMetricsList)]
        [Route("Contacts/{id}/QualityMetrics")]
        public Task<IActionResult> ContactQualityMetricsList(long id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            return ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.QualityMetrics, ViewNames.Health.QualityMetricsList, delegate (long mid)
            {
                var e = Rdb.Eligibility.FirstOrDefault(z => z.ContactId == mid && z.TenantId == this.TenantId);
                return Rdb.QualityMetrics.Where(z => z.dw_member_id == e.dw_member_id);
            }, nameof(QualityMetric.EndDate));
        }

        [ActionName(ActionNames.Health.MedicalClaimsList)]
        [Route("Contacts/{id}/MedicalClaims")]
        public Task<IActionResult> ContactMedicalClaimsList(long id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.MedicalClaims, ViewNames.Health.MedicalClaimsList, mid => Rdb.MedicalClaims.Where(z => z.ContactId == mid), nameof(MedicalClaim.dw_creation_date));

        [ActionName(ActionNames.Health.MedicalClaim)]
        [Route("Contacts/{id}/MedicalClaims/{medicalClaimId}")]
        public Task<IActionResult> ContactMedicalClaimDetail(long id, string medicalClaimId)
            => ContactHealthItemDetail(id, PageKeys.MedicalClaims, ViewNames.Health.MedicalClaim, mid => Rdb.MedicalClaims.Where(z => z.ContactId == mid && z.rev_claim_id == medicalClaimId));

        [ActionName(ActionNames.Health.PharmacyList)]
        [Route("Contacts/{id}/Pharmacy")]
        public Task<IActionResult> ContactPharmacyList(long id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Pharmacy, ViewNames.Health.PharmacyList, mid => Rdb.Pharmacy.Where(z => z.ContactId == mid), nameof(Pharmacy.svc_filled_date));

        [ActionName(ActionNames.Health.Pharmacy)]
        [Route("Contacts/{id}/Pharmacy/{pharmacyItemId}")]
        public Task<IActionResult> ContactPharmacyItemDetail(long id, string pharmacyItemId)
            => ContactHealthItemDetail(id, PageKeys.Pharmacy, ViewNames.Health.Pharmacy, mid => Rdb.Pharmacy.Where(z => z.ContactId == mid && z.rev_transaction_num == pharmacyItemId));

        private async Task<Contact> FindContactByIdAsync(long id)
        {
            Contact c = null;
            c = await Rdb.Contacts.FindAsync(id);
            if (c != null && c.TenantId==this.TenantId)
            {
                return c;
            }
            return null;
        }
    }
}