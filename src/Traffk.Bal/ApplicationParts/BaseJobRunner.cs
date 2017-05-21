using Hangfire.Server;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Traffk.Bal.Data.Rdb;
using ILogger = Serilog.ILogger;

namespace Traffk.Bal.ApplicationParts
{
    public abstract class BaseJobRunner : BaseDisposable
    {
        protected readonly Stack<IDisposable> LogContextProperties = new Stack<IDisposable>();
        protected readonly ILogger Logger;
        protected readonly int InstanceId;
        protected readonly int JobId;

        protected readonly string ConstructedText = "Constructed";
        protected readonly string DisposedText = "Disposed";

        protected readonly TraffkGlobalsContext GlobalContext;
        protected readonly PerformContext PerformContext;

        protected readonly DateTime StartedAtUtc = DateTime.UtcNow;

        private static int InstanceId_s;

        protected BaseJobRunner(TraffkGlobalsContext globalContext, 
            JobRunnerProgram jobRunnerProgram, 
            ILogger logger)
        {
            GlobalContext = globalContext;
            InstanceId = Interlocked.Increment(ref InstanceId_s);

            JobId = jobRunnerProgram.JobId ?? 0;

            Logger = logger.ForContext(new ILogEventEnricher[]
            {
                new PropertyEnricher(nameof(InstanceId), InstanceId),
                new PropertyEnricher(nameof(JobId), JobId),
                new PropertyEnricher(typeof(Type).Name, this.GetType().Name),
            });

            Logger.Information(ConstructedText);
        }

        protected void PostResult(object resultObject)
            => PostResult(JsonConvert.SerializeObject(resultObject));

        protected void PostResult(string serializedResult)
        {
            if (serializedResult == null) return;
            var j = GlobalContext.Job.Find(JobId);
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
