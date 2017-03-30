using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Traffk.Bal.ReportVisuals
{
    internal class ReportVisual : IReportVisual, IReportResource
    {
        public string Title { get; set; }
        public string Description { get; set; }
        long IReportVisual.Id { get; set; }
        string IReportResource.Title
        {
            get { return Title; }
            set { Title = value; }
        }
        string IReportResource.Description
        {
            get { return Description; }
            set { Description = value; }
        }

        string IReportVisual.Title
        {
            get { return Title; }
            set { Title = value; }
        }
        string IReportVisual.Description
        {
            get { return Description; }
            set { Description = value; }
        }
        string IReportVisual.ExternalReportKey { get; set; }
        public string FolderPath { get; set; }
        string IReportVisual.FolderPath
        {
            get { return FolderPath; }
            set { FolderPath = value; }
        }
        string IReportVisual.PreviewImageUrl { get; set; }
        ICollection<string> IReportVisual.Tags { get; set; }
        bool IReportVisual.ContainsPhi { get; set; }
        int? IReportVisual.ParentId { get; set; }
        long? IReportVisual.OwnerContactId { get; set; }
        bool IReportVisual.Shared { get; set; }
        bool IReportVisual.Favorite { get; set; }
        ICollection<KeyValuePair<string, string>> IReportVisual.Parameters { get; set; }
        bool IReportVisual.CanExport { get; set; }
        DateTime IReportVisual.LastEdit { get; set; }
        string IReportVisual.LastEditedField { get; set; }
        VisualContext IReportVisual.VisualContext { get; set; }
        ReportDetails.RenderingAttributeFlags? IReportVisual.RenderingAttributes { get; set; }
        public string FolderName
        {
            get
            {
                var regex = new Regex(@"^\/(?<folderName>[^\\]+)$");
                var folderNameMatch = regex.Match(FolderPath);
                var folderName = folderNameMatch.Groups["folderName"].Value;
                if (String.IsNullOrEmpty(folderName))
                {
                    folderName = "Traffk Reports";
                }
                return folderName;
            }

        }
    }
}
