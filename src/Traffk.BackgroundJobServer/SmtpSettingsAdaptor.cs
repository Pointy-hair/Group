﻿using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System.Linq;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;

namespace Traffk.BackgroundJobRunner
{
    public sealed class SmtpSettingsAdaptor : IOptions<SmtpOptions>
    {
        private readonly ITraffkTenantFinder Finder;
        private readonly TraffkRdbContext Rdb;

        public SmtpSettingsAdaptor(ITraffkTenantFinder finder, TraffkRdbContext rdb)
        {
            Finder = finder;
            Rdb = rdb;
        }

        SmtpOptions IOptions<SmtpOptions>.Value
        {
            get
            {
                var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
                var tenant = Rdb.Tenants.Single(z => z.TenantId == tenantId);
                return tenant.TenantSettings.Smtp;
            }
        }
    }
}