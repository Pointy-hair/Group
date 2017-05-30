using System;
using System.ComponentModel.DataAnnotations.Schema;
using Traffk.Bal.BackgroundJobs;

namespace Traffk.Bal.Data.Rdb
{
    public partial class JobState
    {
        [NotMapped]
        public DateTime CreatedAtUtc => CreatedAt;

        [NotMapped]
        public JobStateDetails JobStateDetails
        {
            get
            {
                if (JobStateDetails_p == null)
                {
                    JobStateDetails_p = JobStateDetails.CreateFromJson(Data) ?? new JobStateDetails();
                }
                return JobStateDetails_p;
            }
        }

        private JobStateDetails JobStateDetails_p;
    }
}
