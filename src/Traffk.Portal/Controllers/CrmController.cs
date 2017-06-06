using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal;
using Traffk.Bal.Communications;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Settings;
using Traffk.Tableau;
using TraffkPortal.Models;
using TraffkPortal.Models.CrmModels;
using TraffkPortal.Models.ReportingModels;
using TraffkPortal.Permissions;
using TraffkPortal.Services;
using ILogger = Serilog.ILogger;

namespace TraffkPortal.Controllers
{
    [Authorize]
    [PermissionAuthorize(PermissionNames.CustomerRelationshipData)]
    [Route("Crm")]
    public class CrmController : BasePageController
    {
        public const string Name = "Crm";
        public const VisualContext ReportVisualContext = VisualContext.ContactPerson;

        public IReportVisualService ReportVisualService { get; }

        public enum PageKeys
        {
            Background,
            Messages,
            Notes,
            Relationships,
            #region Health
            CareAlerts,
            Eligibility,
            EligibilityList,
            MedicalClaimDiagnosis,
            MedicalClaimLines,
            MillimanScoreList,
            MedicalClaims,
            Participation,
            Pharmacy,
            QualityMetrics,
            Visit,
            #endregion
            Reports
        }

        public static class ViewNames
        {
            public static class Health
            {
                public const string CareAlert = "ContactCareAlert";
                public const string CareAlertsList = "ContactCareAlertsList";
                public const string Eligibility = "ContactEligibility";
                public const string EligibilityList = "ContactEligibilityList";
                public const string MedicalClaimDiagnosis = "ContactMedicalClaimDiagnosis";
                public const string MedicalClaimLines = "ContactMedicalClaimLines";
                public const string MillimanScore = "ContactMillimanScore";
                public const string MillimanScoreList = "ContactMillimanScoresList";
                public const string MedicalClaim = "ContactMedicalClaimDetail";
                public const string MedicalClaimsList = "ContactMedicalClaimsList";
                public const string Participation = "ContactParticipation";
                public const string ParticipationList = "ContactParticipationList";
                public const string Pharmacy = "ContactPharmacyItemDetail";
                public const string PharmacyList = "ContactPharmacyList";
                public const string QualityMetric = "ContactQualityMetric";
                public const string QualityMetricsList = "ContactQualityMetricsList";
                public const string Visit = "ContactVisit";
                public const string VisitList = "ContactVisitsList";
            }
        }

        public static class ActionNames
        {
            public const string ContactList = "Index";
            public const string ContactCreate = "Create";
            public const string ContactBackground = "ContactBackground";
            public const string ContactNotes = "ContactNotes";
            public const string ContactMessages = "ContactMessages";
            public const string ContactRelationships = "ContactRelationships";
            public const string CreateNote = "CreateNote";
            public const string SendDirectMessage = "SendDirectMessage";
            public const string Reports = "ReportIndex";
            public const string Report = "Report";

            public static class Health
            {
                public const string CareAlert = "ContactCareAlertDetail";
                public const string CareAlertsList = "ContactCareAlertsList";
                public const string Eligibility = "ContactEligibiliityDetail";
                public const string EligibilityList = "ContactEligibilityList";
                public const string MedicalClaimDiagnosis = "ContactMedicalClaimDiagnosis";
                public const string MedicalClaimLines = "ContactMedicalClaimLines";
                public const string MillimanScore = "ContactMillimanScoreDetail";
                public const string MillimanScoreList = "ContactMillimanScoreList";
                public const string MedicalClaim = "ContactMedicalClaimDetail";
                public const string MedicalClaimsList = "ContactMedicalClaimsList";
                public const string Participation = "ContactParticipationDetail";
                public const string ParticipationList = "ContactParticipationList";
                public const string Pharmacy = "ContactPharmacyItemDetail";
                public const string PharmacyList = "ContactPharmacyList";
                public const string QualityMetric = "ContactQualityMetricDetail";
                public const string QualityMetricsList = "ContactQualityMetricsList";
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
            if (contact.ContactType == ContactTypes.Person)
            {
                ViewData["ContactGender"] = contact.AsPerson.Gender;
            }
            SetHeroLayoutViewData(contact.ContactId, contact.FullName, pageKey, contact.ContactType);
        }

