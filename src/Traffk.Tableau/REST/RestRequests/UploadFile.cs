using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Traffk.Tableau.REST.Helpers;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Uploads a single file to the server...
    /// </summary>
    class UploadFile : TableauServerSignedInRequestBase
    {
        private readonly string LocalUploadPath;
        private readonly int UploadChunkSize;
        private readonly int UploadChunkDelay;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="localUploadPath"></param>
        /// <param name="uploadChunkSize"></param>
        /// <param name="uploadChunkDelay"></param>
        public UploadFile(
            TableauServerUrls onlineUrls, 
            TableauServerSignIn login,
            string localUploadPath,
            int uploadChunkSize = 8000000,
            int uploadChunkDelay = 0)
            : base(onlineUrls, login)
        {
            LocalUploadPath = localUploadPath;
            UploadChunkSize = uploadChunkSize;
            UploadChunkDelay = uploadChunkDelay;

        }

        /// <summary>
        /// Uploads a single file to a Tableau Server
        /// </summary>
        /// <returns>The ID of the uploaded file; to be used in subsequent calls</returns>
        public string ExecuteRequest()
        {
            TimeSpan uploadDuration;
            return ExecuteRequest(out uploadDuration);

        }
        /// <summary>
        /// Uploads a single file to a Tableau Server
        /// </summary>
        /// <returns>The ID of the uploaded file; to be used in subsequent calls</returns>
        public string ExecuteRequest(out TimeSpan uploadDuration)
        {
            DateTime uploadStartTime;
            DateTime uploadEndTime;

            var statusLog = Login.StatusLog;
            string fileToUpload = LocalUploadPath;

            //Sanity check.
            if(!File.Exists(fileToUpload))
            {
                statusLog.AddError("Aborting. Could not find file " + LocalUploadPath);
                uploadDuration = TimeSpan.FromSeconds(0);
                return null;
            }

            uploadStartTime = DateTime.Now;
            statusLog.AddStatus("Intiating file upload " + fileToUpload);
            var uploadSessionId = RequestUploadSessionId();

            UploadFileInChunks(fileToUpload, uploadSessionId);
            //Determine the upload duration
            uploadEndTime = DateTime.Now;
            uploadDuration = uploadEndTime - uploadStartTime;
            return uploadSessionId;
        }

        /// <summary>
        /// Uploads a file in N-MB chunks to a Tableau Server
        /// </summary>
        /// <param name="fileToUpload"></param>
        /// <param name="uploadSessionId">Upload Session ID to use</param>
        private void UploadFileInChunks(string fileToUpload, string uploadSessionId)
        {
//        const int max_chunk_size = 8 * 1000000; //N MB
            int max_chunk_size = UploadChunkSize;
            System.Diagnostics.Debug.Assert(max_chunk_size > 0, "Non positive chunk size");

            byte[] readbuffer = new byte[max_chunk_size];
            var openFile = File.OpenRead(fileToUpload);
            using(openFile)
            {
                int readBytes;
                do 
                {
                    readBytes = openFile.Read(readbuffer, 0, max_chunk_size);
                    if (readBytes > 0)
                    {
                        UploadSingleChunk(uploadSessionId, readbuffer, readBytes);
                    }

                    ConsiderSleepDelay(); //See if we have an enforced sleep delay
                } while(readBytes > 0);
                openFile.Dispose();
            }
        }

        /// <summary>
        /// See if we have an enforced sleep delay
        /// </summary>
        private void ConsiderSleepDelay()
        {
            //See if we have a testing delay we want to do after every chunk
            int uploadChunkDelay = UploadChunkDelay;
            if (uploadChunkDelay > 0)
            {
                Login.StatusLog.AddStatus("Forced delay " + uploadChunkDelay + " seconds...");
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(uploadChunkDelay));
            }
        }

        /// <summary>
        /// Uploads a single chunk
        /// </summary>
        /// <param name="uploadSessionId"></param>
        private void UploadSingleChunk(string uploadSessionId, byte [] uploadDataBuffer, int numBytes)
        {
            var urlAppendChunk = Urls.Url_AppendFileUploadChunk(Login, uploadSessionId);

            var uploadChunkAsMime = new MimeWriterFileUploadChunk(uploadDataBuffer, numBytes);
            var webRequest = this.CreateAndSendMimeLoggedInRequest(urlAppendChunk, "PUT", uploadChunkAsMime); //NOTE: This command requires a PUT not a GET
            var response = this.GetWebReponseLogErrors(webRequest, "upload file chunk");
            using(response)
            {
                var xmlDoc = GetWebResponseAsXml(response);

                //Get all the workbook nodes
                var xDoc = xmlDoc.ToXDocument();
                var uploadInfo = xDoc.Root.Descendants(XName.Get("fileUpload", XmlNamespace)).FirstOrDefault();
                var chunkUploadXml = uploadInfo.ToXmlNode();

                var verifySessionId = chunkUploadXml.Attributes["uploadSessionId"].Value;
                var fileSizeMB = chunkUploadXml.Attributes["fileSize"].Value;

                if(verifySessionId != uploadSessionId)
                {
                    Login.StatusLog.AddError("Upload sessions do not match! " + uploadSessionId + "/" + verifySessionId);
                }

                //Log verbose status
                Login.StatusLog.AddStatus("Upload chunk status " + verifySessionId + " / " + fileSizeMB + " MB", -10);
            }

        }


        /// <summary>
        /// Get an upload sessiosn Id
        /// </summary>
        /// <returns></returns>
        private string RequestUploadSessionId()
        {
            var urlInitiateFileUpload = Urls.Url_InitiateFileUpload(Login);

            var webRequest = this.CreateLoggedInWebRequest(urlInitiateFileUpload, "POST"); //NOTE: This command requires a POST not a GET
            var response = GetWebReponseLogErrors(webRequest, "get datasources list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var xDoc = xmlDoc.ToXDocument();
            var uploadInfo = xDoc.Root.Descendants(XName.Get("fileUpload", XmlNamespace)).FirstOrDefault();
            var uploadInfoNode = uploadInfo.ToXmlNode();
            var sessionId = uploadInfoNode.Attributes["uploadSessionId"].Value;

            return sessionId;
        }
    }
}
