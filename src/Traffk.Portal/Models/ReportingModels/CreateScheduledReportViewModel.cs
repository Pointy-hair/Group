using Traffk.Bal.Data;

namespace Traffk.Portal.Models.ReportingModels
{
    public class CreateScheduledReportViewModel
    {
        public IReportViewModel ReportViewModel { get; set; }
        public RecurrenceSettings RecurrenceSettings { get; set; }


        public CreateScheduledReportViewModel()
        {
            //Required for model binding
        }

        public CreateScheduledReportViewModel(IReportViewModel reportViewModel,
            RecurrenceSettings recurrenceSettings)
        {
            ReportViewModel = reportViewModel;
            RecurrenceSettings = recurrenceSettings;
        }
    }
}
