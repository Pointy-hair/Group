using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core.ApplicationParts;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;
using System.Threading.Tasks;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Bal.BackgroundJobs
{
    public class BackgroundJobRunner : IBackgroundJobRunner
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IBackgroundJobTenantFinder TenantFinder;
        private readonly ITableauAdminService TableauAdminService;
        public IConfigurationRoot Configuration { get; private set; }

        private TraffkRdbContext RdbContext;

        public BackgroundJobRunner(
            IBackgroundJobTenantFinder tenantFinder, 
            ITableauAdminService tableauAdminService)
        {
            var env = PlatformServices.Default.Application;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ApplicationBasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            var services = new ServiceCollection {new ServiceDescriptor(typeof(IConfiguration), Configuration)};
            services.AddOptions();
            ServiceProvider = services.BuildServiceProvider();
            TenantFinder = tenantFinder;
            TableauAdminService = tableauAdminService;
        }

        public void ConvertFiscalYear(Job job)
        {
            ConvertFiscalYear(job, null);
        }

        private void ConvertFiscalYear(Job job, PerformContext context)
        {
            string jobId = context.BackgroundJob.Id;
            Requires.NonNull(jobId, nameof(jobId));
            SetDbContext(jobId);

            var fiscalYearSettings = JsonConvert.DeserializeObject<FiscalYearSettings>(job.JobData);
            var tenantIdParam = new SqlParameter("@tenantId", job.TenantId);
            var baselineFiscalYearParam = new SqlParameter("@baselineFiscalYear", fiscalYearSettings.FiscalYear);
            var baselineCalendarMonthParam = new SqlParameter("@baselineCalendarMonth", fiscalYearSettings.CalendarMonth);
            var baselineCalendarYearParam = new SqlParameter("@baselineCalendarYear", fiscalYearSettings.CalendarYear);

            RdbContext.Database.ExecuteSqlCommandAsync(
                "FiscalYearsConfigure @tenantId, @baselineFiscalYear, @baselineCalendarMonth, @baselineCalendarYear", default(CancellationToken),
                tenantIdParam, baselineFiscalYearParam, baselineCalendarMonthParam, baselineCalendarYearParam).ExecuteSynchronously();
        }

        private void CreateNewTableauTenant(CreateTableauTenantRequest request, PerformContext context)
        {
            string jobId = context.BackgroundJob.Id;
            Requires.NonNull(jobId, nameof(jobId));
            SetDbContext(jobId);

            //How do we prevent the BackgroundJobRunner from having many dependencies/do we care?
            TableauAdminService.CreateTableauTenant(request);

            //Rdb operations to save relevant Tableau info
        }

        private void SetDbContext(string jobId)
        {
            var dbOptions = ServiceProvider.GetRequiredService<DbContextOptions<TraffkRdbContext>>();
            TenantFinder.GetTenantIdUsingJobId(jobId);
            RdbContext = new TraffkRdbContext(dbOptions, TenantFinder, null);
        }
    }

    public class BackgroundJobTenantFinder : IBackgroundJobTenantFinder
    {
        private readonly TraffkGlobalContext RdbContext;
        private int TenantId;

        public BackgroundJobTenantFinder(TraffkGlobalContext rdbContext)
        {
            RdbContext = rdbContext;
        }

        Task<int> ITenantFinder<int>.GetTenantIdAsync() => Task.FromResult(TenantId);

        int IBackgroundJobTenantFinder.GetTenantIdUsingJobId(string jobId)
        {
            if (!String.IsNullOrEmpty(jobId))
            {
                TenantId = 3;
                //TenantId = RdbContext.Jobs.SingleOrDefault(x => x.JobId == jobId);
                return TenantId;
            }
            else
            {
                //Use global context to get the tenant Id
                throw new NotImplementedException();
            }
        }
    }
}
