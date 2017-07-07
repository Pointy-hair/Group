using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    public class CreateSite : TableauServerSignedInRequestBase
    {
        private readonly string TableauObjectName = "site";
        private readonly string TableauOperationDescription = "add site";

        private enum AdminMode
        {
            ContentAndUsers
        }

        public CreateSite(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(onlineUrls, login)
        {

        }
        
        public SiteInfo ExecuteRequest(string tenantName)
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb, XmlHelper.XmlSettingsForWebRequests);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("site");
            xmlWriter.WriteAttributeString("name", tenantName);
            xmlWriter.WriteAttributeString("contentUrl", tenantName.ToUpperCamelCase());
            xmlWriter.WriteAttributeString("adminMode", AdminMode.ContentAndUsers.ToString());
            xmlWriter.WriteEndElement();//</site>
            xmlWriter.WriteEndElement(); // </tsRequest>
            xmlWriter.Dispose();

            var xmlText = sb.ToString();

            var urlQuery = Urls.UrlCreateSite();
            var request = CreateLoggedInRequest(urlQuery, HttpMethod.Post);

            //Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = SendHttpRequest(request, xmlText);

            var xmlDoc = GetHttpResponseAsXml(response);

            var xDoc = xmlDoc.ToXDocument();
            var siteElement = xDoc.Root.Descendants(XName.Get(TableauObjectName, XmlNamespace)).FirstOrDefault();

            return new SiteInfo(siteElement.ToXmlNode());
            
        }
    }
}
