using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Data.Rdb
{
    public enum JobTypes
    {
        CommunicationBlast,
        CreateContactsFromEligibility,
        FiscalYearConversion
    }

    public enum JobStatuses
    {
        NonYetQueued,
        Queued,
        Dequeued,
        Running,
        CompletedSuccess,
        CompletedError,
        Cancelled,
        Cancelling,
    }

    public partial class Job
    {
        [NotMapped]
        public bool CanBeCancelled
        {
            get
            {
                switch (JobStatus)
                {
                    case JobStatuses.NonYetQueued:
                    case JobStatuses.Queued:
                    case JobStatuses.Dequeued:
                    case JobStatuses.Running:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static Job CreateFiscalYearConversionJob(Tenant tenant, FiscalYearSettings jobData)
        {
            var job = new Job
            {
                Tenant = tenant,
                JobStatus = JobStatuses.Queued,
                JobType = JobTypes.FiscalYearConversion,
                JobData = jobData.ToJson()
            };
            return job;
        }
    }
}
