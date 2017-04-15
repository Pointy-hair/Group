﻿using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System.Collections.Generic;
using System.IO;
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public class TableauRestService : ITableauRestService
    {
        public TableauServerSignIn Login { get; set; }

        protected readonly TableauSignInOptions Options;
        private readonly TableauServerUrls Urls;
        private readonly ICacher Cacher;
        private readonly ITableauUserCredentials TableauUserCredentials;

        #region Constructors

        public TableauRestService(IOptions<TableauSignInOptions> options, 
            ITableauUserCredentials tableauUserCredentials,
            ICacher cacher=null)
        {
            TableauUserCredentials = tableauUserCredentials;
            Cacher = cacher ?? Cache.Passthrough;
            Options = options.Value;
            Urls = TableauServerUrls.FromContentUrl(Options.RestApiUrl, 10);
            Login = SignIn(Urls, TableauUserCredentials.UserName, TableauUserCredentials.Password);
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

        public DownloadProjectsList DownloadProjectsList()
        {
            var projects = new DownloadProjectsList(this.Urls, Login);
            projects.ExecuteRequest();
            return projects;
        }

        DownloadViewsForSite ITableauRestService.DownloadViewsForSite()
        {
            var views = new DownloadViewsForSite(Urls, Login);
            views.ExecuteRequest();
            return views;
        }

        ICollection<SiteWorkbook> ITableauRestService.DownloadWorkbooksList()
        {
            var workbooksList = new DownloadWorkbooksList(Urls, Login);
            workbooksList.ExecuteRequest();
            return workbooksList.Workbooks;
        }

        SiteInfo ITableauRestService.CreateSite(string tenantName)
        {
            var addSite = new CreateSite(Urls, Login);
            var siteInfo = addSite.ExecuteRequest(tenantName);
            return siteInfo;
        }

        private string GetNewSiteUrl(SiteInfo site)
        {
            return Options.BaseUrl + @"/site/" + site.ContentUrl;
        }

        public SiteInfo GetSiteInfo()
        {
            var getSiteInfoRequest = new DownloadSiteInfo(Urls, Login);
            return getSiteInfoRequest.ExecuteRequest();
        }

        void ITableauRestService.AddUserToSite(string siteId, string userName)
        {
            var addUserToSite = new AddUserToSite(Urls, Login);
            addUserToSite.ExecuteRequest(siteId, userName);
        }

        void ITableauRestService.RemoveUserFromSite(SiteUser userToRemove)
        {
            var removeUserRequest = new RemoveUserFromSite(Urls, Login);
            removeUserRequest.ExecuteRequest(userToRemove);
        }

        ICollection<SiteWorkbook> ITableauRestService.DownloadWorkbooks(IEnumerable<SiteWorkbook> workbooksToDownload,
            string localSavePath,
            bool generateInfoFile)
        {
            var downloadWorkbooksRequest = new DownloadWorkbooks(Urls, Login, workbooksToDownload, localSavePath, generateInfoFile);
            var downloadedWorkbooks = downloadWorkbooksRequest.ExecuteRequest();
            return downloadedWorkbooks;
        }

        void ITableauRestService.UploadWorkbooks(string projectName, string datasourceUsername, string datasourcePassword, bool isEmbedded, string path)
        {
            var credentialManager = new CredentialManager();
            foreach (var thisFilePath in Directory.GetFiles(path))
            {
                credentialManager.AddWorkbookCredential(Path.GetFileName(thisFilePath), projectName, datasourceUsername, datasourcePassword, isEmbedded);
            }
            var uploadWorkbooksRequest = new UploadWorkbooks(Urls, Login, credentialManager, path);
            uploadWorkbooksRequest.ExecuteRequest();
        }

        ICollection<SiteDatasource> ITableauRestService.DownloadDatasourceList()
        {
            var downloadDataSourceListRequest = new DownloadDatasourcesList(Urls, Login);
            downloadDataSourceListRequest.ExecuteRequest();
            return downloadDataSourceListRequest.Datasources;
        }

        ICollection<SiteConnection> ITableauRestService.DownloadConnectionsForDatasource(string datasourceId)
        {
            var downloadConnectionRequest = new DownloadDatasourceConnections(Urls, Login, datasourceId);
            downloadConnectionRequest.ExecuteRequest();
            return downloadConnectionRequest.Connections;
        }

        void ITableauRestService.DownloadDatasourceFiles(IEnumerable<SiteDatasource> datasources, string savePath)
        {
            var projectRequest = new DownloadProjectsList(Urls, Login);
            projectRequest.ExecuteRequest();
            var downloadDatasources = new DownloadDatasources(Urls, Login, datasources, savePath, projectRequest, false, new KeyedLookup<SiteUser>(new List<SiteUser>()) );
           downloadDatasources.ExecuteRequest();
        }

        ICollection<SiteDatasource> ITableauRestService.UploadDatasourceFiles(string projectName, string datasourceUsername, string datasourcePassword, bool isEmbedded, string path)
        {
            var credentialManager = new CredentialManager();
            foreach (var thisFilePath in Directory.GetFiles(path))
            {
                credentialManager.AddDatasourceCredential(Path.GetFileName(thisFilePath), projectName, datasourceUsername, datasourcePassword, isEmbedded);
            }

            var uploadRequest = new UploadDatasources(Urls, Login, credentialManager, path, false, new List<SiteUser>());
            uploadRequest.ExecuteRequest();
            return uploadRequest.UploadeDatasources;
        }

        byte[] ITableauRestService.DownloadPreviewImageForView(string workbookId, string viewId)
        {
            var downloadPreviewImage = new DownloadPreviewImageForView(Urls, Login);
            downloadPreviewImage.ExecuteRequest(workbookId, viewId);
            return downloadPreviewImage.PreviewImage;
        }

        void ITableauRestService.UpdateDatasourceConnection(SiteDatasource datasourceToUpdate, SiteConnection connectionToUpdate, string newServerAddress)
        {
            var updateDatasourceRequest = new UpdateDatasourceConnection(Urls, Login);
            updateDatasourceRequest.UpdateServerAddress(datasourceToUpdate, connectionToUpdate, newServerAddress);
        }
        
    }
}
