using System.Collections.Generic;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public interface ITableauAdminService
    {
        void CreateTableauTenant(CreateTableauTenantRequest request);
        void MigrateDataset(TableauDataMigrationRequest request);
        void AddUserToSite(string siteId, string userName);
        void RemoveUserFromSite(SiteUser userToRemove);  
    }
}