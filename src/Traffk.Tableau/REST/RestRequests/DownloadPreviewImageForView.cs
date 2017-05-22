using System.IO;

namespace Traffk.Tableau.REST.RestRequests
{
    public class DownloadPreviewImageForView : TableauServerSignedInRequestBase
    {
        public byte[] PreviewImage {
            get { return _previewImage; }
        }

        private readonly TableauServerUrls urls;
        private readonly string xmlNamespace;
        private byte[] _previewImage;

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
            var urlQuery = urls.UrlDownloadPreviewImageForView(Login, workbookId, viewId);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, "get preview image for view");
            var responseStream = response.GetResponseStream();

            using (MemoryStream ms = new MemoryStream())
            {
                responseStream.CopyTo(ms);
                _previewImage = ms.ToArray();
            }
        }
    }
}
