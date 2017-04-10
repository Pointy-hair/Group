using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    public class RemoveUserFromSite : TableauServerSignedInRequestBase
    {
        private readonly string TableauOperationDescription = "delete user from site";

        public RemoveUserFromSite(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(onlineUrls, login)
        {

        }

        public void ExecuteRequest(SiteUser userToRemove)
        {
            var urlQuery = Urls.UrlRemoveUserFromSite(Login, userToRemove);
            var webRequest = CreateLoggedInWebRequest(urlQuery, "DELETE");

            var response = GetWebReponseLogErrors(webRequest, TableauOperationDescription);
        }

    }
}
