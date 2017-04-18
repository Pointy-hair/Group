using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Threading;
using System;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;

namespace Traffk.Bal.ApplicationParts
{
    public static class JobRunnerProgram
    {
        public static void Main<TProgram>(string[] args) where TProgram : CommandLineProgram => CommandLineProgram.Main<TProgram>(args);
    }

    public abstract class JobRunnerProgram<TJobRunner> : CommandLineProgram where TJobRunner : JobRunner
    {
        [CommandLineSwitch(CommandLineSwitchAttribute.CommonArgNames.Threads, Mandatory = false)]
        public int ThreadCount = 1;

        [CommandLineSwitch(nameof(NoJobsReceivedTimeout), Mandatory = false)]
        public TimeSpan NoJobsReceivedTimeout = TimeSpan.FromMinutes(1);

        protected abstract JobTypes JobType { get; }

        protected async override Task OnGoAsync()
        {
            var dbOptions = ServiceProvider.GetRequiredService<DbContextOptions<TraffkRdbContext>>();

            using (var w = new WorkQueue(ThreadCount))
            {
                for (;;)
                {
                    if (w.FreeThreads > 0)
                    {
                        var db = new TraffkRdbContext(dbOptions, null, null);
                        throw new NotImplementedException();
#if false
                        var jobs = await db.JobDequeueAsync(JobType, Environment.MachineName, w.FreeThreads);
                        foreach (var job in jobs)
                        {
                            w.Enqueue(() => Do(db, job));
                        }
                        if (jobs.Items.Count == 0)
                        {
                            db.Dispose();
                            await Task.Delay(NoJobsReceivedTimeout);
                        }
#endif
                    }
                    await Task.Delay(250);
                }
            }
        }

        [ThreadStatic]
        private Job CurrentThreadsJob;

        private class ThreadStaticTenantFinder : ITraffkTenantFinder
        {
            private readonly JobRunnerProgram<TJobRunner> Context;

            public ThreadStaticTenantFinder(JobRunnerProgram<TJobRunner> context)
            {
                Context = context;
            }

            Task<int> ITenantFinder<int>.GetTenantIdAsync() => Task.FromResult(Context.CurrentThreadsJob.TenantId.Value);
        }

        private class NoCurrentUser : ICurrentUser
        {
            ApplicationUser ICurrentUser.User => null;
        }

        private void Do(TraffkRdbContext db, Job job)
        {
            try
            {
                CurrentThreadsJob = job;
                using (var scope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    using (var runner = sp.GetRequiredService<JobRunner>())
                    {
                        runner.GoAsync(db, job).ExecuteSynchronously();
                    }
                }
            }
            finally
            {
                CurrentThreadsJob = null;
            }
        }

        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.Configure<BlobStorageServices.BlobStorageServicesOptions>(Configuration.GetSection(nameof(BlobStorageServices.BlobStorageServicesOptions)));

            services.AddDbContext<TraffkRdbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<JobRunner, TJobRunner>();
            services.AddSingleton<ITraffkTenantFinder>(new ThreadStaticTenantFinder(this));
            services.AddScoped<CurrentTenantServices>();
            services.AddSingleton<ICurrentUser>(new NoCurrentUser());
            services.AddScoped<BlobStorageServices>();
        }
    }
}
