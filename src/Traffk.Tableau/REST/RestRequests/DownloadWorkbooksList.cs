using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Downloads the list of Workbooks from the server
    /// </summary>
    public class DownloadWorkbooksList : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls urls;
        private readonly string userId;
        private readonly string xmlNamespace;


        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private List<SiteWorkbook> _workbooks;
        public ICollection<SiteWorkbook> Workbooks
        {
            get
            {
                var wb = _workbooks;
                if (wb == null) return null;
                return wb.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the workbooks on behalf of the currently logged in user
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="login"></param>
        public DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            urls = onlineUrls;
            userId = login.UserId;
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
            xmlNamespace = nsManager.LookupNamespace("iwsOnline");
        }

        /// <summary>
        /// Constructor: Call when we want to query the Workbooks on behalf of an explicitly specified user
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="user"></param>
        public DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn login, string user) : base(login)
        {
            urls = onlineUrls;
            this.userId = user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {
            //Sanity check
            if(string.IsNullOrWhiteSpace(userId))
            {
                onlineSession.StatusLog.AddError("User ID required to query workbooks");            
            }

            var onlineWorkbooks = new List<SiteWorkbook>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    ExecuteRequest_ForPage(onlineWorkbooks, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    //TODO:Serilog
                    // .AddError("Workbooks error during page request: " + exPageRequest.Message);
                }
            }

            _workbooks = onlineWorkbooks;
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
            var urlQuery = urls.UrlDownloadWorkbooksForSite(onlineSession, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            onlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, "get workbooks list");
            var xmlDoc = GetWebResponseAsXml(response);

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
                    onlineSession.StatusLog.AddError("Error parsing workbook: " + element.Value);
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
