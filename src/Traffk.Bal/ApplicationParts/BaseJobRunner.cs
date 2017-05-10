using Hangfire.Server;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
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

        protected readonly TraffkGlobalContext GlobalContext;
        protected readonly PerformContext PerformContext;

        private static int InstanceId_s;

        protected BaseJobRunner(TraffkGlobalContext globalContext, 
            JobRunnerProgram jobRunnerProgram, 
            ILogger logger)
        {
            GlobalContext = globalContext;
            InstanceId = Interlocked.Increment(ref InstanceId_s);

            JobId = jobRunnerProgram.JobId ?? 0;

            Logger = new LoggerConfiguration().
                Enrich.WithProperty(nameof(InstanceId), InstanceId).
                Enrich.WithProperty("JobId", JobId).
                Enrich.WithProperty("Type", this.GetType().Name).
                WriteTo.Logger(logger).CreateLogger();

            Logger.Information(ConstructedText);
        }

        protected void PostResult(object resultObject)
        {
            var serializedResult = JsonConvert.SerializeObject(resultObject);
            PostResult(serializedResult);
        }

        protected void PostResult(string serializedResult)
        {
            var jobResult = new HangfireJobResult
            {
                JobId = this.JobId,
                JobResultDetailsJson = serializedResult
            };
            //GlobalContext.JobResults.Add(jobResult)
        }

        protected override void OnDispose(bool disposing)
        {
            Logger.Information(DisposedText);
            base.OnDispose(disposing);
        }
    }
}
