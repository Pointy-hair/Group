using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;

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

        public AddUserToSite(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(onlineUrls, login)
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
            var webRequest = CreateLoggedInWebRequest(urlQuery, "POST");
            TableauServerRequestBase.SendPostContents(webRequest, xmlText);

            var response = GetWebReponseLogErrors(webRequest, TableauOperationDescription);
        }
    }
}
