using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using RevolutionaryStuff.Core;

namespace Traffk.Bal.Data.Rdb.TraffkGlobal
{

    public partial class TraffkGlobalDbContext : DbContext
    {
        public const string DefaultDatabaseConnectionStringName = "TraffkGlobal";

        public TraffkGlobalDbContext(DbContextOptions<TraffkGlobalDbContext> options) : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) => SaveChangesAsync().ExecuteSynchronously();

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.PreSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
