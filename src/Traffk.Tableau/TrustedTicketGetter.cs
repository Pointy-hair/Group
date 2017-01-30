using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;

namespace Traffk.Tableau
{
    public class TrustedTicketGetter : ITrustedTicketGetter
    {
        [DataContract]
        public class TrustedTicketResult
        {
            [DataMember(Name = "access_token")]
            public string Token { get; set; }
        }

        public async Task<TrustedTicketResult> Authorize()
        {
            var client = new HttpClient();

            //TODO: Change to use app settings
            var uri = new Uri("http://traffk-dev-tab.eastus.cloudapp.azure.com/trusted/");

            var datas = new FormUrlEncodedContent(new[]
            {
                //TODO: May change this to either WebServer or tenant based
                new KeyValuePair<string, string>("username", "Test")
            });

            var response = await client.PostAsync(uri, datas);
            var token = await response.Content.ReadAsStringAsync();
            return new TrustedTicketResult
            {
                Token = token
            };
        }

        async Task<string> ITrustedTicketGetter.GetTrustedTicket()
        {
            var auth = await Authorize();
            if (auth.Token == "-1")
            {
                return null;
            }
            return auth.Token;
        }
    }
}
