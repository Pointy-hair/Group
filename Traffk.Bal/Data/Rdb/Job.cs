namespace Traffk.Bal.Data.Rdb
{
    public partial class Job
    {
        public static class JobTypes
        {
            public const string CommunicationBlast = "CommunicationBlast";
            public const string CreateContactsFromEligibility = "CreateContactsFromEligibility";
        }

        public static class StatusNames
        {
            public const string NonYetQueued = "NonYetQueued";
            public const string Queued = "Queued";
            public const string Dequeued = "Dequeued";
            public const string Running = "Running";
            public const string CompletedSuccess = "CompletedSuccess";
            public const string CompletedError = "CompletedError";
            public const string Cancelled = "Cancelled";
        }
    }
}
