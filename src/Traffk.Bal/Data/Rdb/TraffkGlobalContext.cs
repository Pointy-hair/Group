using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal.Services;

namespace Traffk.Bal.Data.Rdb
{

    public class TraffkGlobalContext : DbContext
    {
        public const string DefaultDatabaseConnectionStringName = "TraffkGlobal";

        public TraffkGlobalContext(DbContextOptions<TraffkGlobalContext> options) : base(options)
        {
        }

        public DbSet<HangfireTenantMap> HangfireTenantMappings { get; set; }
    }
}
