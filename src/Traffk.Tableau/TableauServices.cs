using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core;

namespace Traffk.Tableau
{
    public class TableauServices : ITableauServices
    {
        private readonly ITrustedTicketGetter TrustedTicketGetter;

        public TableauServices(ITrustedTicketGetter trustedTicketGetter)
        {
            Requires.NonNull(trustedTicketGetter, nameof(trustedTicketGetter));

            TrustedTicketGetter = trustedTicketGetter;
        }

        public Task<string> GetTrustedTicket() => TrustedTicketGetter.GetTrustedTicket();

        [Produces("text/html")]
        public async Task<HttpContent> GetVisualization(string workbook, string view, string trustedTicket)
        {
            var httpClient = new HttpClient();

            //TODO: Change to use app settings
            var uri = new Uri($"http://traffk-dev-tab.eastus.cloudapp.azure.com/trusted/{trustedTicket}/views/{workbook}/{view}");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

            var response = await httpClient.GetAsync(uri);

            var httpContent = response.Content;
            return httpContent;
        }
    }
}
