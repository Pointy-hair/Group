using RevolutionaryStuff.Core.Caching;
using System;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    /// <summary>
    /// Creates the set of server specific URLs
    /// </summary>
    public class TableauServerUrls : ITableauServerSiteInfo
    {
        private readonly ServerVersion ServerVersion_p;
        /// <summary>
        /// What version of Server do we thing we are talking to? (URLs and APIs may differ)
        /// </summary>
        public ServerVersion ServerVersion
        {
            get
            {
                return ServerVersion_p;
            }
        }

        /// <summary>
        /// Url for API login
        /// </summary>
        public readonly string UrlLogin;

        /// <summary>
        /// Url for log out
        /// </summary>
        public readonly string UrlLogout;

        /// <summary>
        /// Template for URL to acess workbooks list
        /// </summary>
        private readonly string UrlListWorkbooksForUserTemplate;
        private readonly string UrlListWorkbookConnectionsTemplate;
        private readonly string UrlListDatasourcesTemplate;
        private readonly string UrlListProjectsTemplate;
        private readonly string UrlListGroupsTemplate;
        private readonly string UrlListUsersTemplate;
        private readonly string UrlListUsersInGroupTemplate;
        private readonly string UrlDownloadWorkbookTemplate;
        private readonly string UrlDownloadDatasourceTemplate;
        private readonly string UrlSiteInfoTemplate;
        private readonly string UrlInitiateUploadTemplate;
        private readonly string UrlAppendUploadChunkTemplate;
        private readonly string UrlFinalizeUploadDatasourceTemplate;
        private readonly string UrlFinalizeUploadWorkbookTemplate;
        private readonly string UrlCreateProjectTemplate;
        private readonly string UrlDeleteWorkbookTagTemplate;
        private readonly string UrlDeleteDatasourceTagTemplate;
        private readonly string UrlUpdateWorkbookTemplate;
        private readonly string UrlUpdateDatasourceTemplate;
        private readonly string UrlListViewsInSite;
        private readonly string UrlListWorkbooksInSite;
        private readonly string UrlDownloadPreviewImageForView_p;
        private readonly string UrlCreateSite_p;
        private readonly string UrlAddUserToSite_p;
        private readonly string UrlDownloadDatasourceConnections;
        private readonly string UrlRemoveUserFromSite_p;
        private readonly string UrlUpdateDatasourceConnection_p;

        /// <summary>
        /// Server url with protocol
        /// </summary>
        public readonly string ServerUrlWithProtocol;
        public readonly string ServerProtocol;

        /// <summary>
        /// Part of the URL that designates the site id
        /// </summary>
        public readonly string SiteUrlSegement;

        public readonly string ServerName;

        public readonly int PageSize = 1000;
        public const int UploadFileChunkSize = 8000000; //8MB

        public readonly string CacheKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverNameWithProtocol"></param>
        /// <param name="siteUrlSegment"></param>
        public TableauServerUrls(string protocol, string serverName, string siteUrlSegment, int pageSize, ServerVersion serverVersion)
        {
            //Cannonicalize the protocol
            protocol = protocol.ToLower();

            CacheKey = Cache.CreateKey(protocol, serverName, siteUrlSegment, pageSize, serverVersion);

            ServerProtocol = protocol;

            PageSize = pageSize;

            string serverNameWithProtocol = protocol + serverName;
            ServerVersion_p = serverVersion;

            SiteUrlSegement = siteUrlSegment;
            ServerName = serverName;
            ServerUrlWithProtocol                 = serverNameWithProtocol;
            UrlLogin                              = serverNameWithProtocol + "/api/2.0/auth/signin";
            UrlLogout                             = serverNameWithProtocol + "/api/2.0/auth/signout";
            UrlListWorkbooksForUserTemplate      = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/users/{{iwsUserId}}/workbooks?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlListWorkbookConnectionsTemplate   = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/connections";
            UrlListDatasourcesTemplate           = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlListProjectsTemplate              = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/projects?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlListGroupsTemplate                = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/groups?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlListUsersTemplate                 = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/users?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlListUsersInGroupTemplate          = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/groups/{{iwsGroupId}}/users?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}"; 
            UrlDownloadDatasourceTemplate        = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources/{{iwsRepositoryId}}/content";
            UrlDownloadWorkbookTemplate          = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks/{{iwsRepositoryId}}/content";
            UrlSiteInfoTemplate                  = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}";
            UrlInitiateUploadTemplate            = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/fileUploads";
            UrlAppendUploadChunkTemplate         = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/fileUploads/{{iwsUploadSession}}";
            UrlFinalizeUploadDatasourceTemplate  = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources?uploadSessionId={{iwsUploadSession}}&datasourceType={{iwsDatasourceType}}&overwrite=true";
            UrlFinalizeUploadWorkbookTemplate    = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks?uploadSessionId={{iwsUploadSession}}&workbookType={{iwsWorkbookType}}&overwrite=true";
            UrlCreateProjectTemplate             = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/projects";
            UrlDeleteWorkbookTagTemplate         = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/tags/{{iwsTagText}}";
            UrlDeleteDatasourceTagTemplate       = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources/{{iwsDatasourceId}}/tags/{{iwsTagText}}";
            UrlUpdateWorkbookTemplate            = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}";
            UrlUpdateDatasourceTemplate          = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources/{{iwsDatasourceId}}";
            UrlListViewsInSite                    = serverNameWithProtocol + "/api/2.2/sites/{{iwsSiteId}}/views?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlListWorkbooksInSite                = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            UrlDownloadPreviewImageForView_p        = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/views/{{iwsViewId}}/previewImage";
            UrlCreateSite_p = serverNameWithProtocol + "/api/2.4/sites";
            UrlAddUserToSite_p = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/users";
            UrlDownloadDatasourceConnections = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources/{{iwsDatasourceId}}/connections";
            UrlRemoveUserFromSite_p = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/users/{{iwsUserId}}";
            UrlUpdateDatasourceConnection_p = serverNameWithProtocol + "/api/2.4/sites/{{iwsSiteId}}/datasources/{{iwsDatasourceId}}/connections/{{iwsConnectionId}}";

            //Any server version specific things we want to do?
            switch (serverVersion)
            {
                case ServerVersion.server8:
                    throw new Exception("This app does not support v8 Server");
                case ServerVersion.server9:
                    break;
                default:
                    //AppDiagnostics.Assert(false, "Unknown server version");
                    throw new Exception("Unknown server version");
            }
        }

        //Parse out the http:// or https://
        private static string GetProtocolFromUrl(string url)
        {
            const string protocolIndicator = "://";
            int idxProtocol = url.IndexOf(protocolIndicator);
            if(idxProtocol < 1)
            {
                throw new Exception("No protocol found in " + url);
            }

            string protocol = url.Substring(0, idxProtocol + protocolIndicator.Length);

            return protocol.ToLower();
        }

        /// <summary>
        /// Parse out the server-user and site name from the content URL
        /// </summary>
        /// <param name="userContentUrl">e.g. https://online.tableausoftware.com/t/tableausupport/workbooks</param>
        /// 
        /// <returns></returns>
        public static TableauServerUrls FromContentUrl(string userContentUrl, int pageSize)
        {
            userContentUrl = userContentUrl.Trim();
            string foundProtocol = GetProtocolFromUrl(userContentUrl);

            //Find where the server name ends
            string urlAfterProtocol = userContentUrl.Substring(foundProtocol.Length);
            var urlParts = urlAfterProtocol.Split('/');
            string serverName = urlParts[0];

            string siteUrlSegment;
            ServerVersion serverVersion;
            //Check for the site specifier.  Infer the server version based on this URL
            if(urlParts[1] == "t")
            {
                siteUrlSegment = urlParts[2];
                serverVersion = ServerVersion.server8;
            }
            else if((urlParts[1] == "#") && (urlParts[2] == "site"))
            {
                siteUrlSegment = urlParts[3];
                serverVersion = ServerVersion.server9;
            }
            else if (urlParts[1] == "#")
            {
                siteUrlSegment = ""; //Default site
                serverVersion = ServerVersion.server9;
            }
            else
            {
                throw new Exception("Expected /t site splitter in url");
            }
         
            return new TableauServerUrls(foundProtocol, serverName, siteUrlSegment, pageSize, serverVersion);
        }

        /// <summary>
        /// The URL to get site info
        /// </summary>
        /// <param name="logInInfo"></param>
        /// <returns></returns>
        public string Url_SiteInfo(TableauServerSignIn logInInfo)
        {
            string workingText = UrlSiteInfoTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// The URL to start na upload
        /// </summary>
        /// <param name="logInInfo"></param>
        /// <returns></returns>
        public string Url_InitiateFileUpload(TableauServerSignIn logInInfo)
        {
            string workingText = UrlInitiateUploadTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// The URL to start a upload
        /// </summary>
        /// <param name="logInInfo"></param>
        /// <returns></returns>
        public string Url_AppendFileUploadChunk(TableauServerSignIn logInInfo, string uploadSession)
        {
            string workingText = UrlAppendUploadChunkTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUploadSession}}", uploadSession);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }


        /// <summary>
        /// URL to finish publishing a datasource
        /// </summary>
        /// <param name="logInInfo"></param>
        /// <param name="uploadSession"></param>
        /// <param name="datasourceType"></param>
        /// <returns></returns>
        public string Url_FinalizeDataSourcePublish(TableauServerSignIn logInInfo, string uploadSession, string datasourceType)
        {

            string workingText = UrlFinalizeUploadDatasourceTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUploadSession}}", uploadSession);
            workingText = workingText.Replace("{{iwsDatasourceType}}", datasourceType);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL to finish publishing a datasource
        /// </summary>
        /// <param name="logInInfo"></param>
        /// <param name="uploadSession"></param>
        /// <param name="datasourceType"></param>
        /// <returns></returns>
        public string Url_FinalizeWorkbookPublish(TableauServerSignIn logInInfo, string uploadSession, string workbookType)
        {

            string workingText = UrlFinalizeUploadWorkbookTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUploadSession}}", uploadSession);
            workingText = workingText.Replace("{{iwsWorkbookType}}", workbookType);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Workbooks list
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_WorkbooksListForUser(TableauServerSignIn session, string userId, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListWorkbooksForUserTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsUserId}}", userId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Workbook's data source connections list
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_WorkbookConnectionsList(TableauServerSignIn session, string workbookId)
        {
            string workingText = UrlListWorkbookConnectionsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for a Datasource's connections list
        /// </summary>
        /// <param name="session"></param>
        /// <param name="datasourceId"></param>
        /// <returns></returns>
        internal string Url_DatasourceConnectionsList(TableauServerSignIn session, string datasourceId)
        {
            string workingText = UrlDownloadDatasourceConnections;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsDatasourceId}}", datasourceId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }


        /// <summary>
        /// URL for the Datasources list
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_DatasourcesList(TableauServerSignIn session, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListDatasourcesTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for creating a project
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_CreateProject(TableauServerSignIn session)
        {
            string workingText = UrlCreateProjectTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for deleting a tag from a workbook
        /// </summary>
        /// <param name="session"></param>
        /// <param name="workbookId"></param>
        /// <param name="tagText">Tag we want to delete</param>
        /// <returns></returns>
        public string Url_DeleteWorkbookTag(TableauServerSignIn session, string workbookId, string tagText)
        {
            string workingText = UrlDeleteWorkbookTagTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            workingText = workingText.Replace("{{iwsTagText}}", tagText);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for updating workbook metadata (e.g. owners, show tabs)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="workbookId"></param>
        /// <param name="tagText">Tag we want to delete</param>
        /// <returns></returns>
        public string Url_UpdateWorkbook(TableauServerSignIn session, string workbookId)
        {
            string workingText = UrlUpdateWorkbookTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for updating datasource metadata (e.g. owner id)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="workbookId"></param>
        /// <param name="tagText">Tag we want to delete</param>
        /// <returns></returns>
        public string Url_UpdateDatasource(TableauServerSignIn session, string datasourceId)
        {
            string workingText = UrlUpdateDatasourceTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsDatasourceId}}", datasourceId);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for deleting a tag from a datasource
        /// </summary>
        /// <param name="session"></param>
        /// <param name="workbookId"></param>
        /// <param name="tagText">Tag we want to delete</param>
        /// <returns></returns>
        public string Url_DeleteDatasourceTag(TableauServerSignIn session, string datasourceId, string tagText)
        {
            string workingText = UrlDeleteDatasourceTagTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsDatasourceId}}", datasourceId);
            workingText = workingText.Replace("{{iwsTagText}}", tagText);
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }


        /// <summary>
        /// URL for the Projects list
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_ProjectsList(TableauServerSignIn session, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListProjectsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Groups list
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_GroupsList(TableauServerSignIn session, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListGroupsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Users list
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_UsersList(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListUsersTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL to get the list of Users in a Group
        /// </summary>
        /// <param name="logInInfo"></param>
        /// <param name="groupId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public string Url_UsersListInGroup(TableauServerSignIn logInInfo, string groupId, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListUsersInGroupTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsGroupId}}", groupId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        public string UrlDownloadViewsForSite(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListViewsInSite.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        public string UrlDownloadWorkbooksForSite(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = UrlListWorkbooksInSite.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        public string UrlDownloadPreviewImageForView(TableauServerSignIn logInInfo, string workbookId, string viewId)
        {
            string workingText = UrlDownloadPreviewImageForView_p.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            workingText = workingText.Replace("{{iwsViewId}}", viewId);
            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        public string UrlCreateSite()
        {
            string workingText = UrlCreateSite_p;
            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        public string UrlAddUserToSite(string siteId)
        {
            string workingText = UrlAddUserToSite_p;
            workingText = workingText.Replace("{{iwsSiteId}}", siteId);
            return workingText;
        }

        /// <summary>
        /// URL to download a workbook
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_WorkbookDownload(TableauServerSignIn session, SiteWorkbook contentInfo)
        {
            string workingText = UrlDownloadWorkbookTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsRepositoryId}}", contentInfo.Id);

            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        /// <summary>
        /// URL to download a datasource
        /// </summary>
        /// <param name="siteUrlSegment"></param>
        /// <returns></returns>
        public string Url_DatasourceDownload(TableauServerSignIn session, SiteDatasource contentInfo)
        {
            string workingText = UrlDownloadDatasourceTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsRepositoryId}}", contentInfo.Id);

            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        public string UrlRemoveUserFromSite(TableauServerSignIn session, SiteUser userToRemove)
        {
            string workingText = UrlRemoveUserFromSite_p;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsUserId}}", userToRemove.Id);
            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        public string UrlUpdateDatasourceConnection(TableauServerSignIn session, SiteDatasource datasourceToUpdate, SiteConnection connectionToUpdate)
        {
            string workingText = UrlUpdateDatasourceConnection_p;
            workingText = workingText.Replace("{{iwsSiteId}}", session.SiteId);
            workingText = workingText.Replace("{{iwsDatasourceId}}", datasourceToUpdate.Id);
            workingText = workingText.Replace("{{iwsConnectionId}}", connectionToUpdate.Id);

            ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool ValidateTemplateReplaceComplete(string str)
        {
            if (str.Contains("{{iws"))
            {
                //AppDiagnostics.Assert(false, "Template has incomplete parts that need to be replaced");
                return false;
            }

            return true;
        }

        string ITableauServerSiteInfo.ServerName
        {
            get 
            {
                return this.ServerName; 
            }
        }

        ServerProtocol ITableauServerSiteInfo.Protocol
        {
            get 
            {
                if (this.ServerProtocol == "https://") return global::ServerProtocol.https;
                if (this.ServerProtocol == "http://") return global::ServerProtocol.http;
                throw new Exception("Unknown protocol " + this.ServerProtocol);
            }
        }

        string ITableauServerSiteInfo.SiteId
        {
            get 
            {
                return this.SiteUrlSegement;
            }
        }

        string ITableauServerSiteInfo.ServerNameWithProtocol
        {
            get
            {
                return this.ServerUrlWithProtocol;
            }
        }

    }
}
