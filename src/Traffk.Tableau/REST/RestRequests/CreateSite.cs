﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RevolutionaryStuff.Core;

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
        
        public SiteinfoSite ExecuteRequest(string tenantName)
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
            var webRequest = CreateLoggedInWebRequest(urlQuery, "POST");
            TableauServerRequestBase.SendPostContents(webRequest, xmlText);

            Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, TableauOperationDescription);

            using (response)
            {
                var xmlDoc = GetWebResponseAsXml(response);

                //if (!xmlDoc.ToString().Contains("Created"))
                //{
                //    throw new Exception("Site not created.");
                //}

                var xDoc = xmlDoc.ToXDocument();
                var siteElement = xDoc.Root.Descendants(XName.Get(TableauObjectName, XmlNamespace)).FirstOrDefault();

                return new SiteinfoSite(siteElement.ToXmlNode());
            }




        }
    }
}
