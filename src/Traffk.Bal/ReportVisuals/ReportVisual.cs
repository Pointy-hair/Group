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
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ExternalReportId { get; set; }
        public string FolderPath { get; set; }
        public string PreviewImageUrl { get; set; }
        public ICollection<string> Tags { get; set; }
        public bool ContainsPhi { get; set; }
        public int? ParentId { get; set; }
        public long? OwnerContactId { get; set; }
        public bool Shared { get; set; }
        public bool Favorite { get; set; }
        public ICollection<KeyValuePair<string, string>> Parameters { get; set; }
        public bool CanExport { get; set; }
        public DateTime LastEdit { get; set; }
        public string LastEditedField { get; set; }
        public VisualContext VisualContext { get; set; }

        bool IReportVisual.CanExport
        {
            get { return CanExport; }
            set { CanExport = value; }
        }

        bool IReportVisual.ContainsPhi
        {
            get { return ContainsPhi; }
            set { ContainsPhi = value; }
        }

        long IReportVisual.Id
        {
            get { return Id; }
            set { Id = value; }
        }

        string IReportVisual.Description
        {
            get { return Description; }
            set { Description = value; }
        }

        string IReportVisual.ExternalReportKey
        {
            get { return ExternalReportId; }
            set { ExternalReportId = value; }
        }

        bool IReportVisual.Favorite
        {
            get { return Favorite; }
            set { Favorite = value; }
        }

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
