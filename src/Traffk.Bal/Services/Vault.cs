using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Threading.Tasks;
using Traffk.Bal.Settings;
using Traffk.Utility;

namespace Traffk.Bal.Services
{
    public class Vault : IVault
    {
        public static class CommonSecretUris
        {
            public const string TableauAdminCredentialsUri = "https://traffkkeyvault.vault.azure.net/secrets/TableauAdminCredentials/1080b68139254ad5b212b749f412a030";
            public const string TraffkFtpTodayCredentialsUri = "https://traffkkeyvault.vault.azure.net/secrets/TraffkFtpTodayCredentials/84955de3da07498cb3a49f112a4c4232";
            public const string TraffkPgpPublicKeyUri = "https://traffkkeyvault.vault.azure.net/secrets/TraffkPgpPublicKey/4f3bf633217140a9a5f7c894accbf54e";
            public const string TraffkPgpPrivateKeyUri = "https://traffkkeyvault.vault.azure.net/secrets/TraffkPgpPrivateKey/c63ddaf975524026b68216000683808d";
            public const string TraffkPgpPrivateKeyPasswordUri = "https://traffkkeyvault.vault.azure.net/secrets/TraffkPgpPrivateKeyPassword/c163ad8eac1d45abb9cc6882c2ddb19d";
            public const string ZipCodesComCredentialsUri = "https://traffkkeyvault.vault.azure.net/secrets/ZipCodesComCredentials/dc3c608040a34ab5907c488898c9e32a";
            public const string SmtpOptionsUri = "https://traffkkeyvault.vault.azure.net/secrets/Portal-SmtpOptions/b2e785d9fd0e4ccc997982b6afcc11d6";
        }

        private readonly IOptions<ActiveDirectoryApplicationIdentificationConfig> AppConfig;
        private readonly ICacher Cacher;
        private readonly TimeSpan CacheTimeout = TimeSpan.FromMinutes(5);
        private readonly IHttpClientFactory HttpClientFactory;

        public Vault(IOptions<ActiveDirectoryApplicationIdentificationConfig> appConfigOptions, ICacher cacher, IHttpClientFactory httpClientFactory)
        {
            AppConfig = appConfigOptions;
            Cacher = cacher;
            HttpClientFactory = httpClientFactory;
        }

        private Task<string> KeyVaultClientAuthenticationCallbackAsync(string authority, string resource, string scope)
            => Cacher.FindOrCreateValWithSimpleKeyAsync(
                Cache.CreateKey(typeof(Vault), nameof(KeyVaultClientAuthenticationCallbackAsync), authority, resource, scope),
                async () =>
                {
                    var ac = new AuthenticationContext(authority);
                    var appId = AppConfig.Value;
                    var cc = new ClientCredential(appId.ApplicationId, appId.ApplicationSecret);
                    var res = await ac.AcquireTokenAsync(resource, cc);
                    return res.AccessToken;
                }, CacheTimeout);

        private Task<SecretBundle> GetSecretBundleAsync(string uri)
            => Cacher.FindOrCreateValWithSimpleKeyAsync(
                Cache.CreateKey(typeof(Vault), nameof(GetSecretBundleAsync), uri),
                async () =>
                {
                    var kv = new Microsoft.Azure.KeyVault.KeyVaultClient(KeyVaultClientAuthenticationCallbackAsync, HttpClientFactory.Create());
                    var u = new Uri(uri);
                    var parts = u.LocalPath.Split('/', '\\');
                    var vaultBaseUrl = u.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
                    var secretName = parts[parts.Length - 2];
                    var secretVersion = parts[parts.Length - 1];
                    var ret = await kv.GetSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, secretVersion);
                    return ret.Body;
                }, CacheTimeout);

        async Task<string> IVault.GetSecretAsync(string uri)
            => (await GetSecretBundleAsync(uri)).Value;

        async Task<ICredentials> IVault.GetCredentialsAsync(string credentialsKey)
        {
            var sb = await GetSecretBundleAsync(credentialsKey);
            return Credentials.CreateFromJson(sb.Value);
        }

        public class Credentials : ICredentials
        {
            public static Credentials CreateFromJson(string json)
            {
                return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<Credentials>(json);
            }

            public string ToJson() => JsonConvert.SerializeObject(this);

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }
    }
}
