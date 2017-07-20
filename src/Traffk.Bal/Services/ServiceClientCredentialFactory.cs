using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Threading.Tasks;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Services
{
    public class ServiceClientCredentialFactory
    {
        private readonly IOptions<ActiveDirectoryApplicationIdentificationConfig> AppIdOptions;
        private readonly IAsyncGetter<OpenIdConfiguration> ConfigurationFinder;

        public ServiceClientCredentialFactory(IOptions<ActiveDirectoryApplicationIdentificationConfig> appIdOptions, IAsyncGetter<OpenIdConfiguration> configurationFinder)
        {
            AppIdOptions = appIdOptions;
            ConfigurationFinder = configurationFinder;
        }

        public async Task<ServiceClientCredentials> GetAsync(string resource)
        {
            var token = await GetTokenCredentialAsync(resource);
            return new TokenCredentials(token);
        }

        public async Task<string> GetTokenCredentialAsync(string resource)
        {
            var config = await ConfigurationFinder.GetAsync();
            var ac = new AuthenticationContext(config.token_endpoint);
            var appId = AppIdOptions.Value;
            var cc = new ClientCredential(appId.ApplicationId, appId.ApplicationSecret);
            var res = ac.AcquireTokenAsync(resource, cc).ExecuteSynchronously();
            return res.AccessToken;
        }
    }
}
