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

namespace Traffk.Bal.BackgroundJobs
{
    //public class BackgroundJobRunner : IBackgroundJobRunner
    //{
    //    private readonly IServiceProvider Services;

    //    public BackgroundJobRunner(IServiceCollection services)
    //    {
    //        Services = services.BuildServiceProvider();
    //    }

    //    void IBackgroundJobRunner.ConvertFiscalYear(Job job, PerformContext context)
    //    {
    //        //string jobId = context.BackgroundJob.Id;
            
    //        ////backgroundJobTenantFinder = 

    //        //var dbOptions = Services.GetRequiredService<DbContextOptions<TraffkRdbContext>>();
    //        ////var dbContext = new TraffkRdbContext(dbOptions, backgroundJobTenantFinder, null)

    //        //var fiscalYearSettings = JsonConvert.DeserializeObject<FiscalYearSettings>(job.JobData);
    //        //var tenantIdParam = new SqlParameter("@tenantId", job.TenantId);
    //        //var baselineFiscalYearParam = new SqlParameter("@baselineFiscalYear", fiscalYearSettings.FiscalYear);
    //        //var baselineCalendarMonthParam = new SqlParameter("@baselineCalendarMonth", fiscalYearSettings.CalendarMonth);
    //        //var baselineCalendarYearParam = new SqlParameter("@baselineCalendarYear", fiscalYearSettings.CalendarYear);
            
    //        //dbContext.Database.ExecuteSqlCommandAsync(
    //        //    "FiscalYearsConfigure @tenantId, @baselineFiscalYear, @baselineCalendarMonth, @baselineCalendarYear", default(CancellationToken),
    //        //    tenantIdParam, baselineFiscalYearParam, baselineCalendarMonthParam, baselineCalendarYearParam).ExecuteSynchronously();
    //    }
    //}

    //public class BackgroundJobTenantFinder : ITraffkTenantFinder
    //{
    //    //public BackgroundJobTenantFinder(TraffkGlobalContext rdbContext) 
    //    //{
    //    //    
    //    //}

    //    Task<int> ITenantFinder<int>.GetTenantIdAsync()
    //    {
    //        //Return tenant Id here
    //        throw new NotImplementedException();
    //    }

    //    private int GetTenantIdFromJob(string jobId)
    //    {
    //        //Use global context to get the tenant Id
    //        throw new NotImplementedException();
    //    }
    //}
}
