using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
        private readonly TraffkGlobalsContext GDB;

        public JobsController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILogger logger,
            IBackgroundJobClient backgrounder,
            TraffkGlobalsContext gdb
            )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, logger)
        {
            Backgrounder = backgrounder;
            GDB = gdb;
        }

        [AllowAnonymous]
        [Route("/Jobs/DS")]
        public IActionResult DS()
        {
            var ds = new DataSource
            {
                TenantId = 3,
                DataSourceSettings = new Traffk.Bal.Settings.DataSourceSettings
                {
                    FTP = new Traffk.Bal.Settings.DataSourceSettings.FtpSettings
                    {
                        CredentialsKeyUri = Traffk.Bal.Services.Vault.CommonSecretUris.TraffkFtpTodayCredentialsUri,
                        Hostname = "traffk.ftptoday.com",
                        Port = 22,
                        FolderPaths = new[] { "/U-HaulWorkday", "/U-HaulCarrum", "/bradm@uhaul.com" }
                    }
                }
            };
            GDB.DataSources.Add(ds);
            GDB.SaveChanges();
            Backgrounder.Enqueue<IDataSourceSyncJobs>(z => z.DataSourceFetch(ds.DataSourceId));
            return Ok();
        }

        [ActionName(ActionNames.Jobs)]
        [Route("/Jobs")]
        public IActionResult Jobs(string sortCol, string sortDir, int? page, int? pageSize)
        {
            var items = GDB.Job.Where(j => j.TenantId == TenantId);
            items = ApplyBrowse(items, sortCol ?? nameof(Job.CreatedAt),
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

        private async Task<Job> FindJobAsync(int jobId)
            => await GDB.Job.FirstOrDefaultAsync(j => j.Id == jobId && j.TenantId == this.TenantId);

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
