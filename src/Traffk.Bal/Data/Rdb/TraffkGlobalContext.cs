using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal.Services;
using System.Threading.Tasks;
using System.Threading;
using RevolutionaryStuff.Core;

namespace Traffk.Bal.Data.Rdb
{

    public partial class TraffkGlobalsContext : DbContext
    {
        public const string DefaultDatabaseConnectionStringName = "TraffkGlobal";

        public TraffkGlobalsContext(DbContextOptions<TraffkGlobalsContext> options) : base(options)
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
