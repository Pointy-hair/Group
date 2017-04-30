﻿using Hangfire;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Traffk.Bal.Data.Rdb;
using Hangfire.Common;
using Hangfire.States;

namespace Traffk.Bal.BackgroundJobs
{
    public class TenantedBackgroundJobClient : IBackgroundJobClient
    {
        private BackgroundJobClient Inner;
        private TraffkGlobalContext GDB;
        private ITraffkTenantFinder Finder;

        public TenantedBackgroundJobClient(TraffkGlobalContext gdb, ITraffkTenantFinder finder)
        {
            Inner = new BackgroundJobClient();
            GDB = gdb;
            Finder = finder;
        }

        bool IBackgroundJobClient.ChangeState(string jobId, IState state, string expectedState)
            => Inner.ChangeState(jobId, state, expectedState);

        string IBackgroundJobClient.Create(Hangfire.Common.Job job, IState state)
        {
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            var jobId = Inner.Create(job, state);
            GDB.HangfireTenantMappings.Add(new HangfireTenantMap { JobId = int.Parse(jobId), TenantId = tenantId });
            GDB.SaveChanges();
            return jobId;
        }
    }
}