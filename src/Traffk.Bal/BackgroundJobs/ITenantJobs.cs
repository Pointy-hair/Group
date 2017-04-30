using Traffk.Bal.Settings;

namespace Traffk.Bal.BackgroundJobs
{
    public interface ITenantJobs
    {
        void ReconfigureFiscalYears(FiscalYearSettings settings);
    }
}
