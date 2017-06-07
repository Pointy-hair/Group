using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Tableau;

namespace Traffk.Portal.Permissions
{
    public interface ITableauAuthorizationService
    {
        ITableauUserCredentials GetTableauUserCredentials(ApplicationUser user, string tableauTenantId);
        void RemoveTableauUserCredentials(ApplicationUser user, string tableauTenantId);
    }
}