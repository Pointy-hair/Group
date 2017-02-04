using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Tableau.REST.RestRequests
{
    public class DownloadPreviewImageForView : TableauServerSignedInRequestBase
    {
        public string ImageUrl {
            get { return imageUrl; }
        }

        private readonly TableauServerUrls urls;
        private readonly string xmlNamespace;
        private string imageUrl;

        public DownloadPreviewImageForView(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            urls = onlineUrls;
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
            xmlNamespace = nsManager.LookupNamespace("iwsOnline");
        }

        public void ExecuteRequest(string workbookId, string viewId)
        {
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = urls.UrlDownloadPreviewImageForView(onlineSession, workbookId, viewId);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            onlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, "get preview image for view");
            var xmlDoc = GetWebResponseAsXml(response);

            imageUrl = xmlDoc.ToString();
        }
    }
}
