using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Linq;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Services;

namespace Traffk.Bal.ApplicationParts
{
    public partial class JobRunnerProgram
    {
        private class MyActivator : JobActivator
        {
            private readonly IServiceProvider ServiceProvider; 

            public MyActivator(JobRunnerProgram runner)
            {
                ServiceProvider = runner.ServiceProvider.CreateScope().ServiceProvider;
            }

            public override JobActivatorScope BeginScope(JobActivatorContext context)
                => new MyScope(ServiceProvider.CreateScope().ServiceProvider, context);

            public class MyScope : JobActivatorScope
            {
                private readonly IServiceProvider SP;
                private readonly JobActivatorContext Context;

                private readonly JobInfo JobInfo = new JobInfo();

                public MyScope(IServiceProvider sp, JobActivatorContext context)
                {
                    SP = sp;
                    Context = context;

                    var gdb = sp.GetRequiredService<Data.Rdb.TraffkGlobal.TraffkGlobalDbContext>();
                    JobInfo.JobId = int.Parse(context.BackgroundJob.Id);
                    var j = gdb.Job.Find(JobInfo.JobId);
                    if (j != null)
                    {
                        JobInfo.TenantId = j.TenantId;
                        JobInfo.ParentJobId = j.ParentJobId;
                        JobInfo.RecurringJobId = j.RecurringJobId;
                        JobInfo.ContactId = j.ContactId;
                    }
                    if (JobInfo.TenantId == null)
                    {
                        JobInfo.RecurringJobId = JobInfo.RecurringJobId ?? context.GetJobParameter<string>("RecurringJobId");
                        if (JobInfo.RecurringJobId != null)
                        {
                            var z = BackgroundJobs.TenantedBackgroundJobClient.ParseRecurringJobId(JobInfo.RecurringJobId);
                            JobInfo.TenantId = z.TenantId;
                            JobInfo.ContactId = z.ContactId;
                        }
                    }
                    ((MyJobInfoFinder)sp.GetRequiredService<IJobInfoFinder>()).JobInfo = JobInfo;
                    var tenantFinder = (MyTraffkTenantFinder)sp.GetRequiredService<ITraffkTenantFinder>();
                    tenantFinder.TenantId = JobInfo.TenantId;
                    if (JobInfo.ContactId != null)
                    {
                        var currentUser = (MyCurrentUser)sp.GetRequiredService<ICurrentUser>();
                        var tdb = sp.GetRequiredService<TraffkTenantModelDbContext>();
                        currentUser.User = tdb.Users.FirstOrDefault(u=>u.TenantId==JobInfo.TenantId && u.ContactId==JobInfo.ContactId);
                    }
                }

                public override object Resolve(Type type)
                    => SP.GetService(type);
            }
        }
    }
}
