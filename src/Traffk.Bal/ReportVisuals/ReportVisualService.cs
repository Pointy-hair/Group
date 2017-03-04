using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.Models;

namespace Traffk.Bal.ReportVisuals
{
    public class ReportVisualService
    {
        public ITableauRestService TableauRestService { get; set; }
        public TraffkRdbContext Rdb { get; set; }
        public ICollection<IReportMetaData> MockReportMetaData { get; set; }

        public ReportVisualService(ITableauRestService tableauRestService, TraffkRdbContext db)
        {
            TableauRestService = tableauRestService;
            Rdb = db;
        }

        public IEnumerable<IReportVisual> GetReportVisuals(string userId, string tenantId = null)
        {
            var tableauReports = GetTableauReportVisuals(tenantId);
            var tableauReportVisuals = tableauReports as IList<ITableauReportVisual> ?? tableauReports.ToList();
            var tableauReportIds = tableauReportVisuals.Select(t => t.Id);

            var relevantReportMetaDatas = GetRelevantReportMetaDatas(userId).Where(x => tableauReportIds.Contains(x.ExternalReportId));
            
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
            var visual = new ReportVisual()
            {
                CanExport = reportMetaData.CanExport,
                ContainsPhi = reportMetaData.ContainsPhi
                //etc.
            };

            return visual;
        }

        private IEnumerable<ITableauReportVisual> GetTableauReportVisuals(string tenantId = null)
        {
            if (tenantId == null)
            {
                var visuals = TableauRestService.DownloadViewsForSite().Views;
                return visuals;
            }
            else
            {
                return new List<ITableauReportVisual>();
            }
        }

        private IEnumerable<IReportMetaData> GetRelevantReportMetaDatas(string userId)
        {
            var relevantReportMetaDatas =
                MockReportMetaData.Where(
                    x => x.ParentReportMetaDataId == null //Traffk supplied metadata
                    || (x.OwnerUserId == userId) //Users metadata
                    || (x.OwnerUserId != userId && x.Shared)); //Shared metadata
            return relevantReportMetaDatas;
        }

        private IEnumerable<IReportMetaData> GetReportMetaData(string externalReportId)
        {
            return MockReportMetaData.Where(x => x.ExternalReportId == externalReportId); //missing check for delete bit
        }
    }
}
