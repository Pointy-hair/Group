using System.Net.Http;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Handles sign out
    /// </summary>
    public class TableauServerSignOut : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="user"></param>
        public TableauServerSignOut(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {
            var statusLog = Login.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = _onlineUrls.UrlLogout;
            var webRequest = CreateLoggedInRequest(urlRequest, HttpMethod.Post);

            //Request the data from server
            //Login.StatusLog.AddStatus("Web request: " + urlRequest, -10);
            var response = SendHttpRequest(webRequest);

        }
    }
}
