using Hangfire.Server;
using RevolutionaryStuff.Core;
using System;
using System.Threading;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using ILogger = Serilog.ILogger;

namespace Traffk.Bal.ApplicationParts
{
    public abstract class BaseJobRunner : BaseDisposable
    {
        protected readonly ILogger Logger;
        protected readonly int InstanceId;
        protected readonly IJobInfo JobInfo;

        protected readonly string ConstructedText = "Constructed";
        protected readonly string DisposedText = "Disposed";

        protected readonly TraffkGlobalDbContext GlobalContext;
        protected readonly PerformContext PerformContext;

        protected readonly DateTime StartedAtUtc = DateTime.UtcNow;

        private static int InstanceId_s;

        public string GetParentJobResultData()
        {
            var parentId = GlobalContext.Job.Find(JobInfo.JobId).ParentJobId;
            if (parentId == null) return null;
            return GlobalContext.Job.Find(parentId.Value).ResultData;
        }

        protected BaseJobRunner(
            TraffkGlobalDbContext globalContext,
            IJobInfoFinder jobInfoFinder,
            ILogger logger)
        {
            GlobalContext = globalContext;
            InstanceId = Interlocked.Increment(ref InstanceId_s);
            JobInfo = jobInfoFinder.JobInfo;

            Logger = logger.ForContext(new ILogEventEnricher[]
            {
                new PropertyEnricher(nameof(InstanceId), InstanceId),
                new PropertyEnricher(nameof(JobInfo.JobId), JobInfo.JobId),
                new PropertyEnricher(typeof(Type).Name, GetType().Name),
            });

            Logger.Information(ConstructedText);
        }

        protected void PostResult(object resultObject)
            => PostResult(JsonConvert.SerializeObject(resultObject));

        protected void PostResult(string serializedResult)
        {
            if (serializedResult == null) return;
            var j = GlobalContext.Job.Find(JobInfo.JobId);
            j.ResultData = serializedResult;
            GlobalContext.SaveChanges();
        }

        protected override void OnDispose(bool disposing)
        {
            Logger.Information(DisposedText);
            base.OnDispose(disposing);
        }
    }
}
