using System;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Portal.Permissions
{
    public class TableauAuthorizationService : ITableauAuthorizationService
    {
        private readonly ITableauRestService AdminTableauRestService;
        private readonly TableauSignInOptions Options;
        private readonly ITableauUserCredentials TableauAdminCredentials;
        private readonly TraffkRdbContext Rdb;

        public TableauAuthorizationService(IOptions<TableauSignInOptions> options,
            IOptions<TableauAdminSignInOptions> adminSignInOptions,
            TraffkRdbContext rdb)
        {
            TableauAdminCredentials = adminSignInOptions.Value;
                //Instead of creating a new instance here, 
                //should we have a subclass of TableauRestService called AdminTableauRestService that uses TableauAdminSignInOptions?
            AdminTableauRestService = new TableauRestService(options, TableauAdminCredentials);
            Options = options.Value;
            Rdb = rdb;
        }

        public ITableauUserCredentials GetTableauUserCredentials(ApplicationUser user, Tenant tenant)
        {
            user.Settings.TableauUserName = TableauAdminCredentials.UserName;
            user.Settings.TableauPassword = TableauAdminCredentials.Password;
            Rdb.Update(user);
            Rdb.SaveChangesAsync();
            return TableauAdminCredentials;

            throw new NotImplementedException();

            if (tenant.TenantSettings.TableauTenantId != null)
            {
                Options.UpdateForTenant(tenant.TenantSettings.TableauTenantId);
                var tenantSiteSignInOptions = ConfigurationHelpers.CreateOptions(Options);
                var tenantTableauRestService = 
                    new TableauRestService(tenantSiteSignInOptions, TableauAdminCredentials);
                
                //Logic for getting an available username and assigning it as user to the specific tableau tenant
            }
        }

        public void RemoveTableauUserCredentials(ApplicationUser user, Tenant tenant)
        {
            throw new NotImplementedException();
            //Will implement in the future
        }
    }
}
