using Traffk.Bal.Settings;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Bal.BackgroundJobs
{
    public interface ITenantJobs
    {
        void ReconfigureFiscalYears(FiscalYearSettings settings);

        void CreateTableauTenant(CreateTableauTenantRequest request);

        //void MigrateTableauDataset(TableauDataMigrationRequest request);
    }
}
