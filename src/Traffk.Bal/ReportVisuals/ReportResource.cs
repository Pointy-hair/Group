namespace Traffk.Bal.ReportVisuals
{
    public abstract class ReportResource : IReportResource
    {
        public string Title { get; set; }
        public string Description { get; set; }

        string IReportResource.Title
        {
            get => Title;
            set => Title = value;
        }

        string IReportResource.Description
        {
            get => Title;
            set => Title = value;
        }
    }
}
