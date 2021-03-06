using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
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
    public class AjaxController : BasePageController
    {
        public AjaxController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger) : base(AspHelpers.MainNavigationPageKeys.NotSpecified, db, current, logger)
        {

        }

        public static class ObjectTypes
        {
            public const string Report = "Report";
            public const string ReportMetaData = "ReportMetaData";
            public const string IReportVisual = "IReportVisual";
            public const string Contact = "Contact";
            public const string Person = "Person";
            public const string Provider = "Provider";
            public const string User = "User";
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
            var parentNoteIdInt = Parse.ParseNullableInt32(parentNoteId);

            switch (objectType)
            {
                case ObjectTypes.Report:
                case ObjectTypes.ReportMetaData:
                case ObjectTypes.IReportVisual:
                    var reportMetaData = Rdb.ReportMetaData.SingleOrDefault(x => x.ReportMetaDataId == id);
                    if (reportMetaData == null) return NotFound();
                    return await base.CreateNote(parentNoteIdInt, content, reportMetaData);
                case ObjectTypes.Contact:
                case ObjectTypes.Person:
                case ObjectTypes.Provider:
                case ObjectTypes.User:
                    var contact = await FindContactByIdAsync(id);
                    if (contact == null) return NotFound();
                    return await base.CreateNote(parentNoteIdInt, content, contact);
                default:
                    return NoContent();
            }
        }
    }
}