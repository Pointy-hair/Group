using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.ComponentModel;

namespace Traffk.Bal.BackgroundJobs
{
    public class HangfireJobDetails
    {
        public static class JobDescriptions
        {
            public static string ReportDownload = "Report Download";
            public static string ScheduledReportDownload = "Scheduled Report Download";
        }

        public string Method { get; set; }

        [DisplayName("Job Description")]
        public string JobDescription
        {
            get
            {
                switch (Method)
                {
                    case nameof(ITenantJobs.DownloadTableauPdfContinuationJobAsync):
                        return JobDescriptions.ReportDownload;
                    case nameof(ITenantJobs.ScheduleTableauPdfDownload):
                        return JobDescriptions.ScheduledReportDownload;
                }

                return Method;
            }
        }

        public static HangfireJobDetails CreateFromJson(string json)
            => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<HangfireJobDetails>(json);

        public string ToJson() 
            => JsonConvert.SerializeObject(this);
    }
}
