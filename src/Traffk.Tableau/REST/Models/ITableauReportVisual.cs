using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Tableau.REST.Models
{
    public interface ITableauReportVisual
    {
        string Id { get; }
        string WorkbookId { get; }
        string WorkbookName { get; }
        string ViewName { get; }
    }

    public interface ITableauContactReportVisual : ITableauReportVisual
    {
        string ContactId { get; set; }
    }
}
