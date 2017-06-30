using System;
using Hangfire.States;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Services;

namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
{
    public partial class Job
    {
        [NotMapped]
        public DateTime CreatedAtUtc => CreatedAt;

        private static readonly HashSet<string> CancellableStateNames = new HashSet<string> { EnqueuedState.StateName, ProcessingState.StateName, ScheduledState.StateName };

        [NotMapped]
        public bool CanBeCancelled => CancellableStateNames.Contains(this.StateName);

        [NotMapped]
        public bool Downloadable
        {
            get
            {
                if (String.IsNullOrEmpty(ResultData)) return false;
                var cloudFilePointer = JsonConvert.DeserializeObject<CloudFilePointer>(ResultData) ?? null;

                return (this.StateName == SucceededState.StateName && cloudFilePointer != null);
            }
        }

        public Uri DownloadLink
        {
            get
            {
                if (String.IsNullOrEmpty(ResultData)) return null;
                var cloudFilePointer = JsonConvert.DeserializeObject<CloudFilePointer>(this.ResultData) ?? null;
                return cloudFilePointer?.Uri;
            }
        }

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
