using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Email;

namespace TraffkPortal.Controllers
{
    [Route("e")]
    public class EmailRedirectorController : Controller
    {
        protected readonly ILogger Logger;
        protected readonly TraffkRdbContext Rdb;

        public EmailRedirectorController(TraffkRdbContext rdb, ILoggerFactory loggerFactory)
        {
            Rdb = rdb;
            Logger = loggerFactory.CreateLogger(this.GetType());
        }

        [Route("{tenantId}/{pieceSegment}/{trackerSegment}")]
        public async Task<IActionResult> LogAndRedirect(int tenantId, string pieceSegment, string trackerSegment)
        {
            Guid g = TrackingEmailer.ConvertPieceSegmentToPieceUid(pieceSegment);
            var piece = await Rdb.CommunicationPieces.FirstOrDefaultAsync(z => z.CommunicationPieceUid == g && z.TenantId==tenantId);
            if (piece == null) return NotFound();
            g = TrackingEmailer.ConvertTrackerSegmentToTrackerUid(trackerSegment);
            var tracker = await Rdb.CommunicationBlastTrackers.FirstOrDefaultAsync(z => z.TrackerUid == g && z.TenantId == tenantId);
            if (tracker == null) return NotFound();
            var visit = new CommunicationPieceVisit
            {
                CommunicationPieceId = piece.CommunicationPieceId,
                CommunicationBlastTrackerId = tracker.CommunicationBlastTrackerId
            };
            Rdb.CommunicationPieceVisits.Add(visit);
            await Rdb.SaveChangesAsync();
            IActionResult res;
            switch (tracker.LinkType)
            {
                case CommunicationBlastTrackerLinkTypes.Asset:
                    res = RedirectPermanent(tracker.RedirectUrl);
                    break;
                case CommunicationBlastTrackerLinkTypes.Anchor:
                default:
                    res = Redirect(tracker.RedirectUrl);
                    break;
            }
            return res;
        }
    }
}
