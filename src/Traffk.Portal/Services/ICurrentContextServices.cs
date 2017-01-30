using System.Threading.Tasks;
using RevolutionaryStuff.PowerBiToys;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Services
{
    public interface ICurrentContextServices
    {
        PowerBiServices PowerBi { get; }
        Application Application { get; }
        ApplicationUser User { get; }
        int TenantId { get; }
        Tenant Tenant { get; }
        Task<Tenant> GetTenantAsync();
    }
}