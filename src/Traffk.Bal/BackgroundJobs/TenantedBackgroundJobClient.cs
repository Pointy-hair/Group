using System;
using Hangfire;
using Hangfire.States;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;

namespace Traffk.Bal.BackgroundJobs
{
    public class TenantedBackgroundJobClient : IBackgroundJobClient, ITraffkRecurringJobManager
    {
        private BackgroundJobClient Inner;
        private TraffkGlobalDbContext GDB;
        private ITraffkTenantFinder Finder;
        private RecurringJobManager RecurringJobManager;
        private ICurrentUser CurrentUser;

        public TenantedBackgroundJobClient(TraffkGlobalDbContext gdb, ITraffkTenantFinder finder, ICurrentUser currentUser = null)
        {
            Inner = new BackgroundJobClient();
            GDB = gdb;
            Finder = finder;
            CurrentUser = currentUser;
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
            j.ContactId = CurrentUser?.User?.ContactId;
            GDB.SaveChanges();
            return jobId;
        }

        string ITraffkRecurringJobManager.Add(Hangfire.Common.Job job, string cronExpression)
        {
            var recurringJobId = GetRecurringJobId();
            RecurringJobManager.AddOrUpdate(recurringJobId, job, cronExpression, new RecurringJobOptions());
            //Look at lines 35-36 and add new rows to store the TenantId/ContactId related to the recurringJob
            return recurringJobId;
        }

        void ITraffkRecurringJobManager.Update(string recurringJobId, Hangfire.Common.Job job, string cronExpression)
        {
            //TODO: Make sure recurringJobId actually exists - need to read Hangfire.Hash table

        }

        private string GetRecurringJobId()
        {
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            var contactId = CurrentUser.User.ContactId;
            var id = $"{tenantId}--{contactId}--{Guid.NewGuid().ToString()}"; 
            return id;
        }
    }
}
