using System.Xml.Linq;
using Traffk.Tableau.REST.Models;

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="user"></param>
        public DownloadSiteInfo(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(onlineUrls, login)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public SiteInfo ExecuteRequest()
        {
            var statusLog = Login.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = Urls.Url_SiteInfo(Login);
            var webRequest = CreateLoggedInWebRequest(urlRequest);
            webRequest.Method = "GET";

            //Request the data from server
            Login.StatusLog.AddStatus("Web request: " + urlRequest, -10);
            var response = GetWebReponseLogErrors(webRequest, "get site info");
        
            var xmlDoc = GetWebResponseAsXml(response);

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

                    statusLog.AddStatus("Site info: " + site.Name + "/" + site.Id + "/" + site.State);
                }
                catch
                {
                    statusLog.AddError("Error parsing site: " + contentXml.ToXmlNode());
                }
            }

            //Sanity check
            if(numberSites > 1)
            {
                statusLog.AddError("Error - how did we get more than 1 site? " + numberSites.ToString() + " sites");
            }

            return Site;
        }
    }
}
