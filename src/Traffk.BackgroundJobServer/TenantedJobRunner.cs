﻿using RevolutionaryStuff.Core;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.BackgroundJobServer
{
    public class TenantedJobRunner : ITenantJobs
    {
        private readonly TraffkRdbContext DB;
        private readonly CurrentTenantServices Current;
        private readonly ITableauAdminService TableauAdminService;

        public TenantedJobRunner(TraffkRdbContext db, CurrentTenantServices current, ITableauAdminService tableauAdminService)
        {
            DB = db;
            Current = current;
            TableauAdminService = tableauAdminService;
        }

        void ITenantJobs.ReconfigureFiscalYears(FiscalYearSettings settings)
        {
            var existingSettings = Current.Tenant.TenantSettings.FiscalYearSettings ?? new FiscalYearSettings();
            if (existingSettings.FiscalYear != settings.FiscalYear ||
                existingSettings.CalendarYear != settings.CalendarYear ||
                existingSettings.CalendarMonth != settings.CalendarMonth)
            {
                DB.FiscalYearsConfigureAsync(Current.TenantId, settings.FiscalYear, settings.CalendarYear, settings.CalendarMonth).ExecuteSynchronously();
                Current.Tenant.TenantSettings.FiscalYearSettings = settings;
                DB.SaveChanges();
            }
        }

        void ITenantJobs.CreateTableauTenant(CreateTableauTenantRequest request)
        {
            var siteInfo = TableauAdminService.CreateTableauTenant(request);
            Current.Tenant.TenantSettings.TableauTenantId = siteInfo.IdentifierForUrl;
            DB.SaveChanges();
        }
    }
}
