using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Bal.ReportVisuals
{
    public interface IReportResource
    {
        string Title { get; set; }
        string Description { get; set; }
    }
}
