using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Traffk.Tableau.REST.RestRequests;
using RevolutionaryStuff.Core.Caching;

namespace Traffk.Tableau.REST
{
    public class TableauRestService : ITableauRestService
    {
        public TableauServerSignIn Login { get; set; }

        private readonly TableauSignInOptions Options;
        private readonly TableauServerUrls Urls;
        private readonly ICacher Cacher;

        #region Constructors

        private TableauRestService(ICacher cacher)
        {
            Cacher = cacher ?? Cache.Passthrough;
        }

        public TableauRestService()
            : this((ICacher)null)
        { }

        public TableauRestService(IOptions<TableauSignInOptions> options, ICacher cacher=null)
            : this(cacher)
        {
            Options = options.Value;
            Urls = TableauServerUrls.FromContentUrl(Options.RestApiUrl, 10);
            Login = SignIn(Urls, Options.Username, Options.Password);
        }

        public TableauRestService(string url, string userName, string password)
            : this ()
        {
            var urls = TableauServerUrls.FromContentUrl(url, 10);
            Login = SignIn(urls, userName, password);
        }

        #endregion

        public TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null)
        {
            return Cacher.FindOrCreate(
                Cache.CreateKey(onlineUrls.CacheKey, userName, password),
                key =>
                {
                    var l = new TableauServerSignIn(onlineUrls, userName, password, statusLog);
                    l.ExecuteRequest();
                    return new CacheEntry<TableauServerSignIn>(l);
                }).Value;
        }

        public DownloadProjectsList DownloadProjectsList(TableauServerUrls onlineUrls, TableauServerSignIn onlineLogin = null)
        {
            if (onlineLogin == null)
            {
                onlineLogin = Login;
            }

            //_statusLog.AddStatusHeader("Request site projects");
            DownloadProjectsList projects = null;
            //===================================================================================
            //Projects...
            //===================================================================================
            try
            {
                projects = new DownloadProjectsList(onlineUrls, onlineLogin);
                projects.ExecuteRequest();
            }
            catch (Exception ex)
            {
                //_statusLog.AddError("Error during projects query, " + ex.ToString());
            }

            return projects;
        }

        public DownloadViewsForSite DownloadViewsForSite()
        {
            try
            {
                return DownloadViewsForSite(Urls, Login);
            }
            catch (Exception e)
            {
                //TODO: Serilog
                throw;
            }
        }

        public DownloadViewsForSite DownloadViewsForSite(TableauServerUrls onlineUrls, TableauServerSignIn onlineLogin = null)
        {
            try
            {
                if (onlineLogin == null)
                {
                    onlineLogin = Login;
                }

                var views = new DownloadViewsForSite(onlineUrls, onlineLogin);
                views.ExecuteRequest();
                return views;
            }
            catch (Exception e)
            {
                //TODO: Serilog
                throw;
            }
        }

        public DownloadWorkbooksList DownloadWorkbooksList()
        {
            try
            {
                return DownloadWorkbooksList(Urls, Login);

            }
            catch (Exception e)
            {
                //TODO: Serilog
                throw;
            }
        }

        public DownloadWorkbooksList DownloadWorkbooksList(TableauServerUrls onlineUrls,
            TableauServerSignIn onlineLogin = null)
        {
            try
            {
                //Get the list of workbooks
                var workbooksList = new DownloadWorkbooksList(onlineUrls, onlineLogin);
                workbooksList.ExecuteRequest();

                return workbooksList;
            }
            catch (Exception exDownload)
            {
                //TODO: Serilog
                throw;
                //_statusLog.AddError("Error during workbooks list download, " + exDownload.ToString());
            }
        }

        public byte[] DownloadPreviewImageForView(string workbookId, string viewId)
        {
            try
            {
                return DownloadPreviewImageForView(Urls, workbookId, viewId, Login);
            }
            catch (Exception e)
            {
                //TODO: Serilog
                throw;
            }
        }

        public byte[] DownloadPreviewImageForView(TableauServerUrls onlineUrls, string workbookId, string viewId, TableauServerSignIn onlineLogin = null)
        {
            try
            {
                var downloadPreviewImage = new DownloadPreviewImageForView(onlineUrls, onlineLogin);
                downloadPreviewImage.ExecuteRequest(workbookId, viewId);
                return downloadPreviewImage.PreviewImage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void GetUnderlyingData(TableauSignInOptions options, string workbookName, string viewName, TableauServerSignIn onlineLogin = null)
        {
            if (onlineLogin == null)
            {
                onlineLogin = Login;
            }

            var request = new GetUnderlyingData(options, onlineLogin);
            request.ExecuteRequest(workbookName, viewName);
        }
    }
}
