using System.Collections.Generic;

namespace Traffk.Bal.ReportVisuals
{
    public class ReportSearchCriteria
    {
        public IReportVisual BaseReportVisual { get; set; }
        public VisualContext VisualContext { get; set; }
        public IEnumerable<string> TagFilter { get; set; }
        public int ReportId { get; set; }

    }
}
