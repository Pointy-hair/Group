﻿using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Traffk.Bal.BackgroundJobs;
using TraffkPortal.Services;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkGlobal;

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
            ITraffkRecurringJobManager rjm,
            TraffkGlobalDbContext gdb
            )
            : base(AspHelpers.MainNavigationPageKeys.Setup, db, current, logger)
        {
            Backgrounder = backgrounder;
            GDB = gdb;
            RJM = rjm;
        }

        private readonly ITraffkRecurringJobManager RJM;

        [AllowAnonymous]
        [Route("/Jobs/LoadInternationalClassificationDiseases")]
        public IActionResult LoadInternationalClassificationDiseases(string msg)
        {
            Backgrounder.Enqueue<IEtlJobs>(z => z.LoadInternationalClassificationDiseasesAsync());
            return Ok();
        }


        [AllowAnonymous]
        [Route("/Jobs/CreateRecurringTrace/{msg}")]
        public IActionResult CreateRecurringTrace(string msg)
        {
//            Backgrounder.en
            RJM.Add(Hangfire.Common.Job.FromExpression(() => System.Diagnostics.Trace.WriteLine(msg)), Cron.Minutely());
            return Ok();
        }

        [AllowAnonymous]
        [Route("/Jobs/CreateTenant/{tenantName}")]
        public IActionResult CreateTenant(string tenantName)
        {
            if (tenantName == null) return BadRequest();
            var d = new TenantCreationDetails
            {
                AdminPassword = "1adminPassword",
                AdminUsername = "admin",
                TenantName = tenantName
            };
            Backgrounder.Enqueue<ITenantManagementJobs>(z => z.CreateTenant(d));
            return Ok();
        }

        [AllowAnonymous]
        [Route("/Jobs/ZipCodes")]
        public IActionResult ZipCodes()
        {
            var ds = new Traffk.Bal.Data.Rdb.TraffkGlobal.DataSource
            {
                TenantId = 3,
                DataSourceSettings = new Traffk.Bal.Settings.DataSourceSettings
                {
                    Web = new Traffk.Bal.Settings.DataSourceSettings.WebSettings
                    {
                        CredentialsKeyUri = Traffk.Bal.Services.Vault.CommonSecretUris.ZipCodesComCredentialsUri,
                        LoginPageConfig = new Traffk.Bal.Settings.DataSourceSettings.WebSettings.WebLoginPageConfig
                        {
                            LoginPage = new Uri("https://www.zip-codes.com/account_login.asp"),
                            UsernameFieldName = "loginUsername",
                            PasswordFieldName = "loginPassword"
                        },
                        DownloadUrls = new[] 
                        {
                            new Uri("https://www.zip-codes.com/account_database.asp?type=csv&product=25"), //CSV Delux DB
                            new Uri("https://www.zip-codes.com/account_database.asp?type=csv&product=38"), //CSV Delux DB with Business
                            new Uri("https://www.zip-codes.com/account_database.asp?type=cs&product=89"), //CSV Zip9
                        }
                    }
                }
            };
            GDB.DataSources.Add(ds);
            GDB.SaveChanges();
            Backgrounder.Enqueue<IDataSourceSyncJobs>(z => z.DataSourceFetchAsync(ds.DataSourceId));
//            RJM.Add(Hangfire.Common.Job.FromExpression<IDataSourceSyncJobs>(z => z.DataSourceFetchAsync(ds.DataSourceId)), Cron.Daily());
            return Ok();
        }

        [AllowAnonymous]
        [Route("/Jobs/DS")]
        public IActionResult DS()
        {
            var ds = new Traffk.Bal.Data.Rdb.TraffkGlobal.DataSource
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
            Backgrounder.Enqueue<IDataSourceSyncJobs>(z => z.DataSourceFetchAsync(ds.DataSourceId));
            return Ok();
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
