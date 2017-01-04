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

        public async Task<HttpContent> GetVisualization(string workbook, string view, string trustedTicket)
        {
            var client = new HttpClient();

            //TODO: Change to use app settings
            var uri = new Uri($"http://traffk-dev-tab.eastus.cloudapp.azure.com/trusted/{trustedTicket}/views/{workbook}/{view}");
            var response = await client.GetAsync(uri);
            var httpContent = response.Content;
            return httpContent;
        }
    }
}
