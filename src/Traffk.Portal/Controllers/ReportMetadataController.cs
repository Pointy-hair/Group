using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Traffk.Bal.Data.Rdb;
using Traffk.Portal.Models.ReportMetadataModels;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Services;

namespace Traffk.Portal.Controllers
{
    [Authorize]
    public class ReportMetadataController : BasePageController
    {
        public static class ActionNames
        {
            public const string Edit = "Edit";
            public const string Index = "Index";
        }

        public ReportMetadataController( 
            TraffkRdbContext db, 
            CurrentContextServices current, 
            ILoggerFactory loggerFactory, 
            ICacher cacher = null) : base(AspHelpers.MainNavigationPageKeys.NotSpecified, db, current, loggerFactory, cacher)
        {

        }

        [Route("/ReportMetadata")]
        public IActionResult Index()
        {
            var reportMetadatas = Rdb.ReportMetaData.AsQueryable();
            return View(reportMetadatas);
        }

        [HttpGet]
        [ActionName(ActionNames.Edit)]
        [Route("/ReportMetadata/{id}")]
        public IActionResult Edit(int id)
        {
            var reportMetaData = Rdb.ReportMetaData.FirstOrDefault(x => x.ReportMetaDataId == id);
            var reportMetaDataViewModel = new ReportMetadataViewModel(reportMetaData);
            return View(reportMetaDataViewModel);
        }

        [HttpPost]
        [ActionName(ActionNames.Edit)]
        [ValidateAntiForgeryToken]
        [Route("/ReportMetadata/{id}")]
        public async Task<IActionResult> Edit(ReportMetadataViewModel reportMetaDataViewModel)
        {
            var newTitle = reportMetaDataViewModel.Title;
            var actualReportMetadata =
                Rdb.ReportMetaData.First(x => x.ReportMetaDataId == reportMetaDataViewModel.ReportMetadataId);
            actualReportMetadata.ReportDetails.Title = newTitle;

            Rdb.Update(actualReportMetadata);
            await Rdb.SaveChangesAsync();
            return RedirectToAction(ActionNames.Index);
        }
    }
}