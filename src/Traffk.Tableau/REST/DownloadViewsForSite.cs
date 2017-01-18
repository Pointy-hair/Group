using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Traffk.Tableau.REST
{
    public class DownloadViewsForSite : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls urls;

        public IEnumerable<SiteView> Views;

        public DownloadViewsForSite(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            urls = onlineUrls;
        }

        public void ExecuteRequest()
        {
            
        }

        private void ExecuteRequestForPage(List<SiteView> views, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = urls.PageSize;
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = urls.UrlDownloadViewsForSite(_onlineSession, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            _onlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, "get views list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the viewe nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var xDoc = xmlDoc.ToXDocument();
            var projectElements = xDoc.Root.Descendants(XName.Get("view", nsManager.LookupNamespace("iwsOnline")));

            //Get information for each of the data sources
            foreach (var element in projectElements)
            {
                var view = ParseSiteXElement(element);
                views.Add(view);
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------

            var paginationElement = xDoc.Root.Descendants(XName.Get("pagination", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();
            totalNumberPages = GetPageCount(paginationElement, pageSize);
        }

        private int GetPageCount(XElement paginationElement, int pageSize)
        {
            var paginationNode = paginationElement.ToXmlNode();
            return DownloadPaginationHelper.GetNumberOfPagesFromPagination(paginationNode, pageSize);
        }

        private SiteView ParseSiteXElement(XElement element)
        {
            try
            {
                var itemXml = element.ToXmlNode();
                var siteView = new SiteView(itemXml);

                SanityCheckView(siteView, itemXml);

                return siteView;
            }
            catch
            {
                _onlineSession.StatusLog.AddError("Error parsing project: " + element.ToString());
            }

            return null;
        }

        private void SanityCheckView(SiteView view, XmlNode xmlNode)
        {
            if (string.IsNullOrWhiteSpace(view.Id))
            {
                _onlineSession.StatusLog.AddError(view.Name + " is missing a view ID. Not returned from server! xml=" + xmlNode.OuterXml);
            }
        }
    }
}
