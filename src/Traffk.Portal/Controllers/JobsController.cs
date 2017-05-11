﻿using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
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
        public IActionResult Jobs(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var jobIdsForTenant =
                GDB.HangfireTenantMappings.Where(x => x.TenantId == Current.TenantId).Select(y => y.JobId);
            var items = GDB.Jobs.AsNoTracking().Where(z => jobIdsForTenant.Contains(z.JobId));
            items = ApplyBrowse(items, sortCol ?? nameof(HangfireJob.CreatedAtUtc),
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

        private async Task<IActionResult> JsonCancelJobAsync<TId>()
        {
            var rawIds = Request.BodyAsJsonObject<TId[]>();
            var ids = rawIds.ConvertAll(x => Convert.ToInt32(x));

            if (ids != null)
            {
                var tenantsJobIdsToCancel = GDB.HangfireTenantMappings.Where(x => ids.Contains(x.JobId) && x.TenantId == Current.TenantId).Select(x => x.JobId);
                var cancellableJobs = GDB.Jobs.Where(j => tenantsJobIdsToCancel.Contains(j.JobId) && j.CanBeCancelled);
                int numDeleted = 0;

                foreach (var job in cancellableJobs)
                {
                    Backgrounder.ChangeState(job.JobId.ToString(), new CancelledState());
                    numDeleted++;
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
