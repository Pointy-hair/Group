﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Manages the signed in session for a Tableau Server site's sign in
    /// </summary>
    [Traffk.Utility.IsSerializable(false)]
    public class TableauServerSignIn : TableauServerRequestBase
    {
        private readonly TableauServerUrls TableauUrls;
        private readonly string Username;
        private readonly string Password;

        public readonly ILogger Logger;
        public string SiteUrlSegment { get; private set; }
        public string LogInAuthToken { get; private set; }
        public string SiteId { get; private set; }
        public string UserId { get; private set; }
        public IEnumerable<string> LogInCookies { get; private set; }
        public bool IsSignedIn { get; private set; }

        public void SignOut(TableauServerUrls serverUrls)
        {
            if(!IsSignedIn)
            {
                Logger.Error("Session not signed in. Sign out aborted");
            }

            var signOut = new TableauServerSignOut(serverUrls, this, HttpClientFactory);
            signOut.ExecuteRequest();

            IsSignedIn = false;
        }

        public TableauServerSignIn(TableauServerUrls tableauUrls, string username, string password, IHttpClientFactory httpClientFactory, ILogger logger)
            : base(httpClientFactory)
        {
            Logger = logger;

            TableauUrls = tableauUrls;
            Username = username;
            Password = password;
            SiteUrlSegment = tableauUrls.SiteUrlSegement;
            HttpClientFactory = httpClientFactory;
        }

        public bool ExecuteRequest()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, TableauUrls.UrlLogin);

                var sbXml = new StringBuilder();
                var xmlWriter = XmlWriter.Create(sbXml, XmlHelper.XmlSettingsForWebRequests);

                xmlWriter.WriteStartElement("tsRequest");
                xmlWriter.WriteStartElement("credentials"); //<credentials>
                xmlWriter.WriteAttributeString("name", Username);
                xmlWriter.WriteAttributeString("password", Password);
                xmlWriter.WriteStartElement("site");       //<site>
                xmlWriter.WriteAttributeString("contentUrl", SiteUrlSegment);
                xmlWriter.WriteEndElement();               //</site>
                xmlWriter.WriteEndElement();              //</credentials>

                xmlWriter.WriteEndElement();  //</tsRequest>
                xmlWriter.Flush();

                string bodyText = sbXml.ToString();

                var response = SendHttpRequest(request, bodyText);
                var allHeaders = response.Headers;

                if (response.IsSuccessStatusCode == false)
                {
                    return IsSignedIn = false;
                }

                if (response.Content.Headers.ContentLength.GetValueOrDefault() < 1)
                {
                    IEnumerable<string> contentLength;
                    response.Content.Headers.TryGetValues("Content-Length", out contentLength);

                    if (int.Parse(contentLength.First()) < 1)
                    {
                        return IsSignedIn = false;
                    }
                }

                IEnumerable<string> cookies;
                allHeaders.TryGetValues("Set-Cookie", out cookies);
                LogInCookies = cookies; //Keep any cookies

                var xmlDoc = GetHttpResponseAsXml(response);

                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");

                XDocument xdoc = xmlDoc.ToXDocument();
                var credentialElement = xdoc.Root.Descendants(XName.Get("credentials", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();
                var siteElement = xdoc.Root.Descendants(XName.Get("site", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();

                SiteId = siteElement.Attribute("id").Value;
                LogInAuthToken = credentialElement.Attribute("token").Value;
                var userElement = xdoc.Root.Descendants(XName.Get("user", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();
                string userId = null;
                if (userElement != null)
                {
                    var userIdAttribute = userElement.Attribute("id");
                    if (userIdAttribute != null)
                    {
                        userId = userIdAttribute.Value;
                    }
                    UserId = userId;
                }

                IsSignedIn = true;
                return IsSignedIn; //Success
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return IsSignedIn = false;
            }
        }
    }
}
