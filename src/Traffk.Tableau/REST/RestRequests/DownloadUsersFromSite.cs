using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    public class DownloadUsersFromSite : TableauServerSignedInRequestBase
    {
        private readonly string TableauOperationDescription = "download users from site";
        private readonly string TableauObjectName = "user";
        private readonly HashSet<SiteUser> Users;

        public DownloadUsersFromSite(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(onlineUrls, login)
        {
            Users = new HashSet<SiteUser>();
        }

        public HashSet<SiteUser> ExecuteRequest()
        {
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                ExecuteRequestForPage(thisPage, out numberPages);
            }

            return Users;
        }

        private void ExecuteRequestForPage(int pageToRequest, out int totalNumberPages)
        {
            int pageSize = Urls.PageSize;

            //Get all users from default site
            var urlQuery = Urls.Url_UsersList(Login, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);

            //Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, TableauOperationDescription);
            var xmlDoc = GetWebResponseAsXml(response);

            var xDoc = xmlDoc.ToXDocument();
            var userElements = xDoc.Root.Descendants(XName.Get(TableauObjectName, XmlNamespace));

            //Get information for each of the data sources
            foreach (var element in userElements)
            {
                var user = new SiteUser(element.ToXmlNode());
                Users.Add(user);
            }

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------

            var paginationElement = xDoc.Root.Descendants(XName.Get("pagination", XmlNamespace)).FirstOrDefault();
            totalNumberPages = GetPageCount(paginationElement, pageSize);
        }
    }
}
