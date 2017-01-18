using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Tableau.REST
{
    public class TableauRestService
    {
        //public TableauServerUrls Urls;
        public TableauRestService()
        {
            
        }

        public TableauRestService(string url, string userName, string password)
        {
            
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
                statusLog.AddError("Failed loging, " + exLogin.ToString());
            }

            return serverLogin;
        }

        public DownloadProjectsList DownloadProjectsList(TableauServerSignIn onlineLogin, TableauServerUrls onlineUrls )
        {
            //_statusLog.AddStatusHeader("Request site projects");
            DownloadProjectsList projects = null;
            //===================================================================================
            //Projects...
            //===================================================================================
            try
            {
                projects = new DownloadProjectsList(onlineUrls, onlineLogin);
                projects.ExecuteRequest();

                //List all the projects
                //foreach (var singleProject in projects.Projects)
                //{
                //    //_statusLog.AddStatus(singleProject.ToString());
                //}
            }
            catch (Exception ex)
            {
                //_statusLog.AddError("Error during projects query, " + ex.ToString());
            }

            //Store it
            //this.projects = projects.Projects;
            return projects;
        }
    }
}
