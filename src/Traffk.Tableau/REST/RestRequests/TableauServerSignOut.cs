using System.Net.Http;
using Traffk.Utility;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Handles sign out
    /// </summary>
    public class TableauServerSignOut : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls OnlineUrls;

        public TableauServerSignOut(TableauServerUrls onlineUrls, TableauServerSignIn login, IHttpClientFactory httpClientFactory)
            : base(login, httpClientFactory)
        {
            OnlineUrls = onlineUrls;
        }

        public void ExecuteRequest()
        {
            //var statusLog = Login.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = OnlineUrls.UrlLogout;
            var webRequest = CreateLoggedInRequest(urlRequest, HttpMethod.Post);

            //Request the data from server
            //Login.Logger.Information("Web request: " + urlRequest);
            var response = SendHttpRequest(webRequest);
        }
    }
}
