namespace Traffk.Tableau.REST.Models
{
    public interface ITableauReportVisual
    {
        string Id { get; }
        string WorkbookId { get; }
        string WorkbookName { get; }
        string ViewName { get; }
        string Title { get; }
    }

    public interface ITableauContactReportVisual : ITableauReportVisual
    {
        string ContactId { get; set; }
    }
}
