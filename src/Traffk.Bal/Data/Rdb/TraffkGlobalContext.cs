using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal.Services;

namespace Traffk.Bal.Data.Rdb
{

    public class TraffkGlobalContext : TraffkRdbContext
    {
        public TraffkGlobalContext(DbContextOptions<TraffkRdbContext> options, 
            ITraffkTenantFinder tenantFinder, 
            ConfigStringFormatter configger) : base(options, tenantFinder, configger)
        {
        }
    }
}
