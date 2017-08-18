using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Services
{
    public class CurrentTenantServices : IConfigureOptions<IdentityOptions>
    {
        private readonly ITraffkTenantFinder TenantFinder;
        private readonly TraffkTenantModelDbContext Rdb;

        public CurrentTenantServices(ITraffkTenantFinder tenantFinder, TraffkTenantModelDbContext rdb)
        {
            TenantFinder = tenantFinder;
            Rdb = rdb;
        }

        private Tenant GetTenantAsync_p;
        public async Task<Tenant> GetTenantAsync()
        {
            if (GetTenantAsync_p == null)
            {
                var tenantId = await TenantFinder.GetTenantIdAsync();
                GetTenantAsync_p = await Rdb.Tenants.FindAsync(tenantId);
            }
            return GetTenantAsync_p;
        }

//        internal void Configure(IdentityOptions options) => Tenant.Configure(options);
        void IConfigureOptions<IdentityOptions>.Configure(IdentityOptions options) => Tenant.Configure(options);

        public int TenantId => TenantFinder.GetTenantIdAsync().ExecuteSynchronously();

        public Tenant Tenant => GetTenantAsync().ExecuteSynchronously();
    }
}
