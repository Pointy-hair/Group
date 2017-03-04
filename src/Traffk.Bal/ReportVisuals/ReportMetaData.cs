using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Bal.ReportVisuals
{
    public class ReportMetaData : IReportMetaData
    {
        public string ReportMetaDataId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReportVisualType { get; set; }
        public string ExternalReportId { get; set; }
        public string FolderPath { get; set; }
        public string PreviewImageUrl { get; set; }
        public ICollection<string> Tags { get; set; }
        public bool ContainsPhi { get; set; }
        public string ParentReportMetaDataId { get; set; }
        public bool Shared { get; set; }
        public bool Favorite { get; set; }
        public ICollection<KeyValuePair<string, string>> Parameters { get; set; }
        public bool CanExport { get; set; }
        public VisualContext VisualContext { get; set; }
        public DateTime LastEdit { get; set; }
        public string LastEditedField { get; set; }
        public string OwnerUserId { get; set; }
        public int TenantId { get; set; }

        public void Merge(IReportMetaData reportMetaData)
        {
            //Could be useful if merging in recent changes from parent RMD
        }

        public IReportMetaData Copy(IReportMetaData reportMetaData)
        {
            //Use reflection?
            return new ReportMetaData();
        }
    }
}
