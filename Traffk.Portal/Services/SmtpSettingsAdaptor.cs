using Microsoft.Extensions.Options;
using Traffk.Bal.Settings;

namespace TraffkPortal.Services
{
    public sealed class SmtpSettingsAdaptor : IOptions<SmtpOptions>
    {
        private readonly CurrentContextServices Current;

        public SmtpSettingsAdaptor(CurrentContextServices current)
        {
            Current = current;
        }

        SmtpOptions IOptions<SmtpOptions>.Value
        {
            get
            {
                return Current.Tenant.TenantSettings.Smtp;
            }
        }
    }
}
