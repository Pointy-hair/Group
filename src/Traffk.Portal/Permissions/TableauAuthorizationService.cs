using Microsoft.Extensions.Options;
using System;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Utility;

namespace Traffk.Portal.Permissions
{
    public class TableauAuthorizationService : ITableauAuthorizationService
    {
        private readonly ITableauViewerService AdminTableauRestService;
        private readonly TableauSignInOptions Options;
        private readonly ITableauUserCredentials TableauAdminCredentials;
        private readonly TraffkTenantModelDbContext Rdb;

        public TableauAuthorizationService(IOptions<TableauSignInOptions> options,
            IOptions<TableauAdminCredentials> adminSignInOptions,
            TraffkTenantModelDbContext rdb,
            IHttpClientFactory httpClientFactory)
        {
            TableauAdminCredentials = adminSignInOptions.Value;
                //Instead of creating a new instance here, 
                //should we have a subclass of TableauRestService called AdminTableauRestService that uses TableauAdminSignInOptions?
            AdminTableauRestService = new TableauViewerService(options, TableauAdminCredentials, httpClientFactory);
            Options = options.Value;
            Rdb = rdb;
        }

        public ITableauUserCredentials GetTableauUserCredentials(ApplicationUser user, string tableauTenantId)
        {
            if (user.Settings.TableauUserName != null && user.Settings.TableauPassword != null)
            {
                return new TableauUserCredentials(user.Settings.TableauUserName, user.Settings.TableauPassword);
            }
            else
            {
                user.Settings.TableauUserName = TableauAdminCredentials.UserName;
                user.Settings.TableauPassword = TableauAdminCredentials.Password;
                Rdb.Update(user);
                Rdb.SaveChangesAsync();
                return TableauAdminCredentials;
            }
        }

        public void RemoveTableauUserCredentials(ApplicationUser user, string tableauTenantId)
        {
            throw new NotImplementedException();
            //Will implement in the future
        }
    }
}
