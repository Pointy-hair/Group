using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;

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

                public MyScope(MyActivator activator, IServiceProvider sp, JobActivatorContext context)
                {
                    Activator = activator;
                    SP = sp;
                    Context = context;
                    Activator.Runner.CurrentThreadScope = this;
                    try
                    {
                        var jobId = int.Parse(context.BackgroundJob.Id);
                        var m = this.Activator.Runner.GDB.HangfireTenantMappings.FirstOrDefaultAsync(z => z.JobId == jobId).ExecuteSynchronously();
                        if (m != null)
                        {
                            TenantId = m.TenantId;
                        }
                    }
                    catch (Exception) { }
                }

                public override object Resolve(Type type)
                    => SP.GetService(type);
            }
        }
    }
}
