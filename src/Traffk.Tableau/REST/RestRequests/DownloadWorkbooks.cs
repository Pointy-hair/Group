using System;
using System.Collections.Generic;
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.Models;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Downloads a set of workbooks from server
    /// </summary>
    public class DownloadWorkbooks : TableauServerSignedInRequestBase
    {
        private readonly string LocalSavePath;
        private readonly bool GenerateInfoFile;
        private readonly IEnumerable<SiteWorkbook> WorkbooksToDownload;


        public DownloadWorkbooks(
            TableauServerUrls onlineUrls, 
            TableauServerSignIn login,
            IEnumerable<SiteWorkbook> workbooksToDownload, 
            string localSavePath, 
            IHttpClientFactory httpClientFactory,
            bool generateInfoFile = false)
            : base(onlineUrls, login, httpClientFactory)
        {
            WorkbooksToDownload = workbooksToDownload;
            LocalSavePath = localSavePath;
            GenerateInfoFile = generateInfoFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public ICollection<SiteWorkbook> ExecuteRequest()
        {
            var statusLog = Login.StatusLog;
            var downloadedContent = new List<SiteWorkbook>();

            if (WorkbooksToDownload == null)
            {
                statusLog.AddError("NULL workbooks. Aborting download.");
                return null;
            }

            //Depending on the HTTP download file type we want different file extensions
            var typeMapper = new DownloadPayloadTypeHelper("twbx", "twb");

            foreach (var contentInfo in WorkbooksToDownload)
            {
                //Local path save the workbook
                string urlDownload = Urls.Url_WorkbookDownload(Login, contentInfo);
                statusLog.AddStatus("Starting Workbook download " + contentInfo.Name + " " + contentInfo.ToString());
                try
                {
                    ////Generate the directory name we want to download into
                    //var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(
                    //    LocalSavePath,
                    //    _downloadToProjectDirectories,
                    //    contentInfo,
                    //    this.StatusLog);

                    var fileDownloaded = this.DownloadFile(urlDownload, LocalSavePath, contentInfo.Name, typeMapper);
                    var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                    statusLog.AddStatus("Finished Workbook download " + fileDownloadedNoPath);

                    //Add to the list of our downloaded workbooks, and save metadata
                    if (!string.IsNullOrWhiteSpace(fileDownloaded))
                    {
                        downloadedContent.Add(contentInfo);

                        //Generate the metadata file that has additional server provided information about the workbook
                        //if (GenerateInfoFile)
                        //{
                        //    WorkbookPublishSettings.CreateSettingsFile(contentInfo, fileDownloaded, _siteUserLookup);
                        //}
                    }
                    else
                    {
                        //We should never hit this code; just being defensive
                        statusLog.AddError("Download error, no local file path for downloaded content");
                    }
                }
                catch (Exception ex)
                {
                    statusLog.AddError("Error during Workbook download " + contentInfo.Name + "\r\n  " + urlDownload +
                                       "\r\n  " + ex.ToString());
                }
            } //foreach

            return downloadedContent;
        }

    }
}
