using Hangfire.Server;
using RevolutionaryStuff.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Threading;
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

        protected BaseJobRunner(TraffkGlobalContext globalContext, JobRunnerProgram jobRunnerProgram)
        {
            GlobalContext = globalContext;
            InstanceId = Interlocked.Increment(ref InstanceId_s);
            JobId = jobRunnerProgram.JobId ?? 0;

            LogContextProperties.Push(LogContext.PushProperty(nameof(InstanceId), InstanceId));
            LogContextProperties.Push(LogContext.PushProperty("JobId", JobId));
            LogContextProperties.Push(LogContext.PushProperty("Type", this.GetType().Name));

            Logger = Log.Logger;

            Logger.Information(ConstructedText);

            //Console.WriteLine("Constructed {0} Thread Id {1}", this.GetType().Name, JobId);
        }

        protected void PostResult(object resultObject)
        {
            var jobResult = new HangfireJobResult
            {
                JobId = this.JobId,
                JobResultDetails = resultObject
            };
            //GlobalContext.JobResults.Add(jobResult)
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
            while (LogContextProperties.Count > 0)
            {
                LogContextProperties.Pop().Dispose();
            }
            base.OnDispose(disposing);
        }
    }
}
