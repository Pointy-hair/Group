using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using System.Collections.Generic;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public class TableauUserService : TableauBaseService, ITableauUserService
    {

        #region Constructors

        public TableauUserService(IOptions<TableauSignInOptions> options, 
            ITableauUserCredentials tableauUserCredentials,
            ICacher cacher=null) : base(options, tableauUserCredentials, cacher)
        {

        }

        #endregion

        DownloadViewsForSite ITableauUserService.DownloadViewsForSite() => base.DownloadViewsForSite();

        byte[] ITableauUserService.DownloadPreviewImageForView(string workbookId, string viewId)
        {
            var downloadPreviewImage = new DownloadPreviewImageForView(Urls, Login);
            downloadPreviewImage.ExecuteRequest(workbookId, viewId);
            return downloadPreviewImage.PreviewImage;
        }
    }
}