        private readonly IEmailSender EmailSender;

        public CrmController(
            IEmailSender emailSender,
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger,
            ICacher cacher,
            IReportVisualService reportVisualService
            )
            : base(AspHelpers.MainNavigationPageKeys.CRM, db, current, logger, cacher)
        {
            ReportVisualService = reportVisualService;
            EmailSender = emailSender;
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

        [Route("Contacts/Delete")]
        [HttpDelete]
        public Task<IActionResult> Delete() => JsonDeleteFromRdbAsync<Contact, int>(Rdb.Contacts);


        [ActionName(ActionNames.ContactCreate)]
        [Route("Contacts/Create/{contactTypeTemplate}")]
        public IActionResult Create(string contactTypeTemplate)
        {
            var contact = Contact.Create(Parse.ParseEnum<ContactTypes>(contactTypeTemplate));
            return View(nameof(ContactBackground), new ContactModel(contact));
        }

        private Task SetViewBagEmailTemplateItems()
        {
            return Task.CompletedTask;
            /*
                        var communications = await
                        Rdb.SystemCommunications.Include(z => z.MessageTemplate).Where(z =>
                          z.TenantId == this.TenantId && z.CommunicationPurpose == SystemCommunication.CommunicationPurposes.DirectMessage && z.CommunicationMedium == SystemCommunication.CommunicationMediums.Email).OrderBy(z => z.MessageTemplate.MessageTemplateTitle).ToListAsync();
                        ViewBag.EmailTemplateItems = communications.ConvertAll(c => new SelectListItem { Text = c.MessageTemplate.MessageTemplateTitle, Value = c.SystemCommunicationId.ToString() });
            */
        }

        [HttpPost]
        [PermissionAuthorize(PermissionNames.DirectMessaging)]
        [ActionName(ActionNames.SendDirectMessage)]
        [Route("Contacts/{id}/SendDirectMessage")]
        public async Task<IActionResult> SendDirectMessage(
            int id,
            int templateId,
            string subject,
            string body)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            var model = CommunicationModelFactory.CreateSimpleContentModel(subject, body);
            await EmailSender.SendEmailCommunicationAsync(SystemCommunicationPurposes.DirectMessage, model, contact.PrimaryEmail, contact.FullName, contact.ContactId);
            return Ok();
        }

        [HttpPost]
        [PermissionAuthorize(PermissionNames.DirectMessaging)]
        [ActionName(ActionNames.CreateNote)]
        [Route("Contacts/{id}/CreateNote")]
        public async Task<IActionResult> CreateNote(
            int id,
            string parentNoteId,
            string content)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            Rdb.AttachNote(Current.User.Contact, null, content, contact);
            await Rdb.SaveChangesAsync();
            return Ok();
        }

        [ActionName(ActionNames.ContactBackground)]
        [Route("Contacts/{id}/Background")]
        public Task<IActionResult> ContactBackground(int id)
        {
            return RawContactRecordInfo(id, PageKeys.Background, nameof(ContactBackground));
        }

        [ActionName(ActionNames.ContactNotes)]
        [Route("Contacts/{id}/Notes")]
        public Task<IActionResult> ContactNotes(int id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            return RawContactRecordInfo(id, PageKeys.Notes, nameof(ContactNotes), m =>
            {
                var notes = Rdb.GetAttachedNotes(m.Contact);
                m.Notes = ApplyBrowse(notes, sortCol ?? nameof(Note.CreatedAtUtc), sortDir, page, pageSize).ToList();
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
        public async Task<IActionResult> ContactMessages(int id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, PageKeys.Messages);

            var items = Rdb.CommunicationHistory.Where(z => z.ContactId == contact.ContactId && z.TenantId == this.TenantId);
            items = ApplyBrowse(
                items, sortCol ?? nameof(CommunicationPiece.CreatedAt), sortDir,
                page, pageSize);

            return View(items);
        }

        private async Task<IActionResult> RawContactRecordInfo(int id, PageKeys pageKey, string viewName, Action<ContactModel> preView = null)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, pageKey);
            var model = new ContactModel(contact);
            if (contact.ContactId == 0)
            {
                ViewData.SetTitleAndHeading($"Create {model.Contact.ContactType}");
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
            int id,
            [Bind(
            nameof(ContactModel.Contact),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactId),
            nameof(ContactModel.Contact)+"."+nameof(Contact.FullName),
            nameof(ContactModel.Contact)+"."+nameof(Contact.PrimaryEmail),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactType),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactDetails),
            nameof(ContactModel.Contact)+"."+nameof(Contact.ContactDetails)+"."+nameof(Contact.ContactDetails_.Tags),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.DateOfBirth),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.Gender),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.Prefix),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.FirstName),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.MiddleName),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.LastName),
            nameof(ContactModel.Contact)+"."+nameof(PersonContact.Suffix),
            nameof(ContactModel.CommunicationPieces)
