﻿using System;
using System.Net;
using System.Xml.Linq;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
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

    }
}
