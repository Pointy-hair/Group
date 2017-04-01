using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;

namespace TraffkPortal.Services.Sms
{
    public class TwilioSmsSender : ITwilioSmsSender
    {
        private readonly TwilioSmsSenderOptions Options;

        public TwilioSmsSender(IOptions<TwilioSmsSenderOptions> options)
        {
            Options = options.Value;
        }

        public async Task SendSmsAsync(string number, string message)
        {
            Requires.Text(number, nameof(number));
            Requires.Text(message, nameof(message));

            using (var client = new HttpClient { BaseAddress = new Uri(Options.BaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Options.SID}:{Options.AuthToken}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To",$"+{number}"),
                    new KeyValuePair<string, string>("From", Options.SendNumber),
                    new KeyValuePair<string, string>("Body", message)
                });

                var response = await client.PostAsync(Options.RequestUri, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var e = new Exception("Twilio Error: " + response.ReasonPhrase);
                    throw e;
                }
            }
        }
    }
}
