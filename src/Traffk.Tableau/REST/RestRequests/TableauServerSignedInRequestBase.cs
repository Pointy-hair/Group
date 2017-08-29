using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.Helpers;
using Traffk.Utility;
using TableauServerRequestBase = Traffk.Tableau.REST.RestRequests.TableauServerRequestBase;

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

        protected TableauServerSignedInRequestBase(TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            Login = login;
        }

        protected TableauServerSignedInRequestBase(TableauServerUrls urls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
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
            //Login.Logger.Information("Attempt web request: " + url);
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
                Login.Logger.Error("Download failed after " + (DateTime.Now - startDownload).TotalSeconds.ToString("#.#") + " seconds. " + urlDownload);
                throw exDownload;
            }

            var finishDownload = DateTime.Now;
            Login.Logger.Information("Download success duration " + (finishDownload - startDownload).TotalSeconds.ToString("#.#") + " seconds. " + urlDownload);
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

            var webClient = CreateLoggedInWebClient();

            using (webClient)
            {
                //Choose a temp file name to download to
                var starterName = System.IO.Path.Combine(downloadToDirectory, baseFilename + ".tmp");
                Login.Logger.Information("Attempting file download: " + urlDownload);
                
                using (HttpResponseMessage response = webClient.GetAsync(urlDownload, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    response.EnsureSuccessStatusCode();

                    using (
                        Stream contentStream = response.Content.ReadAsStreamAsync().Result,
                            fileStream = new FileStream(starterName, FileMode.Create, FileAccess.Write, FileShare.None,
                                8192, true))
                    {
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = contentStream.ReadAsync(buffer, 0, buffer.Length).Result;
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                fileStream.WriteAsync(buffer, 0, read);
                            }
                        } while (isMoreToRead);
                    }

                    //Look up the correct file extension based on the content type downloaded
                    var contentType = response.Content.Headers.ContentType.ToString();
                    var fileExtension = downloadTypeMapper.GetFileExtension(contentType);
                    var finishName = System.IO.Path.Combine(downloadToDirectory, baseFilename + fileExtension);

                    //Rename the downloaded file
                    System.IO.File.Move(starterName, finishName);
                    return finishName;
                }
            }
        }

        /// <summary>
        /// Web client class used for downloads from Tableau Server
        /// </summary>
        /// <returns></returns>
        protected HttpClient CreateLoggedInWebClient()
        {
            var loggedInClient = base.HttpClientFactory.Create();
            loggedInClient.Timeout = TimeSpan.FromMilliseconds(DefaultLongRequestTimeOutMs);

            AppendLoggedInHeadersForClient(loggedInClient);
            return loggedInClient;
        }


        protected HttpResponseMessage CreateAndSendMimeLoggedInRequest(string url, HttpMethod method, MimeWriterBase mimeToSend, int? requestTimeout = null)
        {
            var request = this.CreateLoggedInRequest(url, method);
            return SendHttpRequest(request, mimeToSend);
        }
    }
}
