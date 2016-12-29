using RevolutionaryStuff.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Traffk.Bal.Data.Ddb;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.ApplicationParts
{
    public abstract class JobRunner : BaseDisposable
    {
        private bool GoCalled;

        public async Task GoAsync(TraffkRdbContext db, Job job)
        {
            Requires.NonNull(job, nameof(job));
            Requires.SingleCall(ref GoCalled);

            try
            {
                job.JobStatus = JobStatuses.Running;
                var prev = job.JobResult;
                job.JobResult = CreateJobResult();
                job.JobResult.PreviousResult = prev;
                db.Update(job);
                await db.SaveChangesAsync();
                await OnGoAsync(job);
                job.JobStatus = JobStatuses.CompletedSuccess;
            }
            catch (Exception ex)
            {
                job.JobStatus = JobStatuses.CompletedError;
                job.JobResult.Error = new ExceptionError(ex);
            }
            db.Update(job);
            await db.SaveChangesAsync();
        }

        protected virtual JobResult CreateJobResult() => new JobResult();

        private readonly ManualResetEvent AbortEvent = new ManualResetEvent(false);

        private void Abort() => AbortEvent.Set();

        protected WaitHandle AbortHandle => AbortEvent;

        protected abstract Task OnGoAsync(Job job);
    }

    public abstract class JobRunner<TJobResult> : JobRunner where TJobResult : JobResult, new()
    {
        protected override JobResult CreateJobResult() => new TJobResult();

        protected override Task OnGoAsync(Job job) => OnGoAsync(job, (TJobResult)job.JobResult);

        protected abstract Task OnGoAsync(Job job, TJobResult result);
    }
}
