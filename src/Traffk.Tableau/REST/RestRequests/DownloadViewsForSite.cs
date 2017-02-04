using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    public class DownloadViewsForSite : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls urls;
        private readonly string xmlNamespace;

        public IEnumerable<SiteView> Views;

        public DownloadViewsForSite(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            urls = onlineUrls;
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
            xmlNamespace = nsManager.LookupNamespace("iwsOnline");
        }

        public void ExecuteRequest()
        {
            var views = new List<SiteView>();

            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    ExecuteRequestForPage(views, thisPage, out numberPages);
                }
                catch (Exception e)
                {
                    //TODO: Add logging
                }
            }

            Views = views;
        }

        private void ExecuteRequestForPage(List<SiteView> views, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = urls.PageSize;
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = urls.UrlDownloadViewsForSite(onlineSession, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            onlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, "get views list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the view nodes
            var xDoc = xmlDoc.ToXDocument();
            var viewElements = xDoc.Root.Descendants(XName.Get("view", xmlNamespace));

            //Get information for each of the data sources
            foreach (var element in viewElements)
            {
                var view = ParseSiteXElement(element);
                views.Add(view);
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------

            var paginationElement = xDoc.Root.Descendants(XName.Get("pagination", xmlNamespace)).FirstOrDefault();
            totalNumberPages = GetPageCount(paginationElement, pageSize);
        }

        private SiteView ParseSiteXElement(XElement element)
        {
            try
            {
                var itemXml = element.ToXmlNode();
                var siteView = new SiteView(itemXml, xmlNamespace);
                SanityCheckView(siteView, itemXml);

                return siteView;
            }
            catch
            {
                onlineSession.StatusLog.AddError("Error parsing project: " + element.ToString());
            }

            return null;
        }

        private void SanityCheckView(SiteView view, XmlNode xmlNode)
        {
            if (string.IsNullOrWhiteSpace(view.Id))
            {
                onlineSession.StatusLog.AddError(view.Name + " is missing a view ID. Not returned from server! xml=" + xmlNode.OuterXml);
            }
        }
    }
}
