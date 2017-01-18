using System;
using System.Net;

namespace Traffk.Tableau.REST
{
    /// <summary>
    /// Abstract class for making requests AFTER having logged into the server
    /// </summary>
    public abstract class TableauServerSignedInRequestBase : TableauServerRequestBase
    {
        protected readonly TableauServerSignIn _onlineSession;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="login"></param>
        public TableauServerSignedInRequestBase(TableauServerSignIn login)
        {
            _onlineSession = login;
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
            _onlineSession.StatusLog.AddStatus("Attempt web request: " + url, -10);

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
            request.Headers["X-Tableau-Auth"] = _onlineSession.LogInAuthToken;
            _onlineSession.StatusLog.AddStatus("Append header X-Tableau-Auth: " + _onlineSession.LogInAuthToken, -20);
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

    }
}
