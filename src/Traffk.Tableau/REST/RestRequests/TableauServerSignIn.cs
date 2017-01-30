using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Manages the signed in session for a Tableau Server site's sign in
    /// </summary>
    public class TableauServerSignIn : TableauServerRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userName;
        private readonly string _password;
        public readonly string SiteUrlSegment;
        private string _logInCookies;
        private string _logInToken;
        private string _logInSiteId;
        private string _logInUserId;
        public readonly TaskStatusLogs StatusLog;
        private bool _isSignedIn; //True while we are signed in


        /// <summary>
        /// TRUE if we are currently signed in to a tableau server
        /// </summary>
        public bool IsSignedIn
        {
            get
            {
                return _isSignedIn;
            }
        }

        /// <summary>
        /// Sign us out
        /// </summary>
        /// <param name="serverUrls"></param>
        public void SignOut(TableauServerUrls serverUrls)
        {
            if(!_isSignedIn)
            {
                StatusLog.AddError("Session not signed in. Sign out aborted");
            }

            //Perform the sign out
            var signOut = new TableauServerSignOut(serverUrls, this);
            signOut.ExecuteRequest();

            _isSignedIn = false;
        }

        /// <summary>
        /// Synchronous call to test and make sure sign in works
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userId"></param>
        /// <param name="userPassword"></param>
        /// <param name="statusLog"></param>
        public static void VerifySignInPossible(string url, string userId, string userPassword, TaskStatusLogs statusLog)
        {
            var urlManager = TableauServerUrls.FromContentUrl(url, 1);
            var signIn = new TableauServerSignIn(urlManager, userId, userPassword, statusLog);
            bool success = signIn.ExecuteRequest();

            if(!success)
            {
                throw new Exception("Failed sign in");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="statusLog"></param>
        public TableauServerSignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null)
        {
            if (statusLog == null) { statusLog = new TaskStatusLogs(); }
            this.StatusLog = statusLog;

            _onlineUrls = onlineUrls;
            _userName = userName;
            _password = password;
            SiteUrlSegment = onlineUrls.SiteUrlSegement;
        }

        public string LogInCookies
        {
            get
            {
                return _logInCookies;
            }
        }
        public string LogInAuthToken
        {
            get
            {
                return _logInToken;
            }
        }
        public string SiteId
        {
            get
            {
                return _logInSiteId;
            }
        }
        public string UserId
        {
            get
            {
                return _logInUserId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public bool ExecuteRequest()
        {
            var webRequest = WebRequest.Create(_onlineUrls.UrlLogin);
            var sbXml = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sbXml, XmlHelper.XmlSettingsForWebRequests);
        
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("credentials"); //<credentials>
            xmlWriter.WriteAttributeString("name", _userName);
            xmlWriter.WriteAttributeString("password", _password);
            xmlWriter.WriteStartElement("site");       //<site>
            xmlWriter.WriteAttributeString("contentUrl", SiteUrlSegment);
            xmlWriter.WriteEndElement();               //</site>
            xmlWriter.WriteEndElement();              //</credentials>

            xmlWriter.WriteEndElement();  //</tsRequest>
            xmlWriter.Flush();

            string bodyText = sbXml.ToString();
            //===============================================================================================
            //Make the sign in request, trap and note, and rethrow any errors
            //===============================================================================================
            try
            {
                SendPostContents(webRequest, bodyText);
            }
            catch (Exception exSendRequest)
            {
                this.StatusLog.AddError("Error sending sign in request: " + exSendRequest.ToString());
                throw exSendRequest;
            }


            //===============================================================================================
            //Get the web response, trap and note, and rethrow any errors
            //===============================================================================================
            WebResponse response;
            try
            {
                response = webRequest.GetResponseAsync().Result;
            }
            catch(Exception exResponse)
            {
                this.StatusLog.AddError("Error returned from sign in response: " + exResponse.ToString());
                throw exResponse;
            }

            var allHeaders = response.Headers;
            var cookies = allHeaders["Set-Cookie"];
            _logInCookies = cookies; //Keep any cookies

            //===============================================================================================
            //Get the web response's XML payload, trap and note, and rethrow any errors
            //===============================================================================================
            XmlDocument xmlDoc;
            try
            {
                xmlDoc = GetWebResponseAsXml(response);
            }
            catch (Exception exSignInResponse)
            {
                this.StatusLog.AddError("Error returned from sign in xml response: " + exSignInResponse.ToString());
                throw exSignInResponse;
            }

            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");

            //var credentialNode = xmlDoc.SelectSingleNode("//iwsOnline:credentials", nsManager);
            XDocument xdoc = xmlDoc.ToXDocument();
            var credentialElement = xdoc.Root.Descendants(XName.Get("credentials", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();

            //var siteNode = xmlDoc.SelectSingleNode("//iwsOnline:site", nsManager);
            var siteElement = xdoc.Root.Descendants(XName.Get("site", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();

            _logInSiteId = siteElement.Attribute("id").Value;
            _logInToken = credentialElement.Attribute("token").Value;

            //Adding the UserId to the log-in return was a feature that was added late in the product cycle.
            //For this reason this code is going to defensively look to see if hte attribute is there
            //var userNode = xmlDoc.SelectSingleNode("user", nsManager);
            var userElement = xdoc.Root.Descendants(XName.Get("user", nsManager.LookupNamespace("iwsOnline"))).FirstOrDefault();

            string userId = null;
            if (userElement != null)
            {
                var userIdAttribute = userElement.Attribute("id");
                if (userIdAttribute != null)
                {
                    userId = userIdAttribute.Value;
                }
                _logInUserId = userId;
            }

            //Output some status text...
            if (!string.IsNullOrWhiteSpace(userId))
            {
                this.StatusLog.AddStatus("Log-in returned user id: '" + userId + "'", -10);
            }
            else
            {
                this.StatusLog.AddStatus("No User Id returned from log-in request");
                return false;  //Failed sign in
            }

            _isSignedIn = true; //Mark us as signed in
            return true; //Success
        }
    }
}
