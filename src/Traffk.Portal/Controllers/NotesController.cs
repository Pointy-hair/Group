using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Permissions;
using TraffkPortal.Services;

namespace Traffk.Portal.Controllers
{
    public class NotesController : BasePageController
    {
        public NotesController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger) : base(AspHelpers.MainNavigationPageKeys.NotSpecified, db, current, logger)
        {

        }

        public static class ObjectTypes
        {
            public const string Report = "Report";
            public const string Contact = "Contact";
        }

        [HttpPost]
        [PermissionAuthorize(PermissionNames.DirectMessaging)]
        [ActionName(BaseActionNames.CreateNote)]
        [Route("/Notes/CreateNote/{objectType}/{id}")]
        public async Task<IActionResult> CreateNote(
            int id,
            string parentNoteId,
            string content,
            string objectType)
        {
            switch (objectType)
            {
                case ObjectTypes.Report:
                    var reportMetaData = Rdb.ReportMetaData.SingleOrDefault(x => x.ReportMetaDataId == id);
                    if (reportMetaData == null) return NotFound();
                    return await base.CreateNote(parentNoteId, content, reportMetaData);
                case ObjectTypes.Contact:
                    var contact = await FindContactByIdAsync(id);
                    if (contact == null) return NotFound();
                    return await base.CreateNote(parentNoteId, content, contact);
                default:
                    return NoContent();
            }
        }
    }
}