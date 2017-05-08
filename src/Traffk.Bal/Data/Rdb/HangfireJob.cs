using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hangfire.States;
using RevolutionaryStuff.Core;
using Traffk.Bal.BackgroundJobs;

namespace Traffk.Bal.Data.Rdb
{
    [Table("Job", Schema = "Hangfire")]
    public class HangfireJob : IRdbDataEntity, IPrimaryKey<int>
    {
        int IPrimaryKey<int>.Key { get { return JobId; } }

        object IPrimaryKey.Key { get { return JobId; } }

        [Key]
        [Description("Foreign key to the hangfire job id")]
        [DisplayName("Job Id")]
        [Column("Id")]
        public int JobId { get; set; }

        [DisplayName("Status")]
        [Column("StateName")]
        public string Status { get; set; }

        [DisplayName("Created At")]
        [Column("CreatedAt")]
        public DateTime CreatedAtUtc { get; set; }

        [Column("InvocationData")]
        public string HangfireJobDetailsJson { get; set; }

        [NotMapped]
        public bool CanBeCancelled {
            get
            {

                if (Status.Equals(FailedState.StateName) || Status.Equals(SucceededState.StateName) || Status.Equals(DeletedState.StateName))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [NotMapped]
        public HangfireJobDetails HangfireJobDetails
        {
            get
            {
                if (HangfireJobDetails_p == null)
                {
                    HangfireJobDetails_p = HangfireJobDetails.CreateFromJson(HangfireJobDetailsJson) ?? new HangfireJobDetails();
                }
                return HangfireJobDetails_p;
            }
            set { HangfireJobDetails_p = value; }
        }

        private HangfireJobDetails HangfireJobDetails_p;
    }
}
