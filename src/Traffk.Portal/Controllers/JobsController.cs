using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using TraffkPortal.Services;

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
        private readonly TraffkGlobalDbContext GDB;

        public JobsController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger,
            IBackgroundJobClient backgrounder,
            TraffkGlobalDbContext gdb
            )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, logger)
        {
            Backgrounder = backgrounder;
            GDB = gdb;
        }

        [ActionName(ActionNames.Jobs)]
        [Route("/Jobs")]
        public IActionResult Jobs(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var items = Rdb.Job.Where(j => j.TenantId == TenantId);
            items = ApplyBrowse(items, sortCol ?? nameof(Traffk.Bal.Data.Rdb.TraffkTenantModel.Job.CreatedAt),
                sortDir ?? AspHelpers.SortDirDescending, page, pageSize);
            return View(ViewNames.JobList, items);
        }

        [ActionName(ActionNames.Job)]
        [Route("/Jobs/{id}")]
        public IActionResult JobDetails(int id)
        {
            return RedirectToIndex();

        }

        [ActionName(ActionNames.JobCancel)]
        [HttpDelete]
        [Route("/Jobs/Cancel")]
        public async Task<IActionResult> CancelJob()
        {
            return await JsonCancelJobAsync<int>();
        }

        private async Task<Traffk.Bal.Data.Rdb.TraffkTenantModel.Job> FindJobAsync(int jobId)
            => await Rdb.Job.FirstOrDefaultAsync(j => j.Id == jobId && j.TenantId == this.TenantId);

        private async Task<IActionResult> JsonCancelJobAsync<TId>()
        {
            var rawIds = Request.BodyAsJsonObject<TId[]>();
            var ids = rawIds.ConvertAll(x => Convert.ToInt32(x));

            if (ids != null)
            {
                int numDeleted = 0;
                foreach (var jobId in ids)
                {
                    var job = await FindJobAsync(jobId);
                    if (job.CanBeCancelled)
                    {
                        Backgrounder.ChangeState(jobId.ToString(), new CancelledState());
                        numDeleted++;
                    }
                }
                if (numDeleted > 0)
                {
                    return NoContent();
                }
            }
            return NoContent();
        }
    }
}
