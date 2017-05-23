using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using RevolutionaryStuff.Core;
using System;

namespace Traffk.Bal.BackgroundJobs
{
    public class TraffkJobFilterAttribute : JobFilterAttribute, IApplyStateFilter
    {
        public IConfigurationRoot Configuration { get; private set; }

        private readonly TimeSpan JobExpirationTimeout;
        private readonly TimeSpan DefaultJobExpiration = TimeSpan.FromDays(3650);

        public TraffkJobFilterAttribute(IConfigurationRoot configuration)
        {
            Configuration = configuration;

            JobExpirationTimeout = Parse.ParseTimeSpan(Configuration["JobRunner:JobExpirationTimeout"], DefaultJobExpiration);
        }

        void IApplyStateFilter.OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = JobExpirationTimeout;
        }

        void IApplyStateFilter.OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = JobExpirationTimeout;
        }
    }
}
