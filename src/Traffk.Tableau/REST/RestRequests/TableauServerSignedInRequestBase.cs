using System;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using Traffk.Tableau.REST.Helpers;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Abstract class for making requests AFTER having logged into the server
    /// </summary>
    public abstract class TableauServerSignedInRequestBase : TableauServerRequestBase
    {
        protected readonly TableauServerSignIn Login;
        protected readonly string TableauXmlNamespaceName = "iwsOnline";
        protected readonly string TableauXmlNamespaceUrl = "http://tableau.com/api";
        protected readonly TableauServerUrls Urls;
        protected readonly string XmlNamespace;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="login"></param>
        protected TableauServerSignedInRequestBase(TableauServerSignIn login)
        {
            Login = login;
        }

        protected TableauServerSignedInRequestBase(TableauServerUrls urls, TableauServerSignIn login)
        {
            Login = login;
            Urls = urls;
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager(TableauXmlNamespaceName, TableauXmlNamespaceUrl);
            XmlNamespace = nsManager.LookupNamespace(TableauXmlNamespaceName);
        }

        /// <summary>
        /// Creates a web request and appends the user credential tokens necessary
        /// </summary>
        /// <param name="url"></param>
        /// <param name="protocol"></param>
        /// <param name="requestTimeout">Useful for specifying timeouts for operations that can take a long time</param>
        /// <returns></returns>
        protected WebRequest CreateLoggedInWebRequest(string url, string protocol = "GET", Nullable<int> requestTimeout = null)
        {
            Login.StatusLog.AddStatus("Attempt web request: " + url, -10);

            var webRequest = WebRequest.Create(url);
            webRequest.Method = protocol;

            //If an explicit timeout was passed in then use it
            //if (requestTimeout != null)
            //{
            //    webRequest.Timeout = requestTimeout.Value;
            //}

            AppendLoggedInHeadersForRequest(webRequest);
            return webRequest;
        }

        /// <summary>
        /// Adds header information that authenticates the request to Tableau Online
        /// </summary>
        /// <param name="webHeaders"></param>
        private void AppendLoggedInHeadersForRequest(WebRequest request)
        {
            request.Headers["X-Tableau-Auth"] = Login.LogInAuthToken;
            Login.StatusLog.AddStatus("Append header X-Tableau-Auth: " + Login.LogInAuthToken, -20);
        }

        private void AppendLoggedInHeadersForClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("X-Tableau-Auth", Login.LogInAuthToken);
        }

        /// <summary>
        /// Get the web response; log any error codes that occur and rethrow the exception.
        /// This allows us to get error log data with detailed information
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        protected WebResponse GetWebReponseLogErrors(WebRequest webRequest, string description)
        {
            string requestUri = webRequest.RequestUri.ToString();
            try
            {
                return webRequest.GetResponseAsync().Result;
            }
            catch (WebException webException)
            {
                throw webException;
            }
        }

        protected int GetPageCount(XElement paginationElement, int pageSize)
        {
            var paginationNode = paginationElement.ToXmlNode();
            return DownloadPaginationHelper.GetNumberOfPagesFromPagination(paginationNode, pageSize);
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="urlDownload"></param>
        /// <param name="downloadToDirectory"></param>
        /// <param name="baseFilename"></param>
        /// <param name="downloadTypeMapper"></param>
        /// <returns>The path to the downloaded file</returns>
        protected string DownloadFile(string urlDownload, string downloadToDirectory, string baseFilename, DownloadPayloadTypeHelper downloadTypeMapper)
        {
            //Lets keep track of how long it took
            var startDownload = DateTime.Now;
            string outputPath;
            try
            {
                outputPath = DownloadFile_inner(urlDownload, downloadToDirectory, baseFilename, downloadTypeMapper);
            }
            catch (Exception exDownload)
            {
                Login.StatusLog.AddError("Download failed after " + (DateTime.Now - startDownload).TotalSeconds.ToString("#.#") + " seconds. " + urlDownload);
                throw exDownload;
            }

            var finishDownload = DateTime.Now;
            Login.StatusLog.AddStatus("Download success duration " + (finishDownload - startDownload).TotalSeconds.ToString("#.#") + " seconds. " + urlDownload, -10);
            return outputPath;
        }

        /// <summary>
        /// Downloads a file
        /// </summary>
        /// <param name="urlDownload"></param>
        /// <param name="downloadToDirectory"></param>
        /// <param name="baseFileName">Filename without extension</param>
        /// <returns>The path to the downloaded file</returns>
        private string DownloadFile_inner(string urlDownload, string downloadToDirectory, string baseFilename, DownloadPayloadTypeHelper downloadTypeMapper)
        {

            //[2016-05-06] Interestingly 'GetFileNameWithoutExtension' does more than remove a ".xxxx" extension; it will also remove a preceding
            //            path (e.g. GetFileNameWithoutExtension('foo/bar.xxx') -> "bar'.  This is undesirable because these characters are valid 
            //            in Tableau Server content names. Since this function is supposed to be called with a 'baseFilename' that DOES NOT have a .xxx
            //            extension, it is safe to remove this call
            //baseFilename =  FileIOHelper.GenerateWindowsSafeFilename(System.IO.Path.GetFileNameWithoutExtension(baseFilename));

            //Strip off an extension if its there
            baseFilename = FileIOHelper.GenerateWindowsSafeFilename(baseFilename);

            var webClient = this.CreateLoggedInWebClient();
            using (webClient)
            {
                //Choose a temp file name to download to
                var starterName = System.IO.Path.Combine(downloadToDirectory, baseFilename + ".tmp");
                Login.StatusLog.AddStatus("Attempting file download: " + urlDownload, -10);
                var response = webClient.DownloadFile(urlDownload, starterName); //Download the file

                //Look up the correct file extension based on the content type downloaded
                var contentType = response.Content.Headers.ContentType.ToString();
                var fileExtension = downloadTypeMapper.GetFileExtension(contentType);
                var finishName = System.IO.Path.Combine(downloadToDirectory, baseFilename + fileExtension);

                //Rename the downloaded file
                System.IO.File.Move(starterName, finishName);
                return finishName;
            }
        }

        /// <summary>
        /// Web client class used for downloads from Tableau Server
        /// </summary>
        /// <returns></returns>
        protected TableauServerWebClient CreateLoggedInWebClient()
        {
            Login.StatusLog.AddStatus("Web client being created", -10);

            var webClient = new TableauServerWebClient(); //Create a WebClient object with a large TimeOut value so that larger content can be downloaded
            AppendLoggedInHeadersForClient(webClient);
            return webClient;
        }

        protected WebRequest CreateAndSendMimeLoggedInRequest(string url, string protocol, MimeWriterBase mimeToSend, Nullable<int> requestTimeout = null)
        {
            var webRequest = this.CreateLoggedInWebRequest(url, protocol, requestTimeout);

            //var uploadChunkAsMime = new OnlineMimeUploadChunk(uploadDataBuffer, numBytes);
            var uploadMimeChunk = mimeToSend.GenerateMimeEncodedChunk();

            //webRequest. = uploadMimeChunk.Length;
            webRequest.ContentType = "multipart/mixed; boundary=" + mimeToSend.MimeBoundaryMarker;

            //Write out the request
            var requestStream = webRequest.GetRequestStreamAsync().Result;
            requestStream.Write(uploadMimeChunk, 0, uploadMimeChunk.Length);

            return webRequest;
        }


    }
}
