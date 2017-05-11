using Hangfire.States;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Traffk.Bal.BackgroundJobs;

namespace Traffk.Bal.Data.Rdb
{
    public partial class Job
    {
        private static readonly HashSet<string> CancellableStateNames = new HashSet<string> { FailedState.StateName, SucceededState.StateName, DeletedState.StateName };

        [NotMapped]
        public bool CanBeCancelled => CancellableStateNames.Contains(this.StateName);

        [NotMapped]
        public HangfireJobDetails HangfireJobDetails
        {
            get
            {
                if (HangfireJobDetails_p == null)
                {
                    HangfireJobDetails_p = HangfireJobDetails.CreateFromJson(InvocationData) ?? new HangfireJobDetails();
                }
                return HangfireJobDetails_p;
            }
        }

        private HangfireJobDetails HangfireJobDetails_p;
    }
}
