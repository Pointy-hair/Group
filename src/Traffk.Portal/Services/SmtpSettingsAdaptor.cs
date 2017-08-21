using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;

namespace TraffkPortal.Services
{
    public sealed class SmtpSettingsAdaptor : IOptions<SmtpOptions>
    {
        private readonly CurrentContextServices Current;
        private readonly IVault Vault;
        
        public SmtpSettingsAdaptor(CurrentContextServices current, IVault vault)
        {
            Current = current;
            Vault = vault;
        }

        SmtpOptions IOptions<SmtpOptions>.Value
        {
            get
            {
                if (!Current.Tenant.TenantSettings.Smtp.IsUsable())
                {
                    var smtpOptionsJson = Vault.GetSecretAsync(Traffk.Bal.Services.Vault.CommonSecretUris.SmtpOptionsUri).ExecuteSynchronously();
                    var smtpOptions = JsonConvert.DeserializeObject<SmtpOptions>(smtpOptionsJson);
                    return smtpOptions;
                }

                return Current.Tenant.TenantSettings.Smtp;
            }
        }
    }
}
