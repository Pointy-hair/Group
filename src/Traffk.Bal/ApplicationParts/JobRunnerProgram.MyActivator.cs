using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core.Caching;
using System;
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
                public int? JobId { get; private set; }
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
                            ContactId = j.ContactId;
                        }

                        if ((TenantId == null || ContactId == null) && context.GetJobParameter<string>("RecurringJobId") != null)
                        {
                            var tenantIdContactIdRegex = new Regex(@"^(?<tenantId>[0-9]+)\-\-(?<contactId>[0-9]+)\-\-([a-zA-Z0-9\-]{36})$");
                            var recurringJobId = context.GetJobParameter<string>("RecurringJobId");
                            var matches = tenantIdContactIdRegex.Matches(recurringJobId);
                            foreach (Match m in matches)
                            {
                                var tenantId = int.Parse(m.Groups["tenantId"].ToString());
                                var contactId = int.Parse(m.Groups["contactId"].ToString());

                                if (tenantId > 0)
                                {
                                    TenantId = tenantId;
                                }

                                if (contactId > 0)
                                {
                                    ContactId = contactId;
                                }
                            }
                        }

                        if (ContactId != null && ContactId > 0)
                        {
                            CurrentUser = this.Activator.Runner.TenantDB.Users.Single(x => x.ContactId == ContactId);
                        }
                    }
                    catch (Exception e) { }
                }

                public override object Resolve(Type type)
                    => SP.GetService(type);
            }
        }
    }
}
