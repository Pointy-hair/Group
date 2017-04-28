using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;

namespace Traffk.Bal.BackgroundJobs
{
    public interface IBackgroundJobEnqueuer
    {
        string ConvertFiscalYear(Tenant tenant, FiscalYearSettings fiscalYearSettings);
    }
}