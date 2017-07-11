using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using RevolutionaryStuff.Core;
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
        protected HttpRequestMessage CreateLoggedInRequest(string url, HttpMethod method)
        {
            //Login.StatusLog.AddStatus("Attempt web request: " + url, -10);
            var request = new HttpRequestMessage(method, url);
            AppendLoggedInHeadersForRequest(request);
            return request;
        }

        /// <summary>
        /// Adds header information that authenticates the request to Tableau Online
        /// </summary>
        private void AppendLoggedInHeadersForRequest(HttpRequestMessage request)
        {
            request.Headers.TryAddWithoutValidation("X-Tableau-Auth", Login.LogInAuthToken);
        }

        private void AppendLoggedInHeadersForClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("X-Tableau-Auth", Login.LogInAuthToken);
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

        protected HttpResponseMessage CreateAndSendMimeLoggedInRequest(string url, HttpMethod method, MimeWriterBase mimeToSend, int? requestTimeout = null)
        {
            var request = this.CreateLoggedInRequest(url, method);
            return SendHttpRequest(request, mimeToSend);
        }
    }
}
