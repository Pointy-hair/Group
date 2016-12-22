using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
            public const string Cancelling = "Cancelling";

            public static readonly ICollection<string> CancellableStatusNames = new List<string>(new[] { NonYetQueued, Queued, Dequeued, Running }).AsReadOnly();
        }

        [NotMapped]
        public bool CanBeCancelled => StatusNames.CancellableStatusNames.Contains(this.JobStatus);
    }
}
