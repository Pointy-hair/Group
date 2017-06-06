using System;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkGlobal;

namespace Traffk.Bal.BackgroundJobs
{
    public class TenantedBackgroundJobClient : IBackgroundJobClient, ITraffkRecurringJobManager
    {
        private BackgroundJobClient Inner;
        private TraffkGlobalDbContext GDB;
        private ITraffkTenantFinder Finder;
        private RecurringJobManager RecurringJobManager;

        public TenantedBackgroundJobClient(TraffkGlobalDbContext gdb, ITraffkTenantFinder finder)
        {
            Inner = new BackgroundJobClient();
            GDB = gdb;
            Finder = finder;
            RecurringJobManager = new RecurringJobManager();
        }

        bool IBackgroundJobClient.ChangeState(string jobId, IState state, string expectedState)
            => Inner.ChangeState(jobId, state, expectedState);

        string IBackgroundJobClient.Create(Hangfire.Common.Job job, IState state)
        {
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            var jobId = Inner.Create(job, state);
            var j = GDB.Job.Find(int.Parse(jobId));
            j.TenantId = tenantId;
            GDB.SaveChanges();
            return jobId;
        }

        string ITraffkRecurringJobManager.Add(Hangfire.Common.Job job, string cronExpression)
        {
            var recurringJobId = GetRecurringJobId();
            RecurringJobManager.AddOrUpdate(recurringJobId, job, cronExpression, new RecurringJobOptions());
            return recurringJobId;
        }

        void ITraffkRecurringJobManager.Update(string recurringJobId, Hangfire.Common.Job job, string cronExpression)
        {
            //TODO: Make sure recurringJobId actually exists - need to read Hangfire.Hash table

        }

        private string GetRecurringJobId()
        {
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            var id = $"{tenantId}-{Guid.NewGuid().ToString()}"; 
            return id;
        }
    }
}
