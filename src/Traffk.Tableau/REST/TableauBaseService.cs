using System.Collections.Generic;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Utility;

namespace Traffk.Tableau.REST
{
    public abstract class TableauBaseService
    {
        public TableauServerSignIn Login { get; set; }

        protected readonly TableauSignInOptions Options;
        protected readonly TableauServerUrls Urls;
        protected readonly ILogger Logger;
        protected readonly ICacher Cacher;
        protected readonly ITableauUserCredentials TableauUserCredentials;
        protected IHttpClientFactory HttpClientFactory;
        protected readonly bool IsSignedIn;

        protected TableauBaseService(IOptions<TableauSignInOptions> options,
            ITableauUserCredentials tableauUserCredentials,
            IHttpClientFactory httpClientFactory,
            ILogger logger,
            ICacher cacher = null)
        {
            TableauUserCredentials = tableauUserCredentials;
            Logger = logger;
            Cacher = cacher ?? Cache.Passthrough;
            Options = options.Value;
            Urls = TableauServerUrls.FromContentUrl(Options.RestApiUrl, 10);
            HttpClientFactory = httpClientFactory;

            Login = SignIn(Urls, TableauUserCredentials.UserName, TableauUserCredentials.Password, logger);

            IsSignedIn = Login.IsSignedIn;
        }

        protected TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, ILogger logger)
        {
            var signIn = Cacher.FindOrCreate(
                Cache.CreateKey(onlineUrls.CacheKey, userName, password),
                key =>
                {
                    var l = new TableauServerSignIn(onlineUrls, userName, password, HttpClientFactory, logger);
                    l.ExecuteRequest();
                    return new CacheEntry<TableauServerSignIn>(l, Options.LoginCacheTimeout);
                }).Value;
            return signIn;
        }

        protected DownloadViewsForSite DownloadViewsForSite()
        {
            var views = new DownloadViewsForSite(Urls, Login, HttpClientFactory);
            views.ExecuteRequest();
            return views;
        }

        protected ICollection<SiteWorkbook> DownloadWorkbooksList()
        {
            var workbooksList = new DownloadWorkbooksList(Urls, Login, HttpClientFactory);
            workbooksList.ExecuteRequest();
            return workbooksList.Workbooks;
        }
    }
}
