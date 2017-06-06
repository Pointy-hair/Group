using System;
using System.Threading.Tasks;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core;
using System.Net.Http;

namespace Traffk.Bal.Services
{
    /// <remarks>https://login.windows.net/traffk.onmicrosoft.com/.well-known/openid-configuration</remarks>
    public class OpenIdConfigurationFinder : IAsyncGetter<OpenIdConfiguration>
    {
        private readonly Uri OpenIdConfigurationUri;

        public OpenIdConfigurationFinder(string openIdConfigurationUrl)
        {
            Requires.Url(openIdConfigurationUrl, nameof(openIdConfigurationUrl));
            OpenIdConfigurationUri = new Uri(openIdConfigurationUrl);
        }

        async Task<OpenIdConfiguration> IAsyncGetter<OpenIdConfiguration>.GetAsync()
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(OpenIdConfigurationUri);
                return OpenIdConfiguration.CreateFromJson(json);
            }
        }
    }
}
