using System.Linq;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.ReportVisuals;
using Traffk.Utility;

namespace Traffk.Portal.Models.ReportingModels
{
    public interface IReportViewModel
    {
        long Id { get; set; }
        string Title { get; set; }
        string FolderName { get; set; }
        string WorkbookName { get; set; }
        string ViewName { get; set; }
        string WorksheetName { get; set; }
        string Description { get; set; }
        ReportDetails.RenderingAttributeFlags RenderingAttributes { get; }
        SerializableTreeNode<Note> Notes { get; set; }
        SerializableTreeNode<ReportVisualFolder> RelatedReports { get; set; }
    }

    public class TableauReportViewModel : IReportViewModel
    {
        public TableauReportViewModel()
        { }

        public TableauReportViewModel(IReportVisual reportVisual, SerializableTreeNode<ReportVisualFolder> relatedReports = null)
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
            RelatedReports = relatedReports;
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string FolderName { get; set; }
        public string WorkbookName { get; set; }
        public string ViewName { get; set; }
        public string WorksheetName { get; set; }
        public string Description { get; set; }
        public ReportDetails.RenderingAttributeFlags RenderingAttributes { get; private set; }
        public SerializableTreeNode<Note> Notes { get; set; }
        public SerializableTreeNode<ReportVisualFolder> RelatedReports { get; set; }
        long IReportViewModel.Id
        {
            get => Id;
            set => Id = value;
        }
        string IReportViewModel.Title
        {
            get => Title;
            set => Title = value;
        }
        string IReportViewModel.FolderName
        {
            get => FolderName;
            set => FolderName = value;
        }
        string IReportViewModel.WorkbookName
        {
            get => WorkbookName;
            set => WorkbookName = value;
        }
        string IReportViewModel.ViewName
        {
            get => ViewName;
            set => ViewName = value;
        }
        string IReportViewModel.WorksheetName
        {
            get => WorksheetName;
            set => WorksheetName = value;
        }
        string IReportViewModel.Description
        {
            get => Description;
            set => Description = value;
        }

        ReportDetails.RenderingAttributeFlags IReportViewModel.RenderingAttributes => RenderingAttributes;

        SerializableTreeNode<Note> IReportViewModel.Notes
        {
            get => Notes;
            set => Notes = value;
        }
        SerializableTreeNode<ReportVisualFolder> IReportViewModel.RelatedReports
        {
            get => RelatedReports;
            set => RelatedReports = value;
        }
    }
}
