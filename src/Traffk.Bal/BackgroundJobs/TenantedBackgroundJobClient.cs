﻿using System;
using Hangfire;
using Hangfire.States;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;
using System.Text.RegularExpressions;

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
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            var contactId = CurrentUser?.User?.ContactId;
            var recurringJobId = GetRecurringJobId();
            RecurringJobManager.AddOrUpdate(recurringJobId, job, cronExpression, new RecurringJobOptions());
            GDB.Hash.Add(new HangfireHash { Key = "recurring-job:"+recurringJobId, Field = "TenantId", Value = tenantId.ToString() });
            if (contactId != null)
            {
                GDB.Hash.Add(new HangfireHash { Key = "recurring-job:" + recurringJobId, Field = "ContactId", Value = contactId.ToString() });
            }
            GDB.SaveChanges();
            return recurringJobId;
        }

        void ITraffkRecurringJobManager.Update(string recurringJobId, Hangfire.Common.Job job, string cronExpression)
        {
            //TODO: Make sure recurringJobId actually exists - need to read Hangfire.Hash table

        }

        public class RecurringJobIdContext
        {
            public string RecurringJobId { get; set; }
            public int? TenantId { get; set; }
            public int? ContactId { get; set; }
        }

        private static readonly Regex RecurringJobIdContextParsingExpr = new Regex(@"t:([^,]*),c:([^,]*),(.+)", RegexOptions.Compiled);

        public static RecurringJobIdContext ParseRecurringJobId(string recurringJobId)
        {
            var m = RecurringJobIdContextParsingExpr.Match(recurringJobId);
            var c = new RecurringJobIdContext
            {
                RecurringJobId = recurringJobId,
                TenantId = Parse.ParseNullableInt32(m.Success?m.Groups[1].Value : null),
                ContactId = Parse.ParseNullableInt32(m.Success ? m.Groups[2].Value : null),
            };
            return c;
        }

        private string GetRecurringJobId()
        {
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            var contactId = CurrentUser?.User?.ContactId;
            var id = $"t:{tenantId},c:{contactId},{Guid.NewGuid().ToString()}"; 
            return id;
        }
    }
}
