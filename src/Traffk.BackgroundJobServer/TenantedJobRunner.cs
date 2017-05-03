using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;

namespace Traffk.BackgroundJobRunner
{
    public class TenantedJobRunner : ITenantJobs
    {
        private readonly TraffkRdbContext DB;
        private readonly CurrentTenantServices Current;

        public TenantedJobRunner(TraffkRdbContext db, CurrentTenantServices current)
        {
            DB = db;
            Current = current;
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
    }
}
