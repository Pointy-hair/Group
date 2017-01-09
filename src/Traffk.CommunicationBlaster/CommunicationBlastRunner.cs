using System;
using RevolutionaryStuff.Core;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Email;
using System.Threading.Tasks;
using Traffk.Bal.JobResults;
using Traffk.Bal.Services;

namespace TraffkCommunicationBlastRunner
{
    public class CommunicationBlastRunner : JobRunner<ContactsFromEligibilityJobResult>
    {
        private readonly int MessagesPerBlock = 2;
        private readonly TraffkRdbContext Rdb;
        private readonly ITrackingEmailer Emailer;
        private readonly BlobStorageServices Blobs;


        public CommunicationBlastRunner(BlobStorageServices blobs, TraffkRdbContext rdb, ITrackingEmailer emailer)
        {
            Requires.NonNull(blobs, nameof(blobs));
            Requires.NonNull(rdb, nameof(rdb));
            Requires.NonNull(emailer, nameof(emailer));

            Blobs = blobs;
            Rdb = rdb;
            Emailer = emailer;
        }

        protected async override Task OnGoAsync(Job job, ContactsFromEligibilityJobResult result)
        {
            var tenant = await Rdb.Tenants.FindAsync(job.TenantId.Value);
            throw new NotImplementedException();
#if false

            var blast = Rdb.ZCommunicationBlasts.Single(z => z.TenantId == job.TenantId && z.JobId == job.JobId);
            var messageTemplate = Rdb.MessageTemplates.Include(z => z.HtmlBodyTemplate).Include(z => z.SubjectTemplate).Include(z => z.TextBodyTemplate).FirstOrDefault(z => z.MessageTemplateId == blast.MessageTemplateId);

            var streamByCloubBlob = new Dictionary<CloudBlob, StreamMuxer>();
            foreach (var blob in await messageTemplate.GetFileAttachmentInfosAsync(Blobs))
            {
                var st = new MemoryStream();
                await blob.DownloadToStreamAsync(st);
                st.Position = 0;
                streamByCloubBlob[blob] = new StreamMuxer(st);
            }

            var alreadySentRecipientContactIds = new HashSet<int>();
            foreach (var sent in
                from z in Crm.CommunicationLogs
                where z.TenantId == job.TenantId.Value && z.JobId == job.JobId
                select z)
            {
                alreadySentRecipientContactIds.Add(int.Parse(sent.RecipientContactId));
            }
            result.AlreadySentRecipientContactIdCount = alreadySentRecipientContactIds.Count;

            var ands = new List<TestExpression>()
            {
                new TestExpression(CollectionNames.Contacts, nameof(Contact.ContactType), Operators.Equals, Contact.ContactTypes.Person),
                new TestExpression(CollectionNames.Eligibility, nameof(Eligibility.mbr_relationship_desc), Operators.Equals, "Employee"),
            };

            foreach (var collection in ands.ConvertAll(a => a.Collection).Distinct().ToArray())
            {
                if (collection == CollectionNames.Contacts)
                {
                    ands.Add(new TestExpression(collection, nameof(Contact.PrimaryEmail), Operators.IsNotNull));
                }
                ands.Add(new TestExpression(collection, "TenantId", Operators.Equals, job.TenantId));
            }

            var contactIds = TestExpression.MatchingContactIds(Crm, ands);
            result.MatchingContactIdCount = contactIds.Count;

            var tm = new TemplateManager(new DbTemplateFinder(Rdb, job.TenantId.Value), tenant.TenantSettings.ReusableValues);

            var messages = new List<MimeMessage>();

            var wq = new WorkQueue(1);

            Parallel.ForEach(contactIds, new ParallelOptions { MaxDegreeOfParallelism = 1 }, delegate (int contactId) 
            {
                if (alreadySentRecipientContactIds.Contains(contactId))
                {
                    result.IncrementSkippedDueToPreviousSendAttempt();
                    return;
                }
                try
                {
                    var c = Rdb.Contacts.FirstOrDefault(z => z.TenantId == job.TenantId.Value && z.ContactId == contactId);
                    if (c == null)
                    {
                        result.IncrementMissingContactCount();
                        return;
                    }
                    var e = Rdb.Eligibility.FirstOrDefault();// z => z.ContactId == c.Id);
if (e!=null)                    throw new NotImplementedException();
                    var model = Template.ModelTypes.CreateContactSummaryPhiModel(e);
                    var m = new MimeMessage();
                    m.From.Add(new MailboxAddress(tenant.TenantSettings.EmailSenderName, tenant.TenantSettings.EmailSenderAddress));
                    m.Headers.Add(MailHelpers.ContactIdHeader, contactId.ToString());
                    m.Headers.Add(MailHelpers.TopicHeader, blast.TopicName);
                    m.Headers.Add(MailHelpers.CampaignHeader, blast.CampaignName);
                    m.Headers.Add(MailHelpers.JobId, blast.JobId?.ToString());
                    m.To.Add(new MailboxAddress(c.FullName, c.PrimaryEmail));
                    lock (messageTemplate)
                    {
                        m.Fill(tm, messageTemplate, null, c, model);
                    }
                    if (streamByCloubBlob.Count > 0)
                    {
                        var multipart = new Multipart("mixed");
                        multipart.Add(m.Body);
                        foreach (var kvp in streamByCloubBlob)
                        {
                            var attachment = new MimePart(kvp.Key.Properties.ContentType)
                            {
                                ContentObject = new ContentObject(kvp.Value.Create(true, false), ContentEncoding.Default),
                                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                ContentTransferEncoding = ContentEncoding.Base64,
                                FileName = Path.GetFileName(kvp.Key.Name)
                            };
                            multipart.Add(attachment);
                        }
                        m.Body = multipart;
                    }
                    lock (messages)
                    {
                        messages.Add(m);
                        if (messages.Count >= MessagesPerBlock)
                        {
                            var b = messages.ToArray();
                            messages.Clear();
                            wq.Enqueue(() => Emailer.SendEmailAsync(b));
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.IncrementExceptionsDuringMessageCreation();
                    Trace.WriteLine(ex);
                }
            });
            if (messages.Count > 0)
            {
                wq.Enqueue(() => Emailer.SendEmailAsync(messages));
            }

            wq.WaitTillDone();
            streamByCloubBlob.Values.ForEach(sm => sm.Dispose());
#endif
        }
    }
}

