using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TraffkPortal.Services;
using Traffk.Bal.Data.Rdb;
using Microsoft.EntityFrameworkCore;
using System;
using Hangfire;
using Traffk.Bal.Settings;

namespace TraffkPortal.Controllers
{
    [Authorize]
    public class JobsController : BasePageController
    {
        public const string Name = "Jobs";

        public static class ActionNames
        {
            public const string Jobs = "Jobs";
            public const string Job = "Job";
            public const string JobDetails = "JobDetails";
            public const string JobCancel = "JobCancel";
        }

        public static class ViewNames
        {
            public const string JobDetails = "Job";
            public const string JobList = "Jobs";
        }

        private readonly IBackgroundJobClient Backgrounder;
        private readonly TraffkGlobalContext GDB;

        public JobsController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory,
            IBackgroundJobClient backgrounder,
            TraffkGlobalContext gdb
            )
            : base(AspHelpers.MainNavigationPageKeys.Manage, db, current, loggerFactory)
        {
            Backgrounder = backgrounder;
            GDB = gdb;
        }

        [ActionName(ActionNames.Jobs)]
        [Route("/Jobs")]
        public async Task<IActionResult> Jobs(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var jobIdsForTenant =
                GDB.HangfireTenantMappings.Where(x => x.TenantId == Current.TenantId).Select(y => y.JobId);
            var items = GDB.Jobs.Where(z => jobIdsForTenant.Contains(z.JobId));
            items = ApplyBrowse(items, sortCol ?? nameof(HangfireJob.CreatedAtUtc), sortDir ?? AspHelpers.SortDirDescending, page, pageSize);
            return View(ViewNames.JobList, await items.ToListAsync());
        }

        [ActionName(ActionNames.Job)]
        [Route("/Jobs/{id}")]
        public async Task<IActionResult> JobDetails(int id)
        {
            var item = await Rdb.Jobs.FindAsync(id);
            if (item == null) return NotFound();
            return View(ViewNames.JobDetails, item);
        }

        [ActionName(ActionNames.JobCancel)]
        [HttpDelete]
        [Route("/Jobs/{id}/Cancel")]
        public async Task<IActionResult> CancelJob(int id)
        {
            var job = await Rdb.Jobs.FindAsync(id);
            if (job==null) return NotFound();
            if (!job.CanBeCancelled) return StatusCode(System.Net.HttpStatusCode.Conflict);
            job.JobStatus = JobStatuses.Cancelling;
            Rdb.Update(job);
            await Rdb.SaveChangesAsync();
            return NoContent();
        }
    }
}
