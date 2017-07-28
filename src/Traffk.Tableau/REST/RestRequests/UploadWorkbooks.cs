using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.Models;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Uploads a directory full of workbooks 
    /// </summary>
    partial class UploadWorkbooks : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// Path we are uploading from
        /// </summary>
        private readonly string LocalUploadPath;

        private readonly int UploadChunkSizeBytes;  //Max size of upload chunks
        private readonly int UploadChunkDelaySeconds; //Delay afte reach chunk
        private readonly IHttpClientFactory HttpClientFactory;

        /// <summary>
        ///We will use this to find any stored credentials we need to upload 
        /// </summary>
        private readonly CredentialManager CredentialManager;

        /// <summary>
        /// TRUE: After the upload, attempt to reassign the owner of the content
        /// </summary>
        private readonly bool AttemptOwnershipAssignment;

        /// <summary>
        /// List of users in the site (used for looking up user-ids, based on the user names and mapping ownership)
        /// </summary>
        private readonly IEnumerable<SiteUser> SiteUsers;
        private readonly ProjectFindCreateHelper ProjectFindCreateHelper;
        public int UploadSuccesses { get; private set; }
        public int UploadFailures { get; private set; }

        public UploadWorkbooks(
            TableauServerUrls onlineUrls, 
            TableauServerSignIn login,
            CredentialManager credentialManager,
            string localUploadPath,
            IHttpClientFactory httpClientFactory,
            int uploadChunkSizeBytes = TableauServerUrls.UploadFileChunkSize,
            int uploadChunkDelaySeconds = 0)
            : base(onlineUrls, login, httpClientFactory)
        {
            LocalUploadPath = localUploadPath;
            CredentialManager = credentialManager; 

            //If we are going to attempt to reassign ownership after publication we'll need this information
            //Not implemented for now
            AttemptOwnershipAssignment = false;
            SiteUsers = new List<SiteUser>();

            UploadChunkSizeBytes = uploadChunkSizeBytes;
            UploadChunkDelaySeconds = uploadChunkDelaySeconds;

            HttpClientFactory = httpClientFactory;
            ProjectFindCreateHelper = new ProjectFindCreateHelper(Urls, Login, HttpClientFactory);

            UploadSuccesses = 0;
            UploadFailures = 0;
        }

        /// <summary>
        /// Upload all the workbook files
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {
            var statusLog = Login.StatusLog;
            int countSuccess = 0;
            int countFailure = 0;

            statusLog.AddStatus("Uploading workbooks");
            UploadDirectoryToServer(LocalUploadPath, LocalUploadPath, ProjectFindCreateHelper, true, out countSuccess, out countFailure);
            Login.StatusLog.AddStatus("Workbooks upload done.  Success: " + countSuccess.ToString() + ", Failure: " + countFailure.ToString());
        }

        /// <summary>
        /// Looks to see if there are database associated credentials that should be associated with the
        /// content being uploaded
        /// </summary>
        /// <param name="contentFileName">Filename without path</param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        private CredentialManager.Credential helper_DetermineContentCredential(
            string contentFileName, 
            string projectName)
        {
            var credentialManager = CredentialManager;
            //If there is no credential manager, there can be no credentials for any uploaded content
            return credentialManager?.FindWorkbookCredential(
                contentFileName,
                projectName);
        }
        /// <summary>
        /// Uploads the content of the current directory
        /// </summary>
        /// <param name="rootContentPath"></param>
        /// <param name="currentContentPath"></param>
        /// <param name="recurseDirectories"></param>
        private void UploadDirectoryToServer(
            string rootContentPath, 
            string currentContentPath,
            ProjectFindCreateHelper projectsList,
            bool recurseDirectories, 
            out int countSuccess, out int countFailure)
        {
            countSuccess = 0;
            countFailure = 0;

            //--------------------------------------------------------------------------------------------
            //Look up the project name based on directory name, and creating a project on demand
            //--------------------------------------------------------------------------------------------
            string projectName;
            if (rootContentPath == currentContentPath) //If we are in the root upload directory, then assume any content goes to the Default project
            {
                projectName = "Default"; //Default project
            }
            else
            {
                projectName = FileIOHelper.Undo_GenerateWindowsSafeFilename(Path.GetFileName(currentContentPath));
            }

            //Start off with no project ID -- we'll look it up as needed
            string projectIdForUploads = null;

            //-------------------------------------------------------------------------------------
            //Upload the files from local directory to server
            //-------------------------------------------------------------------------------------
            Parallel.ForEach(Directory.GetFiles(currentContentPath), thisFilePath =>
            {
                bool isValidUploadFile = IsValidUploadFile(thisFilePath);

                if (isValidUploadFile)
                {
                    //If we don't yet have a project ID, then get one
                    if (string.IsNullOrWhiteSpace(projectIdForUploads))
                    {
                        projectIdForUploads = projectsList.GetProjectIdForUploads(projectName);
                    }

                    try
                    {
                        //See if there are any credentials we want to publish with the content
                        var dbCredentialsIfAny = helper_DetermineContentCredential(
                            Path.GetFileName(thisFilePath),
                            projectName);

                        //See what content specific settings there may be for this workbook
                        var publishSettings = helper_DetermineContentPublishSettings(thisFilePath);

                        bool wasFileUploaded = AttemptUploadSingleFile(thisFilePath, projectIdForUploads,
                            dbCredentialsIfAny, publishSettings);
                        if (wasFileUploaded)
                        {
                            UploadSuccesses++;
                        }
                    }
                    catch (Exception ex)
                    {
                        UploadFailures++;
                        Login.StatusLog.AddError("Error uploading workbook " + thisFilePath + ". " + ex.Message);
                    }
                }
            });

            //If we are running recursive , then look in the subdirectories too
            if(recurseDirectories)
            {
                int subDirSuccess;
                int subDirFailure;
                foreach(var subDirectory in Directory.GetDirectories(currentContentPath))
                {
                    UploadDirectoryToServer(rootContentPath, subDirectory, ProjectFindCreateHelper, true, out subDirSuccess, out subDirFailure);
                    countSuccess += subDirSuccess;
                    countFailure += subDirFailure;
                }
            }
        }


        /// <summary>
        /// Look up settings associated with this content
        /// </summary>
        /// <param name="workbookWithPath"></param>
        /// <returns></returns>
        WorkbookPublishSettings helper_DetermineContentPublishSettings(string workbookWithPath)
        {
            return WorkbookPublishSettings.GetSettingsForSavedWorkbook(workbookWithPath);
        }

        /// <summary>
        /// Sanity testing on whether the file being uploaded is worth uploading
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        bool IsValidUploadFile(string localFilePath)
        {
            //If the file is a custom settings file for the workbook, then ignore it
            if(WorkbookPublishSettings.IsSettingsFile(localFilePath))
            {
                return false; //Nothing to do, it's just a settings file
            }

            //Ignore temp files, since we know we don't want to upload them
            var fileExtension = Path.GetExtension(localFilePath).ToLower();
            if ((fileExtension == ".tmp") || (fileExtension == ".temp"))
            {
                Login.StatusLog.AddStatus("Ignoring temp file, " + localFilePath, -10);
                return false;
            }        

            //These are the only kinds of files we know about...
            if ((fileExtension != ".twb") && (fileExtension != ".twbx"))
            {
                Login.StatusLog.AddError("File is not a workbook: " + localFilePath);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisFilePath"></param>
        /// <param name="projectIdForUploads"></param>
        /// <param name="dbCredentials">If not NULL, then these are the DB credentials we want to associate with the content we are uploading</param>
        /// <param name="publishSettings">Workbook publish settings (e.g. whether to show tabs in vizs)</param>
        private bool AttemptUploadSingleFile(
            string thisFilePath, 
            string projectIdForUploads,
            CredentialManager.Credential dbCredentials,
            WorkbookPublishSettings publishSettings)
        {
            return AttemptUploadSingleFile_Inner(thisFilePath, projectIdForUploads, dbCredentials, publishSettings);
        }

        /// <summary>
        /// Attempts to upload a single file a Tableau Server, and then make it a published workbook
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        private bool AttemptUploadSingleFile_Inner(
            string localFilePath, 
            string projectId, 
            CredentialManager.Credential dbCredentials,
            WorkbookPublishSettings publishSettings)
        {
            string uploadSessionId;
            try
            {
                var fileUploader = new UploadFile(Urls, Login, localFilePath, HttpClientFactory, UploadChunkSizeBytes, UploadChunkDelaySeconds);
                uploadSessionId = fileUploader.ExecuteRequest();
            }
            catch (Exception exFileUpload)
            {
                Login.StatusLog.AddError("Unexpected error attempting to upload file " + localFilePath + ", " + exFileUpload.Message);
                throw exFileUpload;
            }

            SiteWorkbook workbook;
            Login.StatusLog.AddStatus("File chunks upload successful. Next step, make it a published workbook", -10);
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(localFilePath);
                string uploadType = RemoveFileExtensionDot(Path.GetExtension(localFilePath).ToLower());
                workbook = FinalizePublish(
                    uploadSessionId,
                    FileIOHelper.Undo_GenerateWindowsSafeFilename(fileName), //[2016-05-06] If the name has escapted characters, unescape them 
                    uploadType, 
                    projectId, 
                    dbCredentials,
                    publishSettings);
                Login.StatusLog.AddStatus("Upload content details: " + workbook.ToString(), -10);
                Login.StatusLog.AddStatus("Success! Uploaded workbook " + Path.GetFileName(localFilePath));
            }
            catch (Exception exPublishFinalize)
            {
                Login.StatusLog.AddError("Unexpected error finalizing publish of file " + localFilePath + ", " + exPublishFinalize.Message);
                throw exPublishFinalize;
            }

            //See if we want to reassign ownership of the workbook
            if(AttemptOwnershipAssignment)
            {
                //try
                //{
                //    AttemptOwnerReassignment(workbook, publishSettings, SiteUsers);
                //}
                //catch (Exception exOwnershipAssignment)
                //{
                //    Login.StatusLog.AddError("Unexpected error reassigning ownership of published workbook " + workbook.Name + ", " + exOwnershipAssignment.Message);
                //    LogManualAction_ReassignOwnership(workbook.Name);
                //    throw exOwnershipAssignment;
                //}
            }

            return true;     //Success
        }


        /// <summary>
        /// If the file extension has a leading '.', remove it.
        /// </summary>
        /// <param name="txtIn"></param>
        /// <returns></returns>
        private string RemoveFileExtensionDot(string txtIn)
        {
            if (string.IsNullOrWhiteSpace(txtIn)) return "";
            if (txtIn[0] == '.') return txtIn.Substring(1);
            return txtIn;
        }

        /// <summary>
        /// After a file has been uploaded in chunks, we need to make a call to COMMIT the file to server as a published Workbook
        /// </summary>
        /// <param name="uploadSessionId"></param>
        /// <param name="publishedContentName"></param>
        private SiteWorkbook FinalizePublish(
            string uploadSessionId, 
            string publishedContentName, 
            string publishedContentType, 
            string projectId,
            CredentialManager.Credential dbCredentials,
            WorkbookPublishSettings publishSettings)
        {
            if (projectId == null)
            {
                projectId = "";
            }
            //See definition: http://onlinehelp.tableau.com/current/api/rest_api/en-us/help.htm#REST/rest_api_ref.htm#Publish_Workbook%3FTocPath%3DAPI%2520Reference%7C_____29
            var sb = new StringBuilder();

            //Build the XML part of the MIME message we will post up to server
            var xmlWriter = XmlWriter.Create(sb, XmlHelper.XmlSettingsForWebRequests);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("workbook");
            xmlWriter.WriteAttributeString("name", publishedContentName);
            xmlWriter.WriteAttributeString("showTabs", XmlHelper.BoolToXmlText(publishSettings.ShowTabs));

            //If we have an associated database credential, write it out
            if (dbCredentials != null)
            {
                CredentialXmlHelper.WriteCredential(
                    xmlWriter, 
                    dbCredentials);
            }

            xmlWriter.WriteStartElement("project"); //<project>
            xmlWriter.WriteAttributeString("id", projectId);
            xmlWriter.WriteEndElement();            //</project>
            xmlWriter.WriteEndElement(); // </workbook>
            xmlWriter.WriteEndElement(); // </tsRequest>
            xmlWriter.Dispose();

            var xmlText = sb.ToString(); //Get the XML text out

            //Generate the MIME message and pack the XML into it
            var mimeGenerator = new MimeWriterXml(xmlText);

            //Create a web request to POST the MIME message to server to finalize the publish
            var urlFinalizeUpload = Urls.Url_FinalizeWorkbookPublish(Login, uploadSessionId, publishedContentType);

            //NOTE: The publish finalization step can take several minutes, because server needs to unpack the uploaded ZIP and file it away.
            //      For this reason, we pass in a long timeout
            var response = CreateAndSendMimeLoggedInRequest(urlFinalizeUpload, HttpMethod.Post, mimeGenerator); 
            using (response)
            {
                var xmlDoc = GetHttpResponseAsXml(response);
                var xDoc = xmlDoc.ToXDocument();
                var workbookXml = xDoc.Root.Descendants(XName.Get("workbook", XmlNamespace)).FirstOrDefault();
                try
                {
                    return new SiteWorkbook(workbookXml.ToXmlNode(), XmlNamespace);
                }
                catch(Exception parseXml)
                {
                    Login.StatusLog.AddError("Workbook upload, error parsing XML response " + parseXml.Message + "\r\n" + workbookXml.ToXmlNode().InnerXml);
                    return null;
                }
            }
        }
    }
}
