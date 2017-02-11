using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.RestRequests;
using Microsoft.Extensions.Options;

namespace Traffk.Tableau
{
    public class TableauServices : ITableauServices
    {
        private readonly ITrustedTicketGetter TrustedTicketGetter;
        private readonly TableauSignInOptions TableauSignInOptions;

        public TableauServices(ITrustedTicketGetter trustedTicketGetter, IOptions<TableauSignInOptions> tableauSignInOptions)
        {
            Requires.NonNull(trustedTicketGetter, nameof(trustedTicketGetter));
            Requires.NonNull(tableauSignInOptions, nameof(tableauSignInOptions));

            TableauSignInOptions = tableauSignInOptions.Value;
            TrustedTicketGetter = trustedTicketGetter;
        }

        public Task<string> GetTrustedTicket() => TrustedTicketGetter.GetTrustedTicket();

        [Produces("text/html")]
        public async Task<HttpContent> GetVisualization(string workbook, string view, string trustedTicket)
        {
            var httpClient = new HttpClient();

            //TODO: Change to use app settings
            var uri = new Uri($"{TableauSignInOptions.TrustedUrl}{trustedTicket}/views/{workbook}/{view}");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

            var response = await httpClient.GetAsync(uri);

            var httpContent = response.Content;
            return httpContent;
        }
    }
}
