using Hangfire;
using Hangfire.Server;
using RevolutionaryStuff.Core;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.BackgroundJobServer
{
    public class TenantedJobRunner : BaseJobRunner, ITenantJobs
    {
        private readonly TraffkRdbContext DB;
        private readonly CurrentTenantServices Current;
        private readonly ITableauAdminService TableauAdminService;

        public TenantedJobRunner(TraffkRdbContext db, 
            TraffkGlobalContext globalContext,
            JobRunnerProgram jobRunnerProgram,
            CurrentTenantServices current, 
            ITableauAdminService tableauAdminService) : base(globalContext, jobRunnerProgram)
        {
            DB = db;
            Current = current;
            TableauAdminService = tableauAdminService;
        }

        void ITenantJobs.ReconfigureFiscalYears(FiscalYearSettings settings)
        {
            //var tenant = Current.Tenant;
            //var tenantSettings = tenant.TenantSettings;
            //var existingSettings = tenantSettings.FiscalYearSettings ?? new FiscalYearSettings();

            var existingSettings = Current.Tenant.TenantSettings.FiscalYearSettings ?? new FiscalYearSettings();
            if (existingSettings.FiscalYear != settings.FiscalYear ||
                existingSettings.CalendarYear != settings.CalendarYear ||
                existingSettings.CalendarMonth != settings.CalendarMonth)
            {
                //DB.FiscalYearsConfigureAsync(Current.TenantId, settings.FiscalYear, settings.CalendarYear, settings.CalendarMonth).ExecuteSynchronously();
                Current.Tenant.TenantSettings.FiscalYearSettings = settings;
                DB.SaveChanges();
                PostResult(Current.Tenant.TenantSettings.FiscalYearSettings);
            }

            Logger.Information("Completed ReconfigureFiscalYearsJob");
        }

        void ITenantJobs.CreateTableauTenant(CreateTableauTenantRequest request)
        {
            var siteInfo = TableauAdminService.CreateTableauTenant(request);
            Current.Tenant.TenantSettings.TableauTenantId = siteInfo.IdentifierForUrl;
            DB.SaveChanges();
        }

        void ITenantJobs.MigrateTableauDataset(TableauDataMigrationRequest request)
        {
            TableauAdminService.MigrateDataset(request);
        }
    }
}
