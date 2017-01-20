using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public class TableauRestService : ITableauRestService
    {
        public TableauServerSignIn Login { get; set; }

        private readonly TableauSignInOptions Options;
        private readonly TableauServerUrls Urls;

        public TableauRestService()
        {
            
        }

        public TableauRestService(IOptions<TableauSignInOptions> options)
        {
            Options = options.Value;
            var urls = TableauServerUrls.FromContentUrl(Options.Url, 10);
            Login = new TableauServerSignIn(urls, Options.Username, Options.Password);
        }

        public TableauRestService(string url, string userName, string password)
        {
            var urls = TableauServerUrls.FromContentUrl(url, 10);
            Login = new TableauServerSignIn(urls, userName, password);
        }

        public TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null)
        {
            //========================================================================================
            //Log into Tableau Online
            //========================================================================================
            var serverLogin = new TableauServerSignIn(onlineUrls, userName, password, statusLog);
            try
            {
                serverLogin.ExecuteRequest();
            }
            catch (Exception exLogin)
            {
                //TODO: Serilog
            }

            return serverLogin;
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

        public DownloadViewsForSite DonwnloadViewsForSite()
        {
            try
            {
                var views = new DownloadViewsForSite(Urls, Login);
                views.ExecuteRequest();
                return views;
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
    }
}
