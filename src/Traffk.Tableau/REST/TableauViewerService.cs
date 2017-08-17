using System;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Utility;

namespace Traffk.Tableau.REST
{
    /// <summary>
    /// TableauViewerService is server implementation of Tableau Javascript API
    /// </summary>
    public class TableauViewerService : TableauBaseService, ITableauViewerService
    {
        TimeSpan ITableauViewerService.ReportIndexCacheTimeout => Options.ReportIndexCacheTimeout;
        
        public TableauViewerService(
            IOptions<TableauSignInOptions> options,
            ITableauUserCredentials tableauUserCredentials,
            IHttpClientFactory httpClientFactory,
            ILogger logger,
            ICacher cacher=null) : base(options, tableauUserCredentials, httpClientFactory, logger, cacher)
        {

        }

        DownloadViewsForSite ITableauViewerService.DownloadViewsForSite() => base.DownloadViewsForSite();

        byte[] ITableauViewerService.DownloadPreviewImageForView(string workbookId, string viewId)
        {
            var downloadPreviewImage = new DownloadPreviewImageForView(Urls, Login, HttpClientFactory);
            downloadPreviewImage.ExecuteRequest(workbookId, viewId);
            return downloadPreviewImage.PreviewImage;
        }
    }
}
