using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Services
{
    public interface ICurrentUser
    {
        ApplicationUser User { get; }
    }
}
