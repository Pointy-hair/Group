using System.Net.Http;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    public class DownloadSiteInfo : TableauServerSignedInRequestBase
    {
        private readonly string TableauObjectName = "site";

        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private SiteInfo Site_p;
        public SiteInfo Site
        {
            get
            {
                return Site_p;
            }
        }
        
        public DownloadSiteInfo(TableauServerUrls onlineUrls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(onlineUrls, login, httpClientFactory)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public SiteInfo ExecuteRequest()
        {
            var logger = Login.Logger;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = Urls.Url_SiteInfo(Login);
            var webRequest = CreateLoggedInRequest(urlRequest, HttpMethod.Get);

            //Request the data from server
            Login.Logger.Information("Web request: " + urlRequest);
            var response = SendHttpRequest(webRequest);
        
            var xmlDoc = GetHttpResponseAsXml(response);

            //Get all the workbook nodes
            var xDoc = xmlDoc.ToXDocument();
            var sites = xDoc.Root.Descendants(XName.Get(TableauObjectName, XmlNamespace));

            int numberSites = 0;
            foreach(var contentXml in sites)
            {
                try
                {
                    numberSites++;
                    var site = new SiteInfo(contentXml.ToXmlNode());
                    Site_p = site;

                    Login.Logger.Information("Site info: " + site.Name + "/" + site.Id + "/" + site.State);
                }
                catch
                {
                    Login.Logger.Error("Error parsing site: " + contentXml.ToXmlNode());
                }
            }

            //Sanity check
            if(numberSites > 1)
            {
                Login.Logger.Error("Error - how did we get more than 1 site? " + numberSites.ToString() + " sites");
            }

            return Site;
        }
    }
}