//            nameof(ContactModel.Optings)
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
                SetToast(AspHelpers.ToastMessages.Saved);
                return RedirectToIndex();
            }
            await SetViewBagEmailTemplateItems();
            return View(model);
        }

        private async Task<IActionResult> ContactHealthItemDetail<THealthItem>(int contactId, PageKeys pageKey, string viewName, Func<Contact, IQueryable<THealthItem>> createModel) where THealthItem : ITraffkTenanted
        {
            var contact = await FindContactByIdAsync(contactId);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, pageKey);
            var items = createModel(contact);
            items = items.Where(i => i.TenantId == this.TenantId);
            return View(viewName, items.FirstOrDefault());
        }

        private Task<IActionResult> ContactHealthItemDetail<THealthItem>(int contactId, PageKeys pageKey, string viewName, Func<long, IQueryable<THealthItem>> createModel) where THealthItem : ITraffkTenanted
            => ContactHealthItemDetail(contactId, pageKey, viewName, c => createModel(c.ContactId));

        private async Task<IActionResult> ContactHealthItemList<THealthItem>(int contactId, string sortCol, string sortDir, int? page, int? pageSize, PageKeys pageKey, string viewName, Func<Contact, IQueryable<THealthItem>> createModel, string defaultSortCol = null) where THealthItem : ITraffkTenanted
        {
            var contact = await FindContactByIdAsync(contactId);
            if (contact == null) return NotFound();

            SetHeroLayoutViewData(contact, pageKey);
            var items = createModel(contact);
            items = items.Where(i => i.TenantId == this.TenantId);
            items = ApplyBrowse(items, sortCol ?? defaultSortCol, sortDir, page, pageSize);
            return View(viewName, items);
        }

        private Task<IActionResult> ContactHealthItemList<THealthItem>(int contactId, string sortCol, string sortDir, int? page, int? pageSize, PageKeys pageKey, string viewName, Func<long, IQueryable<THealthItem>> createModel, string defaultSortCol = null) where THealthItem : ITraffkTenanted
            => ContactHealthItemList<THealthItem>(contactId, sortCol, sortDir, page, pageSize, pageKey, viewName, c => createModel(c.ContactId), defaultSortCol);

        [ActionName(ActionNames.Health.CareAlert)]
        [Route("Contacts/{id}/CareAlerts/{recordId}")]
        public Task<IActionResult> ContactCareAlert(int id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.CareAlerts, ViewNames.Health.CareAlert, mid => Rdb.CareAlerts.Where(z => z.ContactId == mid && z.CareAlertId == recordId));

        [ActionName(ActionNames.Health.CareAlertsList)]
        [Route("Contacts/{id}/CareAlerts")]
        public Task<IActionResult> ContactCareAlertsList(int id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.CareAlerts, ViewNames.Health.CareAlertsList, mid => Rdb.CareAlerts.Where(z => z.ContactId == mid), nameof(CareAlert.CareAlertDd));

        [ActionName(ActionNames.Health.MillimanScore)]
        [Route("Contacts/{id}/MillimanScores/{recordId}")]
        public Task<IActionResult> ContactMillimanScore(int id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.MillimanScoreList, ViewNames.Health.MillimanScore, mid => Rdb.MillimanScores.Where(z => z.ContactId == mid && z.MillimanScoreId==recordId));

        [ActionName(ActionNames.Health.MillimanScoreList)]
        [Route("Contacts/{id}/MillimanScores")]
        public Task<IActionResult> ContactMillimanScoreList(int id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.MillimanScoreList, ViewNames.Health.MillimanScoreList, mid => Rdb.MillimanScores.Where(z => z.ContactId == mid), nameof(MillimanScore.ScorePeriodEndDd));

        [ActionName(ActionNames.Health.Eligibility)]
        [Route("Contacts/{id}/Eligibility")]
        public Task<IActionResult> ContactEligibility(int id)
            => ContactHealthItemDetail(id, PageKeys.Eligibility, ViewNames.Health.Eligibility, mid => Rdb.Eligibility.Where(z => z.ContactId == mid));

        [ActionName(ActionNames.Health.EligibilityList)]
        [Route("Contacts/{id}/EligibilityList")]
        public Task<IActionResult> ContactEligibilityList(int id, string sortCol, string sortDir, int? page,
            int? pageSize)
            =>
                ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Eligibility,
                    ViewNames.Health.EligibilityList, mid => Rdb.Eligibility.Where(z => z.ContactId == mid),
                    nameof(Eligibility.EligibilityId));

        [ActionName(ActionNames.Health.MedicalClaimDiagnosis)]
        [Route("Contacts/{id}/MedicalClaimDiagnosis/{medicalClaimId}")]
        public Task<IActionResult> ContactMedicalClaimDiagnosis(int id, int medicalClaimId)
            => ContactHealthItemDetail(id, PageKeys.MedicalClaimDiagnosis, ViewNames.Health.MedicalClaimDiagnosis, mid => Rdb.MedicalClaimDiagnoses.Where(x => x.MedicalClaim.ContactId == mid && x.MedicalClaimId == medicalClaimId));

        [ActionName(ActionNames.Health.MedicalClaimLines)]
        [Route("Contacts/{id}/MedicalClaimLines/{medicalClaimId}")]
        public Task<IActionResult> ContactMedicalClaimLines(int id, int medicalClaimId)
            => ContactHealthItemDetail(id, PageKeys.MedicalClaimLines, ViewNames.Health.MedicalClaimLines, mid => Rdb.MedicalClaimLines.Where(x => x.ContactId == mid && x.MedicalClaimId == medicalClaimId));

        [ActionName(ActionNames.Health.Participation)]
        [Route("Contacts/{id}/Participation/{recordId}")]
        public Task<IActionResult> ContactParticipation(int id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.Participation, ViewNames.Health.Participation, mid => Rdb.Participation.Where(z => z.ContactId == mid && z.ParticipationId == recordId));

        [ActionName(ActionNames.Health.ParticipationList)]
        [Route("Contacts/{id}/Participation")]
        public Task<IActionResult> ContactParticipationList(int id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Participation, ViewNames.Health.ParticipationList, mid => Rdb.Participation.Where(z => z.ContactId == mid), nameof(Participation.ProgramEndDd));

        [ActionName(ActionNames.Health.Visit)]
        [Route("Contacts/{id}/Visits/{recordId}")]
        public Task<IActionResult> ContactVisit(int id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.Visit, ViewNames.Health.Visit, mid => Rdb.Visits.Where(z => z.ContactId == mid && z.VisitId == recordId));

        [ActionName(ActionNames.Health.VisitList)]
        [Route("Contacts/{id}/Visits")]
        public Task<IActionResult> ContactVisitsList(int id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Visit, ViewNames.Health.VisitList, mid => Rdb.Visits.Where(z => z.ContactId == mid), nameof(Visit.VisitEndDd));

        [ActionName(ActionNames.Health.QualityMetric)]
        [Route("Contacts/{id}/QualityMetrics/{recordId}")]
        public Task<IActionResult> ContactQualityMetric(int id, int recordId)
            => ContactHealthItemDetail(id, PageKeys.QualityMetrics, ViewNames.Health.QualityMetric, mid => Rdb.QualityMetrics.Where(z => z.ContactId==mid));

        [ActionName(ActionNames.Health.QualityMetricsList)]
        [Route("Contacts/{id}/QualityMetrics")]
        public Task<IActionResult> ContactQualityMetricsList(int id, string sortCol, string sortDir, int? page, int? pageSize)
        {
            return ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.QualityMetrics, ViewNames.Health.QualityMetricsList, delegate (long mid)
            {
                return Rdb.QualityMetrics.Where(z => z.ContactId == id);
            }, nameof(QualityMetric.MeasureToDd));
        }

        [ActionName(ActionNames.Health.MedicalClaimsList)]
        [Route("Contacts/{id}/MedicalClaims")]
        public Task<IActionResult> ContactMedicalClaimsList(int id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.MedicalClaims, ViewNames.Health.MedicalClaimsList, mid => Rdb.MedicalClaims.Where(z => z.ContactId == mid), nameof(MedicalClaim.CreatedAtUtc));

        [ActionName(ActionNames.Health.MedicalClaim)]
        [Route("Contacts/{id}/MedicalClaims/{medicalClaimId}")]
        public Task<IActionResult> ContactMedicalClaimDetail(int id, int medicalClaimId)
            => ContactHealthItemDetail(id, PageKeys.MedicalClaims, ViewNames.Health.MedicalClaim, mid => Rdb.MedicalClaims.Where(z => z.ContactId == mid && z.MedicalClaimId == medicalClaimId));

        [ActionName(ActionNames.Health.PharmacyList)]
        [Route("Contacts/{id}/Pharmacy")]
        public Task<IActionResult> ContactPharmacyList(int id, string sortCol, string sortDir, int? page, int? pageSize)
            => ContactHealthItemList(id, sortCol, sortDir, page, pageSize, PageKeys.Pharmacy, ViewNames.Health.PharmacyList, mid => Rdb.Pharmacy.Where(z => z.ContactId == mid), nameof(Pharmacy.PrescriptionFilledDd));

        [ActionName(ActionNames.Health.Pharmacy)]
        [Route("Contacts/{id}/Pharmacy/{pharmacyItemId}")]
        public Task<IActionResult> ContactPharmacyItemDetail(int id, int pharmacyItemId)
            => ContactHealthItemDetail(id, PageKeys.Pharmacy, ViewNames.Health.Pharmacy, mid => Rdb.Pharmacy.Where(z => z.ContactId == mid && z.PharmacyId == pharmacyItemId));
        
        private async Task<Contact> FindContactByIdAsync(int id)
        {
            Contact c = null;
            c = await Rdb.Contacts.FindAsync(id);
            if (c != null && c.TenantId==this.TenantId)
            {
                return c;
            }
            return null;
        }

        [Route("Contacts/{id}/Reports")]
        [ActionName(ActionNames.Reports)]
        public async Task<IActionResult> ReportIndex(int id)
        {
            var contact = await FindContactByIdAsync(id);
            if (contact == null) return NotFound();
            SetHeroLayoutViewData(contact, PageKeys.Reports);

            var reportSearchCriteria = new ReportSearchCriteria
            {
                VisualContext = ReportVisualContext
            };
            var root = ReportVisualService.GetReportFolderTreeRoot(reportSearchCriteria);
            return View(root);
        }

        [SetTableauTrustedTicket]
        [Route("Contacts/{contactId}/Reports/{id}/{anchorName}")]
        [ActionName(ActionNames.Report)]
        public async Task<IActionResult> Report(int contactId, string id, string anchorName)
        {
            var contact = await FindContactByIdAsync(contactId);
            SetHeroLayoutViewData(contact, PageKeys.Messages);

            var reportSearchCriteria = new ReportSearchCriteria
            {
                VisualContext = ReportVisualContext,
                ReportId = Parse.ParseInt32(id)
            };
            var reportVisual = ReportVisualService.GetReportVisual(reportSearchCriteria);
            if (reportVisual == null)
            {
                RedirectToAction(ActionNames.ContactBackground);
            }

            Logger.Information("{@EventType} {@ReportId} {@ContactId}", EventType.LoggingEventTypes.ViewedReport.ToString(), reportVisual.Id.ToString(), contactId);

            var tableauReportViewModel = new TableauReportViewModel(reportVisual);
            return View(tableauReportViewModel);
        }
    }
}