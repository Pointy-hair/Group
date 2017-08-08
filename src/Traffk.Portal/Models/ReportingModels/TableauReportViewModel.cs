using System.Linq;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.ReportVisuals;

namespace Traffk.Portal.Models.ReportingModels
{
    public class TableauReportViewModel
    {
        public TableauReportViewModel()
        { }

        public TableauReportViewModel(IReportVisual reportVisual)
        {
            Id = reportVisual.Id;
            WorkbookName = reportVisual.Parameters.SingleOrDefault(p => p.Key == nameof(WorkbookName)).Value;
            ViewName = reportVisual.Parameters.SingleOrDefault(p => p.Key == nameof(ViewName)).Value;
            WorksheetName = reportVisual.Parameters.SingleOrDefault(p => p.Key == nameof(WorksheetName)).Value;
            FolderName = reportVisual.FolderName;
            Description = reportVisual.Description;
            Title = reportVisual.Title;
            if (reportVisual.RenderingAttributes != null)
            {
                RenderingAttributes = reportVisual.RenderingAttributes.Value;
            }
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string FolderName { get; set; }
        public string WorkbookName { get; set; }
        public string ViewName { get; set; }
        public string WorksheetName { get; set; }
        public string Description { get; set; }
        public ReportDetails.RenderingAttributeFlags RenderingAttributes { get; private set; }
        public IQueryable<Note> Notes { get; set; }
    }
}
