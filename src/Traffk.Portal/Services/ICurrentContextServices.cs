using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Services
{
    public interface ICurrentContextServices
    {
        App Application { get; }
        ApplicationUser User { get; }
        int TenantId { get; }
        Tenant Tenant { get; }
        Task<Tenant> GetTenantAsync();
    }
}