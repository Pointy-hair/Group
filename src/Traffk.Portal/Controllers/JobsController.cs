using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TraffkPortal.Services;
using Traffk.Bal.Data.Rdb;
using Microsoft.EntityFrameworkCore;
using System;

namespace TraffkPortal.Controllers
{
    [Authorize]
    public class JobsController : BasePageController
    {
        public const string Name = "Jobs";

        public static class ActionNames
        {
            public const string Jobs = "Jobs";
            public const string JobDetails = "JobDetails";
            public const string JobCancel = "JobCancel";
        }

        public static class ViewNames
        {
            public const string JobDetails = "Job";
            public const string JobList = "Jobs";
        }

        public JobsController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory
            )
            : base(AspHelpers.MainNavigationPageKeys.Manage, db, current, loggerFactory)
        { }

        [ActionName(ActionNames.Jobs)]
        [Route("/Jobs")]
        public async Task<IActionResult> Jobs(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var items = Rdb.Jobs.Where(z => z.TenantId == TenantId);
            items = ApplyBrowse(items, sortCol ?? nameof(Job.CreatedAtUtc), sortDir ?? AspHelpers.SortDirDescending, page, pageSize);
            return View(ViewNames.JobList, await items.ToListAsync());
        }

        [ActionName(ActionNames.JobCancel)]
        [HttpDelete]
        [Route("/Jobs/{id}/Cancel")]
        public async Task<IActionResult> CancelJob(int id)
        {
            var job = await Rdb.Jobs.FindAsync(id);
            if (job==null) return NotFound();
            if (!job.CanBeCancelled) return StatusCode(System.Net.HttpStatusCode.Conflict);
            job.JobStatus = Job.StatusNames.Cancelling;
            Rdb.Update(job);
            await Rdb.SaveChangesAsync();
            return NoContent();
        }
    }
}
