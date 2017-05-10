using System;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public class TableauViewerService : TableauBaseService, ITableauViewerService
    {
        TimeSpan ITableauViewerService.ReportIndexCacheTimeout => Options.ReportIndexCacheTimeout;
        
        #region Constructors

        public TableauViewerService(
            IOptions<TableauSignInOptions> options,
            ITableauUserCredentials tableauUserCredentials,
            ICacher cacher=null) : base(options, tableauUserCredentials, cacher)
        {

        }

        #endregion

        DownloadViewsForSite ITableauViewerService.DownloadViewsForSite() => base.DownloadViewsForSite();

        byte[] ITableauViewerService.DownloadPreviewImageForView(string workbookId, string viewId)
        {
            var downloadPreviewImage = new DownloadPreviewImageForView(Urls, Login);
            downloadPreviewImage.ExecuteRequest(workbookId, viewId);
            return downloadPreviewImage.PreviewImage;
        }
    }
}
