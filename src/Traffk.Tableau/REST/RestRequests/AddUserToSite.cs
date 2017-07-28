using System.Net.Http;
using System.Text;
using System.Xml;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    public class AddUserToSite : TableauServerSignedInRequestBase
    {
        private readonly string TableauOperationDescription = "add user to site";

        private enum SiteRole
        {
            Interactor,
            Viewer
        }

        public AddUserToSite(TableauServerUrls onlineUrls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(onlineUrls, login, httpClientFactory)
        {

        }

        public void ExecuteRequest(string siteId, string userName)
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb, XmlHelper.XmlSettingsForWebRequests);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("user");
            xmlWriter.WriteAttributeString("name", userName);
            xmlWriter.WriteAttributeString("siteRole", SiteRole.Viewer.ToString());
            xmlWriter.WriteEndElement();//</user>
            xmlWriter.WriteEndElement(); // </tsRequest>
            xmlWriter.Dispose();

            var xmlText = sb.ToString();

            var urlQuery = Urls.UrlAddUserToSite(siteId);
            var httpRequest = CreateLoggedInRequest(urlQuery, HttpMethod.Post);

            SendHttpRequest(httpRequest, xmlText);
        }
    }
}
