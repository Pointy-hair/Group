using Traffk.Bal.Data;

namespace Traffk.Portal.Models.ReportingModels
{
    public class CreateScheduledReportViewModel
    {
        public TableauReportViewModel TableauReportViewModel { get; set; }
        public RecurrenceSettings RecurrenceSettings { get; set; }


        public CreateScheduledReportViewModel()
        {
            //Required for model binding
        }

        public CreateScheduledReportViewModel(TableauReportViewModel tableauReportViewModel,
            RecurrenceSettings recurrenceSettings)
        {
            TableauReportViewModel = tableauReportViewModel;
            RecurrenceSettings = recurrenceSettings;
        }
    }
}
