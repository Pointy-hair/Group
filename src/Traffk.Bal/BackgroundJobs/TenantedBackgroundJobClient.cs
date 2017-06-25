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
        private readonly BackgroundJobClient Inner;
        private readonly TraffkGlobalDbContext GDB;
        private readonly ITraffkTenantFinder Finder;
        private readonly RecurringJobManager RecurringJobManager;
        private readonly ICurrentUser CurrentUser;
        private int TenantId;

        private const string ContactIdField = "ContactId";
        private const string TenantIdField = "TenantId";
        private const string RecurringJobPrefix = @"recurring-job:";

        public TenantedBackgroundJobClient(TraffkGlobalDbContext gdb, ITraffkTenantFinder finder, ICurrentUser currentUser = null)
        {
            Inner = new BackgroundJobClient();
            GDB = gdb;
            Finder = finder;
            CurrentUser = currentUser;
            RecurringJobManager = new RecurringJobManager();
            TenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
        }

        bool IBackgroundJobClient.ChangeState(string jobId, IState state, string expectedState)
            => Inner.ChangeState(jobId, state, expectedState);

        string IBackgroundJobClient.Create(Hangfire.Common.Job job, IState state)
        {
            var tenantId = TenantId;
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
            var rjContactIdRow = new RecurringJobRow
            {
                Key = GetRecurringJobIdForRowCreation(recurringJobId),
                Field = ContactIdField,
                Value = CurrentUser?.User?.ContactId.ToString()
            };
            var rjTenantIdRow = new RecurringJobRow
            {
                Key = GetRecurringJobIdForRowCreation(recurringJobId),
                Field = TenantIdField,
                Value = TenantId.ToString()
            };
            GDB.RecurringJobRow.Add(rjTenantIdRow);
            GDB.RecurringJobRow.Add(rjContactIdRow);
            GDB.SaveChanges();
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

        private static string GetRecurringJobIdForRowCreation(string recurringJobId)
        {
            return RecurringJobPrefix + recurringJobId;
        }
    }
}
