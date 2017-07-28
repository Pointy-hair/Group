using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Downloads the list of data sources
    /// </summary>
    public class DownloadDatasourcesList : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private List<SiteDatasource> Datasources_p;
        public ICollection<SiteDatasource> Datasources
        {
            get
            {
                var ds = Datasources_p;
                if (ds == null) return null;
                return ds.AsReadOnly();
            }
        }

        public DownloadDatasourcesList(TableauServerUrls onlineUrls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(onlineUrls,login,httpClientFactory)
        {
        }

        /// <summary>
        /// Request the data from Online
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {

            var onlineDatasources = new List<SiteDatasource>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    ExecuteRequest_ForPage(onlineDatasources, thisPage, out numberPages);
                }
                catch(Exception exPageRequest)
                {
                    Login.StatusLog.AddError("Datasources error during page request: " + exPageRequest.Message);
                }
            }
            Datasources_p = onlineDatasources;
        }

        /// <summary>
        /// Get a page's worth of Data Sources
        /// </summary>
        /// <param name="onlineDatasources"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void ExecuteRequest_ForPage(List<SiteDatasource> onlineDatasources, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = Urls.PageSize; 
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = Urls.Url_DatasourcesList(Login, pageSize, pageToRequest);
            var request = CreateLoggedInRequest(urlQuery, HttpMethod.Get);

            //Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = SendHttpRequest(request);
            var xmlDoc = GetHttpResponseAsXml(response);

            //Get all the nodes
            var xDoc = xmlDoc.ToXDocument();
            var datasources = xDoc.Root.Descendants(XName.Get("datasource", XmlNamespace));
            
            //Get information for each of the data sources
            foreach (var itemXml in datasources)
            {
                try
                {
                    var itemXmlNode = itemXml.ToXmlNode();
                    var ds = new SiteDatasource(itemXmlNode, XmlNamespace);
                    onlineDatasources.Add(ds);
                }
                catch
                {
                    Login.StatusLog.AddError("Error parsing datasource: " + itemXml.ToXmlNode());
                }
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------
            var paginationElement = xDoc.Root.Descendants(XName.Get("pagination", XmlNamespace)).FirstOrDefault();
            totalNumberPages = GetPageCount(paginationElement, pageSize);
        }
    }
}
