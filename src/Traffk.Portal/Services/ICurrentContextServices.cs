using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Services
{
    public interface ICurrentContextServices
    {
        Application Application { get; }
        ApplicationUser User { get; }
        int TenantId { get; }
        Tenant Tenant { get; }
        Task<Tenant> GetTenantAsync();
    }
}