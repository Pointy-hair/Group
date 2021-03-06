﻿using System;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Utility;

namespace Traffk.Tableau.REST.Helpers
{
    /// <summary>
    /// Helper class for managing access to project Ids and creating server site projects on demand
    /// </summary>
    public class ProjectFindCreateHelper : TableauServerSignedInRequestBase
    {
        private readonly DownloadProjectsList ProjectsList;
        //    private readonly 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public ProjectFindCreateHelper(
            TableauServerUrls onlineUrls, 
            TableauServerSignIn login, 
            IHttpClientFactory httpClientFactory)
            : base(onlineUrls, login, httpClientFactory)
        {
            //Ask server for the list of projects
            var projectsList = new DownloadProjectsList(Urls, Login, httpClientFactory);
            projectsList.ExecuteRequest();
            ProjectsList = projectsList;
        }


        /// <summary>
        /// Looks up the default project ID
        /// </summary>
        /// <returns></returns>
        public string GetProjectIdForUploads(string projectName)
        {
            ////If the project name is empty - look for the default project
            //if (string.IsNullOrEmpty(projectName))
            //{
            //    goto find_default_project;
            //}

            ////Look for the matching project
            //var project = _projectsList.FindProjectWithName(projectName);
            //if (project != null)
            //{
            //    return project.Id;
            //}

            ////If the option is specified; attempt to create the project
            //if (_uploadProjectBehavior.AttemptProjectCreate)
            //{
            //    //Create the project name
            //    var createProject = new SendCreateProject(_onlineUrls, _onlineSession, projectName);
            //    try
            //    {
            //        var newProject = createProject.ExecuteRequest();
            //        _projectsList.AddProject(newProject);
            //        return newProject.Id;
            //    }
            //    catch (Exception exCreateProject)
            //    {
            //        
            //    }
            //}

            ////If we are not allowed to fall back the default project then error
            //if (!_uploadProjectBehavior.UseDefaultProjectIfNeeded)
            //{
            //    throw new Exception("Not allowed to use default project");
            //}


            //find_default_project:
            //If all else fails, fall back to using the default project
            var defaultProject = ProjectsList.FindProjectWithName("default"); //Find the default project
            if (defaultProject != null) return defaultProject.Id;

            defaultProject = ProjectsList.FindProjectWithName("Default"); 
            if (defaultProject != null) return defaultProject.Id;

            defaultProject = ProjectsList.FindProjectWithName(""); //Try empty
            if (defaultProject != null) return defaultProject.Id;

            //Default project not found. Choosing any project
            Login.Logger.Error("Default project not found. Reverting to any project");
            foreach (var thisProj in ProjectsList.Projects)
            {
                return thisProj.Id;
            }

            Login.Logger.Error("Upload could not find a project ID to use");
            throw new Exception("Aborting. Upload Datasources could not find a project ID to use");
        }



    }
}
