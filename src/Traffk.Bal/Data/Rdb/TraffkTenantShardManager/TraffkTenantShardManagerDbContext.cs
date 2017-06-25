using Microsoft.EntityFrameworkCore;

namespace Traffk.Bal.Data.Rdb.TraffkTenantShardManager
{
    public partial class TraffkTenantShardManagerDbContext : DbContext
    {
        public const string DefaultDatabaseConnectionStringName = "TraffkTenantShardManager";

        public TraffkTenantShardManagerDbContext(DbContextOptions<TraffkTenantShardManagerDbContext> options)
            : base(options)
        { }
    }
}
