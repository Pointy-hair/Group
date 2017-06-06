using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using H = cloudscribe.HtmlAgilityPack;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Bal.Data;
using RevolutionaryStuff.Core.Collections;
using RevolutionaryStuff.Core.EncoderDecoders;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Email
{
    public class TrackingEmailer : ITrackingEmailer
    {
        private readonly IEmailer Inner;
        private readonly TraffkTenantModelDbContext Rdb;
        private readonly ITraffkTenantFinder TenantFinder;
        private readonly ICommunicationBlastFinder BlastFinder;
        private readonly int TenantId;

        public TrackingEmailer(IEmailer emailer, TraffkTenantModelDbContext rdb, ITraffkTenantFinder tenantFinder, ICommunicationBlastFinder blastFinder)
        {
            Inner = emailer;
            Rdb = rdb;
            TenantFinder = tenantFinder;
            BlastFinder = blastFinder;
            TenantId = tenantFinder.GetTenantIdAsync().ExecuteSynchronously();
        }

        public static Guid ConvertPieceSegmentToPieceUid(string part)
            => new Guid(Base32.Decode(part));

        public static Guid ConvertTrackerSegmentToTrackerUid(string part)
            => new Guid(Base32.Decode(part));

        private string TrackHtmlContent(CommunicationBlast blast, string html, Guid communicationPieceUid, MultipleValueDictionary<string, CommunicationBlastTracker> trackersByUrl, string hostname)
        {
            var pieceSegment = Base32.Encode(communicationPieceUid.ToByteArray());
            return TrackHtmlContent(
                html,
                (pos, linkType, linkUrl) =>
                {
                    var tracker = trackersByUrl[linkUrl.ToString()]
                    .FirstOrDefault(z =>
                    z.CommunicationType == Communications.CommunicationTypes.Email &&
                    z.Position == pos &&
                    z.LinkType == linkType);
                    if (tracker == null)
                    {
                        tracker = new CommunicationBlastTracker
                        {
                            CommunicationType = Communications.CommunicationTypes.Email,
                            Position = pos,
                            LinkType = linkType,
                            RedirectUrl = linkUrl.ToString(),
                            TrackerUid = Guid.NewGuid(),
                            CommunicationBlast = blast                            
                        };
                        Rdb.CommunicationBlastTrackers.Add(tracker);
                        trackersByUrl.Add(tracker.RedirectUrl, tracker);
                    }
                    return new Uri($"https://{hostname}/e/{TenantId}/{pieceSegment}/{Base32.Encode(tracker.TrackerUid.ToByteArray())}");
                });
        }

        private static string TrackHtmlContent(string html, Func<int, CommunicationBlastTrackerLinkTypes, Uri, Uri> transformer)
        {
            if (string.IsNullOrEmpty(html)) return html;

            var doc = new H.HtmlDocument();
            doc.LoadHtml(html);
            int sequence = 0;
            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//a"))
            {
                Uri origUrl;
                if (!Uri.TryCreate(node.Attributes["href"]?.Value, UriKind.Absolute, out origUrl)) continue;
                var trackedUrl = transformer(sequence++, CommunicationBlastTrackerLinkTypes.Anchor, origUrl);
                node.Attributes["href"].Value = trackedUrl.ToString();
            }
            sequence = 0;
            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//img"))
            {
                Uri origUrl;
                if (!Uri.TryCreate(node.Attributes["src"]?.Value, UriKind.Absolute, out origUrl)) continue;
                var trackedUrl = transformer(sequence++, CommunicationBlastTrackerLinkTypes.Asset, origUrl);
                node.Attributes["src"].Value = trackedUrl.ToString();
                break; //we only need 1 of these
            }
            return doc.ToHtmlString();
        }

        async Task ITrackingEmailer.SendEmailAsync(Creative creative, IEnumerable<MimeMessage> messages, Action<PreSendEventArgs> preSend, Action<PostSendEventArgs> postSend, object context)
        {
            var blast = await BlastFinder.FindAsync(creative);
            creative = blast.Creative;
            var trackerByUrl = Rdb.CommunicationBlastTrackers.Where(z => z.CommunicationBlastId == blast.CommunicationBlastId).ToMultipleValueDictionary(z => z.RedirectUrl);
            var piecesByMessageId = new Dictionary<string, CommunicationPiece>();

            var tenantId = await TenantFinder.GetTenantIdAsync();

            var app = Rdb.Apps.FirstOrDefault(z => z.TenantId == tenantId && z.AppType== AppTypes.Portal);
            var hostname = app.AppSettings.Hosts.HostInfos[0].Hostname;
            foreach (var m in messages)
            {
                var piece = new CommunicationPiece
                {
                    CommunicationBlast = blast,
                    CommunicationPieceUid = Guid.NewGuid(),
                    ContactId = m.ContactId()
                };
                piece.Data.DeliveryEndpoint = m.To[0].ToString();
                piecesByMessageId[m.MessageId] = piece;
                foreach (var part in m.FindHtmlParts())
                {
                    part.Text = TrackHtmlContent(blast, part.Text, piece.CommunicationPieceUid, trackerByUrl, hostname);
                }
            }
            await Inner.SendEmailAsync(
                messages, 
                preSend,
                (psa) => 
                {
                    if (psa.SendException != null)
                    {
                        var piece = piecesByMessageId[psa.Message.MessageId];
                        if (piece != null)
                        {
                            piece.Data.DeliveryError = new ExceptionError(psa.SendException);
                        }
                    }
                    postSend?.Invoke(psa);
                },
                context);

            Rdb.CommunicationPieces.AddRange(piecesByMessageId.Values);
            await Rdb.SaveChangesAsync();
        }
    }
}
