using System.Collections.Generic;

namespace Traffk.Bal.ReportVisuals
{
    public interface IReportVisualFolder
    {
        string Title { get; set; }
        List<ReportVisual> Reports { get; set; }
    }

    public class ReportVisualFolder : ReportResource, IReportVisualFolder
    {
        public ReportVisualFolder(string title)
        {
            Title = title;
        }


        string IReportVisualFolder.Title
        {
            get => Title;
            set => Title = value;
        }

        public List<ReportVisual> Reports { get; set; } = new List<ReportVisual>();

        List<ReportVisual> IReportVisualFolder.Reports
        {
            get => Reports;
            set => Reports = value;
        }
    }
}
