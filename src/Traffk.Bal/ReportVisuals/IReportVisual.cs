using System;
using System.Collections;
using System.Collections.Generic;

namespace Traffk.Bal.ReportVisuals
{
    public interface IReportVisual
    {
        long Id { get; set; } //Unique Id - primarily for use in URLs
        string Title { get; set; }
        string Description { get; set; }
        string ExternalReportKey { get; set; } //For connection to Tableau Reports or even embeddable reports from other sources in future
        string FolderPath { get; set; } //similar to Windows folder paths - used to construct Report Index UI
        string PreviewImageUrl { get; set; } //not URI so that we can use relative paths - e.g. /Report/PreviewImage/{Id}
        ICollection<string> Tags { get; set; } //for user to add tags
        bool ContainsPhi { get; set; } //PHI vs. Non PHI
        int? ParentId { get; set; } //can be null if no parent
        long? OwnerContactId { get; set; }
        bool Shared { get; set; } //User can choose to share reports with other users on the tenant, if shared and user is not owner, then FolderPath is /Shared
        bool Favorite { get; set; } //Show favorite reports on home screen
        ICollection<KeyValuePair<string, string>> Parameters { get; set; } //Any necessary parameters e.g. (ContactId, 9), (WorkbookName, Risk Index), (ViewName, Risk Index)
        bool CanExport { get; set; }
        DateTime LastEdit { get; set; } //For user convenience
        string LastEditedField { get; set; } //For user convenience
        VisualContext VisualContext { get; set; }
        string FolderName { get; }
        ReportDetails.RenderingAttributeFlags? RenderingAttributes { get; set; }
    }

    public enum VisualContext
    {
        ContactPerson,
        ContactProvider,
        Tenant
    }

    public enum TenantType
    {
        Underwriter,
        Employer
    }
}
