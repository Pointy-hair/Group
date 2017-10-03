using System;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Traffk.Bal.Permissions;
using Traffk.Bal.Services;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Utility;

namespace Traffk.Bal.ReportVisuals
{
    public static class ReportTagFilters
    {
        public static string RiskIndex = "Risk Index";
    }

    public interface IReportVisualService
    {
        ReportTreeNode<ReportVisualFolder> GetReportFolderTreeRoot(ReportSearchCriteria reportSearchCriteria);
        ReportVisual GetReportVisual(ReportSearchCriteria reportSearchCriteria);
        ReportTreeNode<ReportVisualFolder> GetReportFolder(ReportSearchCriteria reportSearchCriteria,
            string topLevelFolderName);
        byte[] DownloadPreviewImageForTableauVisual(string workbookId, string viewId);
        bool IsOnline { get; set; }
    }

    public class ReportVisualService : IReportVisualService
    {
        private const string Root = "Root";

        private ITableauViewerService TableauViewerService;
        private TraffkTenantModelDbContext Rdb;
        private ITableauTenantFinder TableauTenantFinder;
        private ApplicationUser CurrentUser;
        private bool CanSeePhi;
        private IEnumerable<string> TableauReportIds;
        private ICacher Cacher;
        private TimeSpan ReportIndexCacheTimeout;

        public string TableauTenantId { get; private set; }
        bool IReportVisualService.IsOnline { get; set; }

        public ReportVisualService(ITableauViewerService tableauViewerService, 
            TraffkTenantModelDbContext rdb, 
            ITableauTenantFinder tableauTenantFinder, 
            ICurrentUser currentUser, 
            IPhiAuthorizer phiAuthorizer,
            ICacher cacher = null)
        {
            CanSeePhi = phiAuthorizer.CanSeePhi;
            TableauViewerService = tableauViewerService;
            Rdb = rdb;
            TableauTenantFinder = tableauTenantFinder;
            CurrentUser = currentUser.User;

            Cacher = cacher.CreateScope(CurrentUser.UserName, CanSeePhi);

            TableauTenantId = TableauTenantFinder.GetTenantIdAsync().Result;

            ReportIndexCacheTimeout = TableauViewerService.ReportIndexCacheTimeout;

            ((IReportVisualService) this).IsOnline = TableauViewerService.IsOnline;
        }

        ReportVisual IReportVisualService.GetReportVisual(ReportSearchCriteria reportSearchCriteria)
        {
            var iReportVisualService = this as IReportVisualService;
            var root = iReportVisualService.GetReportFolderTreeRoot(reportSearchCriteria);
            ReportVisual matchingReportVisual = null;
            var id = reportSearchCriteria.ReportId;
            root.Walk((node, depth) =>
            {
                var currentFolderReports = node.Data.Reports;
                if (!currentFolderReports.Any()) return;
                foreach (var report in currentFolderReports)
                {
                    if (id == report.Id ||
                        id == report.ParentId)
                    {
                        matchingReportVisual = report;
                        return;
                    }
                }
            });

            return matchingReportVisual;
        }

        ReportTreeNode<ReportVisualFolder> IReportVisualService.GetReportFolderTreeRoot(ReportSearchCriteria reportSearchCriteria)
        {
            return GetReportFolder(reportSearchCriteria, Root);
        }

        ReportTreeNode<ReportVisualFolder> IReportVisualService.GetReportFolder(
            ReportSearchCriteria reportSearchCriteria,
            string topLevelFolderName) => GetReportFolder(reportSearchCriteria, topLevelFolderName);

        private ReportTreeNode<ReportVisualFolder> GetReportFolder(
            ReportSearchCriteria reportSearchCriteria, string topLevelFolderName)
        {
            var visualContext = reportSearchCriteria.VisualContext;
            var reportTagFilter = reportSearchCriteria.TagFilter;

            var rootKey = visualContext.ToString() + topLevelFolderName + reportSearchCriteria.BaseReportVisual?.Id;

            if (reportTagFilter != null)
            {
                foreach (var tag in reportTagFilter)
                {
                    rootKey += tag;
                }
            }

            return Cacher.FindOrCreate(rootKey, key => GetTreeCacheEntry(reportSearchCriteria, topLevelFolderName), ReportIndexCacheTimeout).Value;
        }

        byte[] IReportVisualService.DownloadPreviewImageForTableauVisual(string workbookId, string viewId)
        {
            return TableauViewerService.DownloadPreviewImageForView(workbookId, viewId);
        }

