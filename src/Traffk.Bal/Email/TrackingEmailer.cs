using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Settings;
using MimeKit;
using H = cloudscribe.HtmlAgilityPack;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Ddb;

namespace Traffk.Bal.Email
{
    public class TrackingEmailer : IEmailer
    {
        private readonly IOptions<SmtpOptions> Options;
        private readonly IEmailer Inner;
        private readonly CrmDdbContext Crm;
        private readonly TraffkRdbContext Rdb;
        private readonly ITraffkTenantFinder TenantFinder;

        public TrackingEmailer(IOptions<SmtpOptions> options, CrmDdbContext crm, TraffkRdbContext rdb, ITraffkTenantFinder tenantFinder)
        {
            Requires.NonNull(options, nameof(options));

            Options = options;
            Inner = new RawEmailer(options);
            Crm = crm;
            Rdb = rdb;
            TenantFinder = tenantFinder;
        }

        private string TrackHtmlContent(string html, ICollection<CommunicationLog.TrackedLink> trackedLinkBag, string hostname, int tenantId, string contactId, string messageId)
        {
            return TrackHtmlContent(html, trackedLinkBag, (a, linkId) => 
            {
                var u = new Uri($"http://{hostname}/e/{tenantId}/{contactId}");
                return u.AppendParameter("m", messageId).AppendParameter("l", linkId);
            });
        }

        private static string TrackHtmlContent(string html, ICollection<CommunicationLog.TrackedLink> trackedLinkBag, Func<Uri, string, Uri> transformer)
        {
            if (string.IsNullOrEmpty(html)) return html;

            var doc = new H.HtmlDocument();
            doc.LoadHtml(html);
            int sequence = 0;
            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//a"))
            {
                Uri origUrl;
                if (!Uri.TryCreate(node.Attributes["href"]?.Value, UriKind.Absolute, out origUrl)) continue;
                var linkId = CommunicationLog.TrackedLink.CreateId(CommunicationLog.TrackedLinkTypes.Anchor, sequence);
                var trackedUrl = transformer(origUrl, linkId);
                node.Attributes["href"].Value = trackedUrl.ToString();
                trackedLinkBag.Add(new CommunicationLog.TrackedLink(trackedUrl.PathAndQuery, origUrl, CommunicationLog.TrackedLinkTypes.Anchor, sequence++, linkId));
            }
            sequence = 0;
            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//img"))
            {
                Uri origUrl;
                if (!Uri.TryCreate(node.Attributes["src"]?.Value, UriKind.Absolute, out origUrl)) continue;
                var linkId = CommunicationLog.TrackedLink.CreateId(CommunicationLog.TrackedLinkTypes.Asset, sequence);
                var trackedUrl = transformer(origUrl, linkId);
                node.Attributes["src"].Value = trackedUrl.ToString();
                trackedLinkBag.Add(new CommunicationLog.TrackedLink(trackedUrl.PathAndQuery, origUrl, CommunicationLog.TrackedLinkTypes.Asset, sequence++, linkId));
                break;
            }

            return doc.ToHtmlString();
        }

        async Task IEmailer.SendEmailAsync(IEnumerable<MimeMessage> messages, Action<PreSendEventArgs> preSend, Action<PostSendEventArgs> postSend, object context)
        {
            var tenantId = await TenantFinder.GetTenantIdAsync();
            var app = Rdb.Applications.FirstOrDefault(z => z.TenantId == tenantId && z.ApplicationType== Application.ApplicationTypes.Portal);
            var hostname = app.ApplicationSettings.Hosts.HostInfos[0].Hostname;
            var smtpOptions = Options.Value;
            var logByMessage = new Dictionary<string, CommunicationLog>();
            foreach (var m in messages)
            {
                CommunicationLog log = null;
                var contactId = m.Headers[MailHelpers.ContactIdHeader];
                if (!string.IsNullOrEmpty(contactId))
                {
                    m.MessageId = m.MessageId.LeftOf("@") + "@" + Stuff.CoalesceStrings(smtpOptions.LocalDomain, m.MessageId.RightOf("@"));
                    var to = m.To[0] as MailboxAddress;
                    log = new CommunicationLog
                    {
                        TrackedLinks = new List<CommunicationLog.TrackedLink>(),
                        CommunicationTopic = m.Headers[MailHelpers.TopicHeader],
                        CommunicationCampaign = m.Headers[MailHelpers.CampaignHeader],
                        RecipientContactId = contactId,
                        RecipientEmailAddress = to.Address,
                        Subject = m.Subject,
                        Id = m.MessageId,
                        TenantId = tenantId,
                        CommunicationMedium = Data.Rdb.SystemCommunication.CommunicationMediums.Email
                    };
                    int jobId;
                    if (int.TryParse(m.Headers[MailHelpers.JobId], out jobId))
                    {
                        log.JobId = jobId;
                    }
                    foreach (var part in m.FindHtmlParts())
                    {
                        part.Text = TrackHtmlContent(part.Text, log.TrackedLinks, hostname, tenantId, log.RecipientContactId, m.MessageId);
                    }
                }
                logByMessage[m.MessageId] = log;
            }
            await Inner.SendEmailAsync(
                messages, 
                preSend,
                (psa) => 
                {
                    if (psa.SendException != null)
                    {
                        var log = logByMessage[psa.Message.MessageId];
                        if (log != null)
                        {
                            log.DeliveryError = new ExceptionError(psa.SendException);
                        }
                    }
                    postSend?.Invoke(psa);
                },
                context);
            await Crm.InsertEntitiesAsync(logByMessage.Values.WhereNotNull());
        }
    }
}
