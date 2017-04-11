using Traffk.Bal.Data.Rdb;
using Traffk.Tableau;

namespace Traffk.Portal.Permissions
{
    public interface ITableauAuthorizationService
    {
        ITableauUserCredentials GetTableauUserCredentials(ApplicationUser user, Tenant tenant);
        void RemoveTableauUserCredentials(ApplicationUser user, Tenant tenant);
    }
}