using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.Core.Collections;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Permissions;
using Traffk.Bal.Services;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;

namespace Traffk.Bal.ReportVisuals
{
    public static class ReportTagFilters
    {
        public static string RiskIndex = "Risk Index";
    }

    public interface IReportVisualService
    {
        TreeNode<IReportResource> GetReportFolderTreeRoot(VisualContext visualContext, string reportTagFilter = null);
        IEnumerable<ReportVisual> GetReportVisuals(VisualContext visualContext, string reportTagFilter);
        ReportVisual GetReportVisual(VisualContext context, string id);

        byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId);
    }

    public class ReportVisualService : IReportVisualService
    {
        private ITableauRestService TableauRestService;
        private TraffkRdbContext Rdb;
        private ITableauTenantFinder TableauTenantFinder;
        private ApplicationUser CurrentUser;
        private AuthorizationHandlerContext AuthorizationHandlerContext;
        private HashSet<ReportMetaData> MockReportMetaData;
        private bool CanAccessPhi;
        private IEnumerable<string> TableauReportIds;
        private ICacher Cacher;

        //Do we make these private?
        public string TableauTenantId { get; private set; }

        public ReportVisualService(ITableauRestService tableauRestService, TraffkRdbContext rdb, 
            ITableauTenantFinder tableauTenantFinder, ICurrentUser currentUser, ICacher cacher = null)
        {
            TableauRestService = tableauRestService;
            Rdb = rdb;
            TableauTenantFinder = tableauTenantFinder;
            Cacher = cacher;

            TableauTenantId = TableauTenantFinder.GetTenantIdAsync().Result;
            CurrentUser = currentUser.User;

            CanAccessPhi = true;
            MockReportMetaData = new HashSet<ReportMetaData>();
        }

        IEnumerable<ReportVisual> IReportVisualService.GetReportVisuals(VisualContext visualContext, string reportTagFilter) 
            => GetReportVisuals(visualContext, reportTagFilter);

        TreeNode<IReportResource> IReportVisualService.GetReportFolderTreeRoot(VisualContext context, string reportTagFilter)
            => GetReportFolderTreeRoot(context, reportTagFilter);
        ReportVisual IReportVisualService.GetReportVisual(VisualContext context, string id) 
            => GetReportVisual(context, id);

        public ReportVisual GetReportVisual(VisualContext context, string id)
        {
            var root = GetReportFolderTreeRoot(context);
            ReportVisual matchingReportVisual = null;
            root.Walk((node, depth) =>
            {
                var currentReportVisual = node.Data as ReportVisual;
                if (currentReportVisual == null) return;
                var urlFriendlyReportName = CreateAnchorName(currentReportVisual);
                if (id == currentReportVisual.Id || id == currentReportVisual.ParentId)
                {
                    matchingReportVisual = currentReportVisual;
                }
            });

            return matchingReportVisual;
        }

        public TreeNode<IReportResource> GetReportFolderTreeRoot(VisualContext visualContext, string reportTagFilter = null)
        {
            var rootKey = visualContext.ToString() + reportTagFilter;
            return Cacher.FindOrCreate(rootKey, key => GetTreeCacheEntry(visualContext, reportTagFilter)).Value;
        }
        
        public IEnumerable<ReportVisual> GetReportVisuals(VisualContext visualContext, string reportTagFilter = null)
        {
            var tableauReports = GetTableauReportVisuals();
            var tableauReportVisuals = tableauReports as IList<ITableauReportVisual> ?? tableauReports.ToList();
            TableauReportIds = tableauReportVisuals.Select(t => t.Id);

            var relevantReportMetaDatas = new HashSet<IReportMetaData>(GetRelevantReportMetaDatas(visualContext, reportTagFilter));

            List<ReportVisual> visuals = new List<ReportVisual>();
            foreach (var rmd in relevantReportMetaDatas)
            {
                var tableauReport = tableauReportVisuals.SingleOrDefault(x => x.Id == rmd.ExternalReportId);
                var visual = (ReportVisual)Merge(tableauReport, rmd);
                visuals.Add(visual);
            }

            return visuals;
        }

        private TreeNode<IReportResource> GetTreeCacheEntry(VisualContext visualContext, string reportTagFilter)
        {
            var root = new TreeNode<IReportResource>(new ReportVisualFolder("Root"));
            var reportVisuals = (ICollection<ReportVisual>)(GetReportVisuals(visualContext, reportTagFilter) as ICollection<ReportVisual> ??
                                GetReportVisuals(visualContext, reportTagFilter).ToList());

            if (reportVisuals.Any())
            {
                var workbookFolders = new HashSet<TreeNode<IReportResource>>();
                foreach (var visual in reportVisuals)
                {
                    var folderName = visual.FolderName;

                    if (folderName == "")
                    {
                        root.Add(visual);
                    }

                    var existingFolder = workbookFolders.SingleOrDefault(x => x.Data.Title == folderName);
                    if (existingFolder != null)
                    {
                        existingFolder.Add(visual);
                    }
                    else
                    {
                        var newWorkbookFolder = new TreeNode<IReportResource>(new ReportVisualFolder(folderName));
                        newWorkbookFolder.Add(visual);
                        workbookFolders.Add(newWorkbookFolder);
                    }
                }

                foreach (var folder in workbookFolders)
                {
                    root.Add(folder);
                }
            }
            return new CacheEntry<TreeNode<IReportResource>>(root).Value;
        }


        private static IReportVisual Merge(ITableauReportVisual tableauReportVisual, IReportMetaData reportMetaData)
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
                var tableauReportVisuals = TableauRestService.DownloadViewsForSite().Views.ToList();
                var random = new Random();

                //Placeholder for Risk Index tags
                var riskIndexReportNames = new HashSet<string>(Comparers.CaseInsensitiveStringComparer);
                const string riskIndex = "RiskIndex";
                var reportNames = new List<string> { "tables", "age", "urbanicity", "occupations", "diseaseincidence" };
                foreach (var name in reportNames)
                {
                    riskIndexReportNames.Add(riskIndex + name);
                }

                int reportMetaDataCount = 0;

                //Dummy method in lieu of database
                foreach (var visual in tableauReportVisuals)
                {
                    int tagNumber = random.Next(1, 10);
                    var tags = new Collection<string> { "Tag " + tagNumber.ToString() };
                    bool containsPhi = tagNumber % 2 == 0;

                    var folderPath = "";

                    if (tagNumber < 4)
                    {
                        folderPath = @"\Low Folder";
                    }

                    if (tagNumber >= 4 && tagNumber < 7)
                    {
                        folderPath = @"\Medium Folder";
                    }

                    if (tagNumber >= 7)
                    {
                        folderPath = @"\High Folder";
                    }

                    if (riskIndexReportNames.Contains(CreateAnchorName(visual.WorkbookName)))
                    {
                        tags.Add("Risk Index"); 
                    }

                    var visualContext = VisualContext.Tenant;
                    if (visual.Id == "076e81f7-9dce-417b-a9c8-ee8b29041ab2")
                    {
                        visualContext = VisualContext.ContactPerson;;
                    }

                    var rmd = new ReportMetaData
                    {
                        ReportMetaDataId = reportMetaDataCount.ToString(),
                        ExternalReportId = visual.Id,
                        Title = visual.Title,
                        Description = "This description is from ReportVisualService loop.",
                        ContainsPhi = containsPhi,
                        Tags = tags,
                        Parameters = null,
                        VisualContext = visualContext,
                        FolderPath = folderPath
                    };
                    MockReportMetaData.Add(rmd);

                    if (visual.Id == "9f271580-e62d-4ef6-bf7b-0b9ff42d1c8a")
                    {
                        var rmd2 = new ReportMetaData
                        {
                            ReportMetaDataId = "Copy" + reportMetaDataCount.ToString(),
                            ParentReportMetaDataId = reportMetaDataCount.ToString(),
                            ExternalReportId = visual.Id,
                            Title = "Copy of " + visual.Title,
                            Description = "My custom description.",
                            ContainsPhi = containsPhi,
                            Tags = new Collection<string> { "Tag " + tagNumber.ToString() },
                            Parameters = null,
                            OwnerUserId = CurrentUser.Id,
                            VisualContext = visualContext,
                            FolderPath = @"\Your Reports"
                        };
                        rmd2.Tags.Add("Copy");
                        MockReportMetaData.Add(rmd2);

                        var rmd3 = new ReportMetaData
                        {
                            ReportMetaDataId = "Shared" + reportMetaDataCount.ToString(),
                            ParentReportMetaDataId = reportMetaDataCount.ToString(),
                            ExternalReportId = visual.Id,
                            Title = "Shared version of " + visual.Title,
                            Description = "Shared by Aaron C Scott",
                            ContainsPhi = containsPhi,
                            Tags = new Collection<string> { "Tag " + tagNumber.ToString() },
                            Parameters = null,
                            OwnerUserId = "SomeoneElse",
                            Shared = true,
                            VisualContext = visualContext,
                            FolderPath = @"\Shared Reports"
                        };
                        rmd3.Tags.Add("Copy");
                        rmd3.Tags.Add("Shared");
                        MockReportMetaData.Add(rmd3);
                    }

                    reportMetaDataCount++;
                }

                return tableauReportVisuals;
            }
            else
            {
                return new List<ITableauReportVisual>();
            }
        }

        private IEnumerable<IReportMetaData> GetRelevantReportMetaDatas(VisualContext visualContext, string tag = null)
        {
            var relevantReportMetaDatas =
                MockReportMetaData.Where(
                        x => TableauReportIds.Contains(x.ExternalReportId) &&
                        x.VisualContext == visualContext &&
                        (tag == null || x.Tags.Contains(tag)) &&
                            ((x.ParentReportMetaDataId == null //Traffk supplied metadata
                            || (x.OwnerUserId == CurrentUser.Id) //User's metadata
                            || (x.OwnerUserId != CurrentUser.Id && x.Shared))) //Shared metadata
                    );
            return relevantReportMetaDatas;
        }

        private IEnumerable<IReportMetaData> GetReportMetaData(string externalReportId)
        {
            return MockReportMetaData.Where(x => x.ExternalReportId == externalReportId); //missing check for delete bit
        }

        public static string CreateAnchorName(IReportResource resource) => CreateAnchorName(resource.Title);
        private static string CreateAnchorName(string name) => name.Trim()?.ToLower()?.RemoveSpecialCharacters() ?? "";

        public byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId)
        {
            return TableauRestService.DownloadPreviewImageForView(workbookId, viewId);
        }
    }
}
