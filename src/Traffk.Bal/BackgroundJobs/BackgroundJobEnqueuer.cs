using Hangfire;
using Microsoft.Extensions.Options;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;

namespace Traffk.Bal.BackgroundJobs
{
    public class BackgroundJobEnqueuer
    {
        private readonly ITraffkTenantFinder TraffkTenantFinder;
        private readonly BackgroundJobEnqueuerOptions Options;
        private readonly IBackgroundJobRunner BackgroundJobRunner;

        public BackgroundJobEnqueuer(ITraffkTenantFinder traffkTenantFinder,
            IOptions<BackgroundJobEnqueuerOptions> options,
            IBackgroundJobRunner backgroundJobRunner)
        {
            TraffkTenantFinder = traffkTenantFinder;
            Options = options.Value;
            BackgroundJobRunner = backgroundJobRunner;
        }

        public string ConvertFiscalYear(Tenant tenant, FiscalYearSettings fiscalYearSettings)
        {
            var job = Job.CreateFiscalYearConversionJob(tenant, fiscalYearSettings);
            var jobId = BackgroundJob.Enqueue(() => BackgroundJobRunner.ConvertFiscalYear(job));
            return jobId;
        }
    }

}
