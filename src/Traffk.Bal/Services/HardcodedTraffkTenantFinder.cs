using System.Threading.Tasks;
using RevolutionaryStuff.Core.ApplicationParts;

namespace Traffk.Bal.Services
{
    public class HardcodedTraffkTenantFinder : ITraffkTenantFinder
    {
        private readonly int TenantId;
        public HardcodedTraffkTenantFinder(int tenantId)
        {
            TenantId = tenantId;
        }

        Task<int> ITenantFinder<int>.GetTenantIdAsync()
            => Task.FromResult(TenantId);
    }
}