        #region Private Methods
        private IEnumerable<IReportVisual> GetReportVisuals(ReportSearchCriteria searchCriteria)
        {
            var tableauReports = GetTableauReportVisuals();
            var tableauReportVisuals = tableauReports as IList<ITableauReportVisual> ?? tableauReports.ToList();
            TableauReportIds = tableauReportVisuals.Select(t => t.Id);

            var relevantReportMetaDatas = new HashSet<ReportMetaData>(GetRelevantReportMetaDatas(searchCriteria));

            List<ReportVisual> visuals = new List<ReportVisual>();
            foreach (var rmd in relevantReportMetaDatas)
            {
                var tableauReport = tableauReportVisuals.SingleOrDefault(x => x.Id == rmd.ExternalReportKey);
                var visual = (ReportVisual)Merge(tableauReport, rmd);
                visuals.Add(visual);
            }

            return visuals;
        }

        private ReportTreeNode<ReportVisualFolder> GetTreeCacheEntry(ReportSearchCriteria reportSearchCriteria, string topLevelFolderName)
        {
            var root = new ReportTreeNode<ReportVisualFolder>(new ReportVisualFolder(topLevelFolderName));
            var reportVisuals = GetReportVisuals(reportSearchCriteria) as ICollection<IReportVisual> ?? GetReportVisuals(reportSearchCriteria).ToList();

            if (reportVisuals.Any())
            {
                var workbookFolders = new HashSet<ReportTreeNode<ReportVisualFolder>>();
                foreach (var rv in reportVisuals)
                {
                    var visual = rv as ReportVisual;
                    var folderName = rv.FolderName;

                    var existingFolder = workbookFolders.SingleOrDefault(x => x.Data.Title == folderName);

                    if (existingFolder != null)
                    {
                        existingFolder.Data.Reports.Add(visual);
                    }
                    else if (folderName == "")
                    {
                        root.Data.Reports.Add(visual);
                    }
                    else
                    {
                        var newWorkbookFolder = new ReportTreeNode<ReportVisualFolder>(new ReportVisualFolder(folderName));
                        newWorkbookFolder.Data.Reports.Add(visual);
                        workbookFolders.Add(newWorkbookFolder);
                    }
                }

                foreach (var folder in workbookFolders)
                {
                    root.Add(folder);
                }
            }
            return new CacheEntry<ReportTreeNode<ReportVisualFolder>>(root).Value;
        }

        private static IReportVisual Merge(ITableauReportVisual tableauReportVisual, ReportMetaData reportMetaData)
        {
            Requires.NonNull(reportMetaData.ReportDetails.Description, nameof(reportMetaData.ReportDetails.Description));

            const string siteIdKey = "SiteId";

            var metadataWorkbookName = reportMetaData.ReportDetails.Parameters?.SingleOrDefault(x => x.Key == nameof(tableauReportVisual.WorkbookName)).Value ?? tableauReportVisual.WorkbookName;
            var metadataWorkbookId =
                reportMetaData.ReportDetails.Parameters?.SingleOrDefault(
                    x => x.Key == nameof(tableauReportVisual.WorkbookId)).Value ?? tableauReportVisual.WorkbookId;
            var metadataViewId =
                reportMetaData.ReportDetails.Parameters?.SingleOrDefault(x => x.Key == nameof(tableauReportVisual.Id))
                    .Value ?? tableauReportVisual.Id;
            var metadataViewName =
                reportMetaData.ReportDetails.Parameters?.SingleOrDefault(
                    x => x.Key == nameof(tableauReportVisual.ViewName)).Value ?? tableauReportVisual.ViewName;
            var metadataWorksheetName =
                reportMetaData.ReportDetails.Parameters?.SingleOrDefault(
                    x => x.Key == nameof(tableauReportVisual.WorksheetName)).Value;


            var workbookName = new KeyValuePair<string, string>(nameof(tableauReportVisual.WorkbookName), metadataWorkbookName);
            var workbookId = new KeyValuePair<string, string>(nameof(tableauReportVisual.WorkbookId), metadataWorkbookId);
            var viewId = new KeyValuePair<string, string>(nameof(tableauReportVisual.Id), metadataViewId);
            var viewName = new KeyValuePair<string, string>(nameof(tableauReportVisual.ViewName), metadataViewName);
            var worksheetName = new KeyValuePair<string, string>(nameof(tableauReportVisual.WorksheetName), metadataWorksheetName);
            var siteId = new KeyValuePair<string, string>(siteIdKey, reportMetaData.TenantId.ToString());
            var parameters = new Collection<KeyValuePair<string, string>>
            {
                workbookName,
                workbookId,
                viewId,
                viewName,
                siteId,
                worksheetName
            };

            var visual = new ReportVisual();

            ((IReportVisual)visual).CanExport = reportMetaData.ReportDetails.CanExport;
            ((IReportVisual)visual).ContainsPhi = reportMetaData.ReportDetails.ContainsPhi;
            ((IReportVisual)visual).Description = reportMetaData.ReportDetails.Description ?? "No description available";
            ((IReportVisual)visual).ExternalReportKey = tableauReportVisual.Id;
            ((IReportVisual)visual).Favorite = reportMetaData.ReportDetails.Favorite;
            ((IReportVisual)visual).FolderPath = reportMetaData.ReportDetails.FolderPath ?? "/";
            ((IReportVisual)visual).Id = reportMetaData.ReportMetaDataId;
            ((IReportVisual)visual).LastEdit = reportMetaData.ReportDetails.LastEdit;
            ((IReportVisual)visual).LastEditedField = reportMetaData.ReportDetails.LastEditedField;
            ((IReportVisual)visual).OwnerContactId = reportMetaData.OwnerContactId;
            ((IReportVisual)visual).Parameters = parameters;
            ((IReportVisual)visual).ParentId = reportMetaData.ParentReportMetaDataId;
            ((IReportVisual)visual).PreviewImageUrl = reportMetaData.ReportDetails.PreviewImageUrl ??
                                                      $"/Reporting/PreviewImage/{tableauReportVisual.WorkbookId}/{tableauReportVisual.Id}";
            ((IReportVisual)visual).Shared = reportMetaData.ReportDetails.Shared;
            ((IReportVisual)visual).Tags = reportMetaData.ReportDetails.Tags;
            ((IReportVisual)visual).Title = reportMetaData.ReportDetails.Title ?? tableauReportVisual.ViewName;
            ((IReportVisual)visual).VisualContext = reportMetaData.ReportDetails.VisualContext;
            ((IReportVisual)visual).RenderingAttributes = reportMetaData.ReportDetails.RenderingAttributes;

            return visual;
        }

