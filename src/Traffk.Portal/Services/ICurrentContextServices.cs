using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

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