using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Bal.ReportVisuals
{
    public class ReportVisualFolder : IReportVisualFolder, IReportResource
    {
        public ReportVisualFolder(string title)
        {
            Title = title;
        }
        public string Title { get; set; }
        string IReportResource.Title
        {
            get { return Title; }
            set { Title = value; }
        }
        string IReportVisualFolder.Title
        {
            get { return Title; }
            set { Title = value; }
        }
        public string Description { get; set; }
    }
}
