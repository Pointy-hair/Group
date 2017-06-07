namespace Traffk.Bal.BackgroundJobs
{
    public interface ITraffkRecurringJobManager
    {
        string Add(Hangfire.Common.Job job, string cronExpression);

        void Update(string recurringJobId, Hangfire.Common.Job job, string cronExpression);
    }
}
