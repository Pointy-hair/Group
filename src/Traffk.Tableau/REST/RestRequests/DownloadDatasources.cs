using System;
using System.Collections.Generic;
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Manages the download of a set of data sources from a Tableau Server site
    /// </summary>
    public class DownloadDatasources : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// Datasources we've parsed from server results
        /// </summary>
        private readonly IEnumerable<SiteDatasource> Datasources;

        /// <summary>
        /// Local directory to save to
        /// </summary>
        private readonly string LocalSavePath;

        /// <summary>
        /// If not NULL, put downloads into directories named like the projects they belong to
        /// </summary>
        private readonly IProjectsList DownloadToProjectDirectories;

        /// <summary>
        /// If TRUE a companion XML file will be generated for each downloaded Workbook with additional
        /// information about it that is useful for uploads
        /// </summary>
        private readonly bool GenerateInfoFile;

        /// <summary>
        /// May be NULL.  If not null, this is the list of sites users, so we can look up the user name
        /// </summary>
        private readonly KeyedLookup<SiteUser> SiteUserLookup;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="datasources"></param>
        /// <param name="localSavePath"></param>
        /// <param name="projectsList"></param>
        /// <param name="generateInfoFile">TRUE = Generate companion file for each download that contains metadata (e.g. whether "show tabs" is selected, the owner, etc)</param>
        /// <param name="siteUsersLookup">If not NULL, use to look up the owners name, so we can write it into the InfoFile for the downloaded content</param>
        public DownloadDatasources(
            TableauServerUrls onlineUrls, 
            TableauServerSignIn login, 
            IEnumerable<SiteDatasource> datasources,
            string localSavePath,
            IProjectsList projectsList,
            bool generateInfoFile,
            KeyedLookup<SiteUser> siteUserLookup)
            : base(onlineUrls, login)
        {
            Datasources = datasources;
            LocalSavePath = localSavePath;
            DownloadToProjectDirectories = projectsList;
            GenerateInfoFile = generateInfoFile;
            SiteUserLookup = siteUserLookup;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public List<SiteDatasource> ExecuteRequest()
        {
            var statusLog = Login.StatusLog;
            var downloadedContent = new List<SiteDatasource>();

            //Depending on the HTTP download file type we want different file extensions
            var typeMapper = new DownloadPayloadTypeHelper("tdsx", "tds");

            var datasources = Datasources;
            if (datasources == null)
            {
                statusLog.AddError("NULL datasources. Aborting download.");
                return null;
            }

            //For each datasource, download it and save it to the local file system
            foreach (var dsInfo in datasources)
            {
                //Local path save the workbook
                string urlDownload = Urls.Url_DatasourceDownload(Login, dsInfo);
                statusLog.AddStatus("Starting Datasource download " + dsInfo.Name);
                try
                {
                    //Generate the directory name we want to download into
                    var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(
                        LocalSavePath,
                        DownloadToProjectDirectories,
                        dsInfo,
                        statusLog);

                    var fileDownloaded = this.DownloadFile(urlDownload, pathToSaveTo, dsInfo.Name, typeMapper);
                    var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                    statusLog.AddStatus("Finished Datasource download " + fileDownloadedNoPath);

                    //Add to the list of our downloaded data sources
                    if(!string.IsNullOrEmpty(fileDownloaded))
                    {
                        downloadedContent.Add(dsInfo);

                        //Generate the metadata file that has additional server provided information about the workbook
                        if (GenerateInfoFile)
                        {
                            Helpers.DatasourcePublishSettings.CreateSettingsFile(dsInfo, fileDownloaded, SiteUserLookup);
                        }
                    }
                    else
                    {
                        //We should never hit this code; just being defensive
                        statusLog.AddError("Download error, no local file path for downloaded content");
                    }
                }
                catch(Exception ex)
                {
                    statusLog.AddError("Error during Datasource download " + dsInfo.Name + "\r\n  " + urlDownload + "\r\n  " + ex.ToString());
                }
            } //foreach


            //Return the set of successfully downloaded content
            return downloadedContent;
        }
    }
}
