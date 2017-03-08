using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Permissions;
using Traffk.Bal.Services;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;

namespace Traffk.Bal.ReportVisuals
{
    public interface IReportVisualService
    {
        IEnumerable<ReportVisual> GetReportVisuals(VisualContext visualContext);

        byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId);
    }

    public class ReportVisualService : IReportVisualService
    {
        private ITableauRestService TableauRestService;
        private TraffkRdbContext Rdb;
        private ITableauTenantFinder TableauTenantFinder;
        private ICurrentUser CurrentUser;
        private AuthorizationHandlerContext AuthorizationHandlerContext;
        private ICollection<ReportMetaData> MockReportMetaData;
        private bool CanAccessPhi;

        //Do we make these private?
        public string TableauTenantId { get; private set; }

        public ReportVisualService(ITableauRestService tableauRestService, TraffkRdbContext rdb, 
            ITableauTenantFinder tableauTenantFinder, ICurrentUser currentUser, AuthorizationHandlerContext authorizationHandlerContext)
        {
            TableauRestService = tableauRestService;
            Rdb = rdb;
            TableauTenantFinder = tableauTenantFinder;

            TableauTenantId = TableauTenantFinder.GetTenantIdAsync().Result;
            CurrentUser = currentUser;
            AuthorizationHandlerContext = authorizationHandlerContext;

            MockReportMetaData = new Collection<ReportMetaData>();

            //Can we move to higher level?
            var permission = AuthorizationHandlerContext.User.FindFirst(c => c.Type == Permissions.PermissionNames.ProtectedHealthInformation.ToString());
            var pcv = PermissionClaimValue.CreateFromJson(permission.Value);
            CanAccessPhi = pcv.Granted;
        }

        public IEnumerable<ReportVisual> GetReportVisuals(VisualContext visualContext)
        {
            var tableauReports = GetTableauReportVisuals();
            var tableauReportVisuals = tableauReports as IList<ITableauReportVisual> ?? tableauReports.ToList();
            var tableauReportIds = tableauReportVisuals.Select(t => t.Id);

            var relevantReportMetaDatas = GetRelevantReportMetaDatas().Where(x => tableauReportIds.Contains(x.ExternalReportId) && x.VisualContext == visualContext);
            
            List<ReportVisual> visuals = new List<ReportVisual>();
            foreach (var rmd in relevantReportMetaDatas)
            {
                //could there be 2 tableau reports that have same ExternalReportId??
                var tableauReport = tableauReportVisuals.SingleOrDefault(x => x.Id == rmd.ExternalReportId);
                var visual = (ReportVisual)Merge(tableauReport, rmd);
                visuals.Add(visual);
            }

            return visuals;
        }
        
        private IReportVisual Merge(ITableauReportVisual tableauReportVisual, IReportMetaData reportMetaData)
        {
            Requires.NonNull(reportMetaData.Description, nameof(reportMetaData.Description));

            const string siteIdKey = "SiteId";

            var workbookName = new KeyValuePair<string, string>(nameof(tableauReportVisual.WorkbookName), tableauReportVisual.WorkbookName);
            var workbookId = new KeyValuePair<string, string>(nameof(tableauReportVisual.WorkbookId), tableauReportVisual.WorkbookId);
            var viewId = new KeyValuePair<string, string>(nameof(tableauReportVisual.Id), tableauReportVisual.Id);
            var viewName = new KeyValuePair<string, string>(nameof(tableauReportVisual.ViewName), tableauReportVisual.ViewName);
            var siteId = new KeyValuePair<string, string>(siteIdKey, reportMetaData.TenantId.ToString());
            var parameters = new Collection<KeyValuePair<string, string>>
            {
                workbookName,
                workbookId,
                viewId,
                viewName,
                siteId,
            };

            var visual = new ReportVisual()
            {
                CanExport = reportMetaData.CanExport,
                ContainsPhi = reportMetaData.ContainsPhi,
                Description = reportMetaData.Description ?? "Description via ReportVisualService",
                ExternalReportId = tableauReportVisual.Id,
                Favorite = reportMetaData.Favorite,
                FolderPath = reportMetaData.FolderPath ?? "/",
                Id = reportMetaData.ReportMetaDataId,
                LastEdit = reportMetaData.LastEdit,
                LastEditedField = reportMetaData.LastEditedField,
                OwnerUserId = reportMetaData.OwnerUserId,
                Parameters = reportMetaData.Parameters ?? parameters,
                ParentId = reportMetaData.ParentReportMetaDataId,
                PreviewImageUrl = reportMetaData.PreviewImageUrl ??
                                  $"/Reporting/PreviewImage/{tableauReportVisual.WorkbookId}/{tableauReportVisual.Id}",
                Shared = reportMetaData.Shared,
                Tags = reportMetaData.Tags,
                Title = reportMetaData.Title ?? tableauReportVisual.ViewName,
                VisualContext = reportMetaData.VisualContext               
            };

            return visual;
        }

        private IEnumerable<ITableauReportVisual> GetTableauReportVisuals()
        {
            if (TableauTenantId == null)
            {
                var visuals = TableauRestService.DownloadViewsForSite().Views.ToList();
                var random = new Random();

                //Dummy method in lieu of database
                foreach (var visual in visuals)
                {
                    int tagNumber = random.Next(1, 10);
                    bool containsPhi = tagNumber % 2 == 0;
                    var rmd = new ReportMetaData
                    {
                        ReportMetaDataId = visual.Id,
                        ExternalReportId = visual.Id,
                        Title = visual.ViewName,
                        Description = "This description is from ReportVisualService loop.",
                        ContainsPhi = containsPhi,
                        Tags = new Collection<string> { "tag" + tagNumber.ToString() },
                        Parameters = null
                    };
                    MockReportMetaData.Add(rmd);

                    if (visual.Id == "9f271580-e62d-4ef6-bf7b-0b9ff42d1c8a")
                    {
                        var rmd2 = new ReportMetaData
                        {
                            ParentReportMetaDataId = "Copy",
                            ExternalReportId = visual.Id,
                            Title = "Copy of" + visual.ViewName,
                            Description = "My custom description.",
                            ContainsPhi = containsPhi,
                            Tags = new Collection<string> { "tag" + tagNumber.ToString(), "Copy" },
                            Parameters = null,
                            OwnerUserId = CurrentUser.User.Id
                        };
                        MockReportMetaData.Add(rmd2);

                        var rmd3 = new ReportMetaData
                        {
                            ParentReportMetaDataId = "Shared",
                            ExternalReportId = visual.Id,
                            Title = "Shared version of" + visual.ViewName,
                            Description = "Shared by Aaron C Scott",
                            ContainsPhi = containsPhi,
                            Tags = new Collection<string> { "tag" + tagNumber.ToString(), "Copy", "Shared" },
                            Parameters = null,
                            OwnerUserId = "SomeoneElse",
                            Shared = true
                        };
                        MockReportMetaData.Add(rmd3);
                    }
                }

                return visuals;
            }
            else
            {
                return new List<ITableauReportVisual>();
            }
        }

        private IEnumerable<IReportMetaData> GetRelevantReportMetaDatas()
        {
            var relevantReportMetaDatas =
                MockReportMetaData.Where(
                    x => x.ContainsPhi == CanAccessPhi && 
                    (x.ParentReportMetaDataId == null //Traffk supplied metadata
                    || (x.OwnerUserId == CurrentUser.User.Id) //Users metadata
                    || (x.OwnerUserId != CurrentUser.User.Id && x.Shared))); //Shared metadata
            return relevantReportMetaDatas;
        }

        private IEnumerable<IReportMetaData> GetReportMetaData(string externalReportId)
        {
            return MockReportMetaData.Where(x => x.ExternalReportId == externalReportId); //missing check for delete bit
        }

        public byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId)
        {
            return TableauRestService.DownloadPreviewImageForView(workbookId, viewId);
        }
    }
}