        private static ReportMetaData Merge(ReportMetaData primary, ReportMetaData secondary)
        {
            var mergedReportDetails = new ReportDetails
            {
                Title = primary.ReportDetails.Title ?? secondary.ReportDetails.Title,
                Description = primary.ReportDetails.Description ?? secondary.ReportDetails.Description,
                ContainsPhi = primary.ReportDetails.ContainsPhi,
                Tags = primary.ReportDetails.Tags ?? secondary.ReportDetails.Tags,
                Parameters = primary.ReportDetails.Parameters ?? secondary.ReportDetails.Parameters,
                VisualContext = primary.ReportDetails.VisualContext,
                FolderPath = primary.ReportDetails.FolderPath,
                CanExport = primary.ReportDetails.CanExport,
                RenderingAttributes = primary.ReportDetails.RenderingAttributes ?? secondary.ReportDetails.RenderingAttributes
            };

            var mergedRmd = new ReportMetaData
            {
                ParentReportMetaDataId = primary.ParentReportMetaDataId ?? secondary.ParentReportMetaDataId,
                ExternalReportKey = primary.ExternalReportKey ?? secondary.ExternalReportKey,
                ReportDetails = mergedReportDetails,
                OwnerContactId = primary.OwnerContactId ?? secondary.OwnerContactId,
                TenantId = primary.TenantId
            };

            return mergedRmd;
        }

        private IEnumerable<ITableauReportVisual> GetTableauReportVisuals()
        {
            if (TableauTenantId == null)
            {
                var tableauReportVisuals = TableauViewerService.DownloadViewsForSite().Views.OrderBy(x => x.ViewName).ToList();

                return tableauReportVisuals;
            }
            else
            {
                return new List<ITableauReportVisual>();
            }
        }

        private IEnumerable<ReportMetaData> GetRelevantReportMetaDatas(ReportSearchCriteria reportSearchCriteria)
        {
            var relevantReportMetaDatas =
                Rdb.ReportMetaData.Where(
                    x =>
                        (!x.ReportDetails.ContainsPhi || this.CanSeePhi)
                        &&
                        TableauReportIds.Contains(x.ExternalReportKey)
                        &&
                        x.ReportDetails.VisualContext == reportSearchCriteria.VisualContext
                        &&
                        (reportSearchCriteria.TagFilter == null || x.ReportDetails.Tags.Any(tag => reportSearchCriteria.TagFilter.Contains(tag)))
                        &&
                        (reportSearchCriteria.BaseReportVisual == null || x.ReportMetaDataId != reportSearchCriteria.BaseReportVisual.Id) //when fetching related reports, don't include the current report
                        &&
                        ((x.ParentReportMetaDataId == null //Traffk supplied metadata
                          || ((long)x.OwnerContactId == CurrentUser.ContactId) //User's metadata
                          || (x.OwnerContactId != CurrentUser.ContactId && x.ReportDetails.Shared))) //Shared metadata
                );

            return relevantReportMetaDatas;
        }

        private IEnumerable<ReportMetaData> GetReportMetaData(string externalReportKey)
        {
            return Rdb.ReportMetaData.Where(x => x.ExternalReportKey == externalReportKey);
        }
        #endregion

        #region Static Methods
        public static string CreateAnchorName(ReportResource resource) => CreateAnchorName(resource.Title);
        public static string CreateAnchorName(string name) => name.Trim()?.ToLower()?.RemoveSpecialCharacters() ?? "";
        #endregion

    }
}
