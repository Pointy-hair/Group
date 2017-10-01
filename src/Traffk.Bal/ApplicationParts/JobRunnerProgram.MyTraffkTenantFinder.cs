using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Bal.Services;

namespace Traffk.Bal.ApplicationParts
{
    public partial class JobRunnerProgram
    {
        public class MyTraffkTenantFinder : ITraffkTenantFinder, IEnumerable<KeyValuePair<string, object>>
        {
            private readonly TraffkTenantShardsDbContext DB;
            internal int? TenantId
            {
                set { TenantId_p = value; }
                get => TenantId_p.GetValueOrDefault(TraffkHelpers.TraffkGlobalsTenantId);
            }
            private int? TenantId_p;

            public MyTraffkTenantFinder(TraffkTenantShardsDbContext db)
            {
                DB = db;
            }

            private string GetTenantDatabaseName(int tenantId)
            {
                return DB.TenantFindByTenantId(tenantId).Result.FirstOrDefault().HostDatabaseName;
            }

            Task<int> ITenantFinder<int>.GetTenantIdAsync()
                => Task.FromResult(TenantId.Value);

            IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
            {
                var tenantId = TenantId.Value;
                var databaseName = GetTenantDatabaseName(tenantId);
                yield return new KeyValuePair<string, object>(ConfigStringFormatter.CommonTerms.DatabaseName, databaseName);
            }

            IEnumerator IEnumerable.GetEnumerator()
                => CollectionHelpers.GetEnumerator(this);
        }
    }
}
