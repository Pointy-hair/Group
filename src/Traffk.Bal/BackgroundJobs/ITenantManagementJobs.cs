using Hangfire;
using System.Threading.Tasks;

namespace Traffk.Bal.BackgroundJobs
{
    [Queue(BackgroundJobHelpers.QueueNames.TenantManagement)]
    public interface ITenantManagementJobs
    {
        void CreateTenant(TenantCreationDetails details);
        Task InitializeNewTenantAsync(TenantInitializeDetails details);
        Task AddTenantToShardManagerAsync(TenantInitializeDetails details);
    }
}
