using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Traffk.Tableau.REST.RestRequests
{
    public class GetUnderlyingData : TableauServerSignedInRequestBase
    {
        private readonly TableauSignInOptions options;
        private ITrustedTicketGetter trustedTicketGetter;
        private string token;
        private string sessionId;
        private CookieCollection requiredCookies = new CookieCollection();

        public GetUnderlyingData(TableauSignInOptions options, TableauServerSignIn login) : base(login)
        {
            this.options = options;
            trustedTicketGetter = new TrustedTicketGetter(new OptionsWrapper<TableauSignInOptions>(options));
        }

        private void GetTrustedTicket()
        {
            var ticket = trustedTicketGetter.Authorize().Result;
            token = ticket.Token;
        }

        //private void SetCookies(string workbookName, string viewName)
        //{
        //    var cookieContainer = new CookieContainer();
        //    HttpClientHandler handler = new HttpClientHandler();
        //    handler.AllowAutoRedirect = false;
        //    handler.CookieContainer = cookieContainer;


        //    using (var httpClient = new HttpClient(handler))
        //    {
        //        var uri = new Uri($"{options.Url}/trusted/{token}/views/{workbookName}/{viewName}?:size=1610,31&:embed=y&:showVizHome=n&:jsdebug=y&:bootstrapWhenNotified=y&:tabs=n&:apiID=host0");

        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", $"{options.Host}");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "Keep-Alive");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", $"{options.Url}");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch, br");
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

        //        var response = httpClient.GetAsync(uri).Result;

        //        var responseCookies = cookieContainer.GetCookies(uri).Cast<Cookie>();
        //        foreach (var responseCookie in responseCookies)
        //        {
        //            requiredCookies.Add(responseCookie);
        //        }
        //    }
        //}

        private string GetSessionId(string workbookName, string viewName)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookieContainer;


            using (var httpClient = new HttpClient(handler))
            {
                var uri = new Uri($"{options.Url}/trusted/{token}/views/{workbookName}/{viewName}?:size=1610,31&:embed=y&:showVizHome=n&:jsdebug=y&:bootstrapWhenNotified=y&:tabs=n&:apiID=host0");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", $"{options.Host}");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "Keep-Alive");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", $"{options.Url}");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch, br");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

                var response = httpClient.GetAsync(uri).Result;
                var headers = response.Headers;

                var responseCookies = cookieContainer.GetCookies(uri).Cast<Cookie>();
                foreach (var responseCookie in responseCookies)
                {
                    requiredCookies.Add(responseCookie);
                }

                IEnumerable<string> values;
                if (headers.TryGetValues("X-Session-Id", out values))
                {
                    sessionId = values.First();
                    return sessionId;
                }
            }

            return null;
        }

        public void ExecuteRequest(string workbookName, string viewName)
        {
            //GetTrustedTicket();

            //SetCookies(workbookName, viewName);

            GetTrustedTicket();

            var session = GetSessionId(workbookName, viewName);
            var uri = new Uri($"{options.Url}/vizql/vud/sessions/{session}/views/1756194915096830534_2074546099886777711?csv=true&showall=true");

            //var uri = new Uri($"{options.Url}/vizql/vud/sessions/0DB8DD53E08C43CB811E1DB1FA810D74-0:0/views/1756194915096830534_2074546099886777711?csv=true&showall=true");


            var cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookieContainer;

            using (var httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", $"{options.Host}");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "Keep-Alive");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15031");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", $"{options.Url}/vizql/w/{viewName}/viewData/sessions//views/1756194915096830534_2074546099886777711?maxrows=200");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch, br");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");
                //cookieContainer.Add(uri, requiredCookies);
                cookieContainer.Add(uri, new Cookie("workgroup_session_id", requiredCookies["workgroup_session_id"].Value));
                cookieContainer.Add(uri, new Cookie("XSRF-TOKEN", requiredCookies["XSRF-TOKEN"].Value));
                cookieContainer.Add(uri, new Cookie("tableau_locale", requiredCookies["tableau_locale"].Value));

                //cookieContainer.Add(uri, new Cookie("workgroup_session_id", "Njs29YuUbpyqzA8Ss0VA4qju8vhGCIyU"));
                //cookieContainer.Add(uri, new Cookie("XSRF-TOKEN", "JaxVzGrOUQRTbcK1PR7xGDE7v0gbld5y"));
                //cookieContainer.Add(uri, new Cookie("tableau_locale", "en"));

                var response = httpClient.GetAsync(uri).Result;

                var response2 = httpClient.GetAsync(uri).Result;

                StreamReader sr = new StreamReader(response.Content.ReadAsStreamAsync().Result);
                string results = sr.ReadToEnd();
            }
        }
    }
}
