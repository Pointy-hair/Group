using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Traffk.Utility
{
    public class TraffkSecretManager : IKeyVaultSecretManager
    {
        private readonly string AppNamePrefix;

        public TraffkSecretManager(string appName)
        {
            AppNamePrefix = appName + "-";
        }

        string IKeyVaultSecretManager.GetKey(SecretBundle secret)
        {
            return secret.SecretIdentifier.Name.Substring(AppNamePrefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }

        bool IKeyVaultSecretManager.Load(SecretItem secret)
        {
            return secret.Identifier.Name.StartsWith(AppNamePrefix);
        }
    }
}
