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
    /// Downloads the list of Workbooks from the server
    /// </summary>
    public class DownloadWorkbooksList : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls urls;
        private readonly string UserId;
        private readonly string xmlNamespace;


        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private List<SiteWorkbook> Workbooks_p;
        public ICollection<SiteWorkbook> Workbooks
        {
            get
            {
                var wb = Workbooks_p;
                if (wb == null) return null;
                return wb.AsReadOnly();
            }
        }

        public DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(login, httpClientFactory)
        {
            urls = onlineUrls;
            UserId = login.UserId;
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
            xmlNamespace = nsManager.LookupNamespace("iwsOnline");
        }

        public DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn login, string user, IHttpClientFactory httpClientFactory) 
            : base(login, httpClientFactory)
        {
            urls = onlineUrls;
            this.UserId = user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {
            //Sanity check
            if(string.IsNullOrWhiteSpace(UserId))
            {
                Login.Logger.Error("User ID required to query workbooks");            
            }

            var onlineWorkbooks = new List<SiteWorkbook>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                ExecuteRequest_ForPage(onlineWorkbooks, thisPage, out numberPages);
            }

            Workbooks_p = onlineWorkbooks;
        }

        /// <summary>
        /// Get a page's worth of Workbook listings
        /// </summary>
        /// <param name="onlineWorkbooks"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void ExecuteRequest_ForPage(List<SiteWorkbook> onlineWorkbooks, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = urls.PageSize;
            if (pageToRequest < 1)
            {
                pageToRequest = 1;
            }
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = urls.UrlDownloadWorkbooksForSite(Login, pageSize, pageToRequest);
            var webRequest = CreateLoggedInRequest(urlQuery, HttpMethod.Get);

            Login.Logger.Information("Web request: " + urlQuery);
            var response = SendHttpRequest(webRequest);
            var xmlDoc = GetHttpResponseAsXml(response);

            //Get all the workbook nodes
            var xDoc = xmlDoc.ToXDocument();
            var workbookElements = xDoc.Root.Descendants(XName.Get("workbook", xmlNamespace));

            //Get information for each of the data sources
            foreach (var element in workbookElements)
            {
                try
                {
                    var workbook = ParseWorkbookElement(element);
                    onlineWorkbooks.Add(workbook);
                }
                catch
                {
                    Login.Logger.Error("Error parsing workbook: " + element.Value);
                }
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------
            var paginationElement = xDoc.Root.Descendants(XName.Get("pagination", xmlNamespace)).FirstOrDefault();
            totalNumberPages = GetPageCount(paginationElement, pageSize);
        }

        private SiteWorkbook ParseWorkbookElement(XElement element)
        {
            try
            {
                var workbookXml = element.ToXmlNode();
                var workbook = new SiteWorkbook(workbookXml, xmlNamespace);
                return workbook;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
