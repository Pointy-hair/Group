using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Services
{
    public interface ICurrentUser
    {
        ApplicationUser User { get; }
    }
}
