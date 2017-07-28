using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.Helpers;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Base class on top of which requests to Tableau Server are based
    /// </summary>
    public abstract class TableauServerRequestBase
    {
        protected readonly IHttpClientFactory HttpClientFactory;
        public const int DefaultLongRequestTimeOutMs = 15 * 60 * 1000;  //15 minutes * 60 sec/minute * 1000 ms/sec

        protected TableauServerRequestBase(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Sends the body text up to the server
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bodyText"></param>
        protected HttpResponseMessage SendHttpRequest(HttpRequestMessage request, string bodyText = "")
        {
            Requires.NonNull(request.Method, nameof(request.Method));
            request.Headers.TryAddWithoutValidation("Content-Type", "application/xml;charset=utf-8");
            if (!String.IsNullOrEmpty(bodyText))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(bodyText);
                request.Content = new ByteArrayContent(byteArray, 0, byteArray.Length);
            }
            
            return GetHttpResponseAsync(request).ExecuteSynchronously();
        }

        protected HttpResponseMessage SendHttpRequest(HttpRequestMessage request, MimeWriterBase mimeToSend)
        {
            Requires.NonNull(request.Method, nameof(request.Method));
            var uploadMimeChunk = mimeToSend.GenerateMimeEncodedChunk();
            request.Headers.TryAddWithoutValidation("Content-Type", "multipart/mixed; boundary=" + mimeToSend.MimeBoundaryMarker);
            request.Content = new ByteArrayContent(uploadMimeChunk, 0, uploadMimeChunk.Length);
            return GetHttpResponseLongRunningAsync(request).ExecuteSynchronously();
        }

        private async Task<HttpResponseMessage> GetHttpResponseAsync(HttpRequestMessage request)
        {
            try
            {
                Requires.NonNull(request.RequestUri, nameof(request.RequestUri));
                Requires.NonNull(request.Method, nameof(request.Method));

                var httpClient = HttpClientFactory.Create();
                var response = httpClient.SendAsync(request);
                return await response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<HttpResponseMessage> GetHttpResponseLongRunningAsync(HttpRequestMessage request)
        {
            try
            {
                Requires.NonNull(request.RequestUri, nameof(request.RequestUri));
                Requires.NonNull(request.Method, nameof(request.Method));

                var httpClient = HttpClientFactory.Create();
                httpClient.Timeout = TimeSpan.FromMilliseconds(DefaultLongRequestTimeOutMs);
                var response = httpClient.SendAsync(request);
                return await response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected XmlDocument GetHttpResponseAsXml(HttpResponseMessage response)
        {
            string streamText = "";
            var responseStream = response.Content.ReadAsStreamAsync().ExecuteSynchronously();
            using (responseStream)
            {
                var streamReader = new StreamReader(responseStream);
                using (streamReader)
                {
                    streamText = streamReader.ReadToEnd();
                    streamReader.Dispose();
                }
                responseStream.Dispose();
            }

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(streamText);
            return xmlDoc;
        }
    }
}
