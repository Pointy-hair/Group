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
        IEnumerable<IReportVisual> GetReportVisuals(VisualContext visualContext, string reportTagFilter);
        IReportVisual GetReportVisual(VisualContext context, int id);

        byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId);
    }

    public class ReportVisualService : IReportVisualService
    {
        private ITableauRestService TableauRestService;
        private TraffkRdbContext Rdb;
        private ITableauTenantFinder TableauTenantFinder;
        private ApplicationUser CurrentUser;
        private AuthorizationHandlerContext AuthorizationHandlerContext;
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
        }

        IEnumerable<IReportVisual> IReportVisualService.GetReportVisuals(VisualContext visualContext, string reportTagFilter) 
            => GetReportVisuals(visualContext, reportTagFilter);

        TreeNode<IReportResource> IReportVisualService.GetReportFolderTreeRoot(VisualContext context, string reportTagFilter)
            => GetReportFolderTreeRoot(context, reportTagFilter);
        IReportVisual IReportVisualService.GetReportVisual(VisualContext context, int id) 
            => GetReportVisual(context, id);

        public IReportVisual GetReportVisual(VisualContext context, int id)
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
        
        public IEnumerable<IReportVisual> GetReportVisuals(VisualContext visualContext, string reportTagFilter = null)
        {
            var tableauReports = GetTableauReportVisuals();
            var tableauReportVisuals = tableauReports as IList<ITableauReportVisual> ?? tableauReports.ToList();
            TableauReportIds = tableauReportVisuals.Select(t => t.Id);

            var relevantReportMetaDatas = new HashSet<ReportMetaData>(GetRelevantReportMetaDatas(visualContext, reportTagFilter));

            List<ReportVisual> visuals = new List<ReportVisual>();
            foreach (var rmd in relevantReportMetaDatas)
            {
                var tableauReport = tableauReportVisuals.SingleOrDefault(x => x.Id == rmd.ExternalReportKey);
                var visual = (ReportVisual)Merge(tableauReport, rmd);
                visuals.Add(visual);
            }

            return visuals;
        }

        private TreeNode<IReportResource> GetTreeCacheEntry(VisualContext visualContext, string reportTagFilter)
        {
            var root = new TreeNode<IReportResource>(new ReportVisualFolder("Root"));
            var reportVisuals = (ICollection<IReportVisual>)(GetReportVisuals(visualContext, reportTagFilter) as ICollection<IReportVisual> ??
                                GetReportVisuals(visualContext, reportTagFilter).ToList());

            if (reportVisuals.Any())
            {
                var workbookFolders = new HashSet<TreeNode<IReportResource>>();
                foreach (var rv in reportVisuals)
                {
                    var visual = rv as ReportVisual;
                    var folderName = rv.FolderName;

                    var existingFolder = workbookFolders.SingleOrDefault(x => x.Data.Title == folderName);

                    if (existingFolder != null)
                    {
                        existingFolder.Add(visual);
                    }
                    else if (folderName == "")
                    {
                        root.Add(visual);
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


        private static IReportVisual Merge(ITableauReportVisual tableauReportVisual, ReportMetaData reportMetaData)
        {
            Requires.NonNull(reportMetaData.MetaData.Description, nameof(reportMetaData.MetaData.Description));

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
                CanExport = reportMetaData.MetaData.CanExport,
                ContainsPhi = reportMetaData.MetaData.ContainsPhi,
                Description = reportMetaData.MetaData.Description ?? "No description available",
                ExternalReportId = tableauReportVisual.Id,
                Favorite = reportMetaData.MetaData.Favorite,
                FolderPath = reportMetaData.MetaData.FolderPath ?? "/",
                Id = reportMetaData.ReportMetaDataId,
                LastEdit = reportMetaData.MetaData.LastEdit,
                LastEditedField = reportMetaData.MetaData.LastEditedField,
                OwnerContactId = reportMetaData.OwnerContactId,
                Parameters = reportMetaData.MetaData.Parameters ?? parameters,
                ParentId = reportMetaData.ParentReportMetaDataId,
                PreviewImageUrl = reportMetaData.MetaData.PreviewImageUrl ??
                                  $"/Reporting/PreviewImage/{tableauReportVisual.WorkbookId}/{tableauReportVisual.Id}",
                Shared = reportMetaData.MetaData.Shared,
                Tags = reportMetaData.MetaData.Tags,
                Title = reportMetaData.MetaData.Title ?? tableauReportVisual.ViewName,
                VisualContext = reportMetaData.MetaData.VisualContext               
            };

            return visual;
        }

        private static ReportMetaData Merge(ReportMetaData primary, ReportMetaData secondary)
        {
            var mergedReportDetails = new ReportDetails
            {
                Title = primary.MetaData.Title ?? secondary.MetaData.Title,
                Description = primary.MetaData.Description ?? secondary.MetaData.Description,
                ContainsPhi = primary.MetaData.ContainsPhi,
                Tags = primary.MetaData.Tags ?? secondary.MetaData.Tags,
                Parameters = primary.MetaData.Parameters ?? secondary.MetaData.Parameters,
                VisualContext = primary.MetaData.VisualContext,
                FolderPath = primary.MetaData.FolderPath,
                CanExport = primary.MetaData.CanExport
            };

            var mergedRmd = new ReportMetaData
            {
                ParentReportMetaDataId = primary.ParentReportMetaDataId ?? secondary.ParentReportMetaDataId,
                ExternalReportKey = primary.ExternalReportKey ?? secondary.ExternalReportKey,
                MetaData = mergedReportDetails,
                OwnerContactId = primary.OwnerContactId ?? secondary.OwnerContactId,
                TenantId = primary.TenantId ?? secondary.TenantId
            };

            return mergedRmd;
        }

        private IEnumerable<ITableauReportVisual> GetTableauReportVisuals()
        {
            if (TableauTenantId == null)
            {
                var tableauReportVisuals = TableauRestService.DownloadViewsForSite().Views.OrderBy(x => x.ViewName).ToList();

                //Placeholder for Risk Index tags
                var riskIndexReportNames = new HashSet<string>(Comparers.CaseInsensitiveStringComparer);
                const string riskIndex = "RiskIndex";
                var reportNames = new List<string> { "tables", "age", "urbanicity", "occupations", "diseaseincidence" };
                foreach (var name in reportNames)
                {
                    riskIndexReportNames.Add(riskIndex + name);
                }

                return tableauReportVisuals;
            }
            else
            {
                return new List<ITableauReportVisual>();
            }
        }

        private IEnumerable<ReportMetaData> GetRelevantReportMetaDatas(VisualContext visualContext, string tag = null)
        {
            var relevantReportMetaDatas =
                Rdb.ReportMetaData.Where(
                        x => TableauReportIds.Contains(x.ExternalReportKey) &&
                        x.MetaData.VisualContext == visualContext &&
                        (tag == null || x.MetaData.Tags.Contains(tag)) &&
                            ((x.ParentReportMetaDataId == null //Traffk supplied metadata
                            || (x.OwnerContactId == CurrentUser.ContactId) //User's metadata
                            || (x.OwnerContactId != CurrentUser.ContactId && x.MetaData.Shared))) //Shared metadata
                    );

            return relevantReportMetaDatas;
        }

        private IEnumerable<ReportMetaData> GetReportMetaData(string externalReportKey)
        {
            return Rdb.ReportMetaData.Where(x => x.ExternalReportKey == externalReportKey); //missing check for delete bit
        }

        public static string CreateAnchorName(IReportResource resource) => CreateAnchorName(resource.Title);
        private static string CreateAnchorName(string name) => name.Trim()?.ToLower()?.RemoveSpecialCharacters() ?? "";

        public byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId)
        {
            return TableauRestService.DownloadPreviewImageForView(workbookId, viewId);
        }
    }
}
