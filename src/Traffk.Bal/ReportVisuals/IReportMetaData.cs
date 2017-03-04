using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Traffk.Bal.ReportVisuals
{
    public interface IReportMetaData
    {
        //Shared to IReportVisual
        string ReportMetaDataId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string ReportVisualType { get; set; }
        string ExternalReportId { get; set; }
        string FolderPath { get; set; } //similar to Windows folder paths
        string PreviewImageUrl { get; set; } //not URI so that we can use relative paths
        ICollection<string> Tags { get; set; }
        bool ContainsPhi { get; set; }
        string ParentReportMetaDataId { get; set; } //can be null if no parent
        bool Shared { get; set; } //User can choose to share copies of reports
        bool Favorite { get; set; } //Show favorite reports on home screen
        ICollection<KeyValuePair<string, string>> Parameters { get; set; }
        //Any necessary parameters e.g. ContactId, 9
        //WorkbookName, Risk Index
        //ViewName, Risk Index
        bool CanExport { get; set; }
        VisualContext VisualContext { get; set; }
        DateTime LastEdit { get; set; }
        string LastEditedField { get; set; }
        string OwnerUserId { get; set; }

        //Stored in db but not sent to IReportVisual
        int TenantId { get; set; } //Foreign key on Tenants table    

        void Merge(IReportMetaData reportMetaData);
        IReportMetaData Copy(IReportMetaData reportMetaData);
    }
}
