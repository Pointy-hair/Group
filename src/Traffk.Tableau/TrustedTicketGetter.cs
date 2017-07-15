using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.RestRequests;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Traffk.Tableau
{
    public class TrustedTicketGetter : ITrustedTicketGetter
    {
        private readonly TableauSignInOptions TableauSignInOptions;
        private readonly ITableauUserCredentials TableauUserCredentials;

        public TrustedTicketGetter(IOptions<TableauSignInOptions> tableauSignInOptions, 
            ITableauUserCredentials tableauUserCredentials,
            ITableauTenantFinder tableauTenantFinder)
        {
            Requires.NonNull(tableauSignInOptions, nameof(tableauSignInOptions));
            Requires.NonNull(tableauUserCredentials, nameof(tableauUserCredentials));
            
            TableauSignInOptions = tableauSignInOptions.Value;
            TableauUserCredentials = tableauUserCredentials;
            var tenantId = tableauTenantFinder.GetTenantIdAsync().ExecuteSynchronously();
            if (tenantId != null)
            {
                TableauSignInOptions.UpdateForTenant(tenantId);
            }
        }

        public class TrustedTicketResult
        {
            public TrustedTicketResult(string token)
            {
                Token = token;
            }

            [JsonProperty("access_token")]
            public string Token { get; private set; }
        }

        private TrustedTicketResult Res;

        async Task<TrustedTicketResult> ITrustedTicketGetter.AuthorizeAsync()
        {
            if (Res == null)
            {
                using (var client = new HttpClient())
                {
                    var uri = new Uri(TableauSignInOptions.TrustedUrl);

                    var datas = new FormUrlEncodedContent(new[]
                    {
                        //TODO: May change this to either WebServer or tenant based
                        new KeyValuePair<string, string>("username", TableauUserCredentials.UserName)
                    });
                    var response = await client.PostAsync(uri, datas);
                    var token = await response.Content.ReadAsStringAsync();
                    Res = new TrustedTicketResult(token);
                }
            }
            return Res;
        }
    }
}
