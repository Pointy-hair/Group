using Microsoft.Extensions.Options;
using System;
using Serilog;
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
            IHttpClientFactory httpClientFactory,
            ILogger logger)
        {
            TableauAdminCredentials = adminSignInOptions.Value;
            AdminTableauRestService = new TableauViewerService(options, TableauAdminCredentials, httpClientFactory, logger);
            Options = options.Value;
            Rdb = rdb;
        }

        public ITableauUserCredentials GetTableauUserCredentials(ApplicationUser user, string tableauTenantId)
        {
            if (user.UserSettings.TableauUserName != null && user.UserSettings.TableauPassword != null)
            {
                return new TableauUserCredentials(user.UserSettings.TableauUserName, user.UserSettings.TableauPassword);
            }
            else
            {
                user.UserSettings.TableauUserName = TableauAdminCredentials.UserName;
                user.UserSettings.TableauPassword = TableauAdminCredentials.Password;
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
