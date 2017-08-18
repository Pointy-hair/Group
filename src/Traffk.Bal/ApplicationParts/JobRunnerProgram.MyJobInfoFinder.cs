namespace Traffk.Bal.ApplicationParts
{
    public partial class JobRunnerProgram
    {
        private class JobInfo : IJobInfo
        {
            public int JobId { get; set; }
            public int? ParentJobId { get; set; }
            public string RecurringJobId { get; set; }
            public int? ContactId { get; set; }
            public int? TenantId { get; set; }

            public override string ToString()
                => $"{this.GetType().Name} jobId={JobId} tenantId={TenantId}";
        }

        public class MyJobInfoFinder : IJobInfoFinder
        {
            public IJobInfo JobInfo { get; set; }

            public override string ToString()
                => $"{this.GetType().Name} jobId={JobInfo?.JobId} tenantId={JobInfo?.TenantId}";
        }
    }
}
