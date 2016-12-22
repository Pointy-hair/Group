using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal;
using System;
using System.Threading.Tasks;

namespace TraffkPortal.Controllers
{
    [Route("e")]
    public class EmailRedirectorController : Controller
    {
        private readonly CrmDdbContext CRM;

        public EmailRedirectorController(CrmDdbContext crm)
        {
            CRM = crm;
        }

        [Route("{tenantId}/{contactId}")]
        public async Task<IActionResult> LogAndRedirect(int tenantId, string contactId, string m, string l)
        {
            var cl = CRM.CommunicationLogs.FirstOrDefault(z => z.TenantId == tenantId && z.Id == m);
            if (cl == null || cl.TrackedLinks==null) return NotFound();
            var tl = cl.TrackedLinks.FirstOrDefault(z => z.Id == l);
            if (tl == null) return NotFound();
            var utcNow = DateTime.UtcNow;
            if (cl.FirstSeenAtUtc == null)
            {
                cl.FirstSeenAtUtc = utcNow;
            }
            IActionResult res;
            WebActionLog visitor = new WebActionLog
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                TraceIdentifier = HttpContext.TraceIdentifier,
                UserAgentString = Request.Headers["User-Agent"].ToString(),
                CreatedAtUtc = utcNow
            };
            tl.AddVisitor(visitor);
            switch (tl.TrackedLinkType)
            {
                case CommunicationLog.TrackedLinkTypes.Asset:
                    res = RedirectPermanent(tl.RedirectUrl.ToString());
                    break;
                case CommunicationLog.TrackedLinkTypes.Anchor:
                default:
                    res = Redirect(tl.RedirectUrl.ToString());
                    break;
            }
            CRM.Update(cl);
            await CRM.SaveChangesAsync();
            return res;
        }
    }
}