using System.Collections.Generic;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public abstract class TableauBaseService
    {
        public TableauServerSignIn Login { get; set; }

        protected readonly TableauSignInOptions Options;
        protected readonly TableauServerUrls Urls;
        protected readonly ICacher Cacher;
        protected readonly ITableauUserCredentials TableauUserCredentials;

        protected TableauBaseService(IOptions<TableauSignInOptions> options,
            ITableauUserCredentials tableauUserCredentials,
            ICacher cacher = null)
        {
            TableauUserCredentials = tableauUserCredentials;
            Cacher = cacher ?? Cache.Passthrough;
            Options = options.Value;
            Urls = TableauServerUrls.FromContentUrl(Options.RestApiUrl, 10);
            Login = SignIn(Urls, TableauUserCredentials.UserName, TableauUserCredentials.Password);
        }

        protected TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null)
        {
            return Cacher.FindOrCreate(
                Cache.CreateKey(onlineUrls.CacheKey, userName, password),
                key =>
                {
                    var l = new TableauServerSignIn(onlineUrls, userName, password, statusLog);
                    l.ExecuteRequest();
                    return new CacheEntry<TableauServerSignIn>(l, Options.LoginCacheTimeout);
                }).Value;
        }

        protected DownloadViewsForSite DownloadViewsForSite()
        {
            var views = new DownloadViewsForSite(Urls, Login);
            views.ExecuteRequest();
            return views;
        }

        protected ICollection<SiteWorkbook> DownloadWorkbooksList()
        {
            var workbooksList = new DownloadWorkbooksList(Urls, Login);
            workbooksList.ExecuteRequest();
            return workbooksList.Workbooks;
        }
    }
}
