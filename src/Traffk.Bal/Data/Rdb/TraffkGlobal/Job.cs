using System;
using Hangfire.States;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Traffk.Bal.BackgroundJobs;

namespace Traffk.Bal.Data.Rdb.TraffkGlobal
{
    public partial class Job
    {
        [NotMapped]
        public DateTime CreatedAtUtc => CreatedAt;

        private static readonly HashSet<string> CancellableStateNames = new HashSet<string> { EnqueuedState.StateName, ProcessingState.StateName, ScheduledState.StateName };

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
