using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.ApplicationParts
{
    public partial class JobRunnerProgram
    {
        private class MyActivator : JobActivator
        {
            private readonly JobRunnerProgram Runner;

            public MyActivator(JobRunnerProgram runner)
            {
                Runner = runner;
            }

            public override JobActivatorScope BeginScope(JobActivatorContext context)
                => new MyScope(this, this.Runner.ServiceProvider.CreateScope().ServiceProvider, context);

            public class MyScope : JobActivatorScope
            {
                private readonly MyActivator Activator;
                private readonly IServiceProvider SP;
                private readonly JobActivatorContext Context;

                public int? TenantId { get; private set; }
                public int? ParentJobId { get; private set; }
                public int? JobId { get; private set; }
                public string RecurringJobId { get; private set; }
                public int? ContactId { get; private set; }
                public ApplicationUser CurrentUser { get; private set; }

                public MyScope(MyActivator activator, IServiceProvider sp, JobActivatorContext context)
                {
                    Activator = activator;
                    SP = sp;
                    Context = context;
                    Activator.Runner.CurrentThreadScope = this;
                    try
                    {
                        var jobId = int.Parse(context.BackgroundJob.Id);
                        var j = this.Activator.Runner.GDB.Job.Find(jobId);
                        if (j != null)
                        {
                            TenantId = j.TenantId;
                            JobId = j.Id;
                            ParentJobId = j.ParentJobId;
                            RecurringJobId = j.RecurringJobId;
                            ContactId = j.ContactId;
                        }
                        if (TenantId == null || ContactId == null)
                        {
                            RecurringJobId = RecurringJobId ?? context.GetJobParameter<string>("RecurringJobId");
                            if (RecurringJobId != null)
                            {
                                var z = BackgroundJobs.TenantedBackgroundJobClient.ParseRecurringJobId(RecurringJobId);
                                TenantId = z.TenantId;
                                ContactId = z.ContactId;
                            }
                        }
                        if (ContactId != null && ContactId > 0)
                        {
                            CurrentUser = this.Activator.Runner.TenantDB.Users.FirstOrDefault(x => x.ContactId == ContactId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }

                public override object Resolve(Type type)
                    => SP.GetService(type);
            }
        }
    }
}
