using System.Net.Http;
using Traffk.Tableau.REST.Models;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    public class RemoveUserFromSite : TableauServerSignedInRequestBase
    {
        private readonly string TableauOperationDescription = "delete user from site";

        public RemoveUserFromSite(TableauServerUrls onlineUrls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(onlineUrls, login, httpClientFactory)
        {

        }

        public void ExecuteRequest(SiteUser userToRemove)
        {
            var urlQuery = Urls.UrlRemoveUserFromSite(Login, userToRemove);
            var webRequest = CreateLoggedInRequest(urlQuery, HttpMethod.Delete);

            var response = SendHttpRequest(webRequest, TableauOperationDescription);
        }

    }
}
