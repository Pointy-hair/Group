using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System.Linq;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Services;
using Traffk.Portal.Permissions;
using Traffk.Tableau;
using Traffk.Tableau.REST.RestRequests;

namespace TraffkPortal.Services
{
    public class CurrentContextServices : CurrentTenantServices, ICurrentUser, ICurrentContextServices, ITableauUserCredentials
    {
        private readonly ITraffkTenantFinder TenantFinder;
        private readonly TraffkTenantModelDbContext Rdb;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ITableauAuthorizationService TableauAuthorizationService;

        private static readonly ICache<string, AzureActiveDirectoryResourceAuthorizationGetter.AuthenticationResult>
            BearerCache = CachingServices.Instance.CreateSynchronized<string, AzureActiveDirectoryResourceAuthorizationGetter.AuthenticationResult>(CachingServices.FlushPeriods.Medium);

        public App Application
        {
            get
            {
                return Rdb.Apps.First(a => 
                a.TenantId == TenantId && 
                a.AppType==AppTypes.Portal);
            }
        }

        public ApplicationUser User
        {
            get
            {
                var principal = HttpContextAccessor.HttpContext.User;
                if (principal != null && principal.Identity != null && principal.Identity.Name!=null)
                {
                    return UserManager.FindByNameAsync(principal.Identity.Name).ExecuteSynchronously();
                }
                return null;
            }
        }

        private readonly ConfigStringFormatter Stringer;

        public TableauSignInOptions TableauSignInOptions { get; private set; }

        public CurrentContextServices(
            ConfigStringFormatter stringer,
            ITraffkTenantFinder tenantFinder,
            TraffkTenantModelDbContext rdb, 
            IHttpContextAccessor httpContextAccessor, 
            UserManager<ApplicationUser> userManager,
            IOptions<TableauSignInOptions> tableauSigninOptions,
            ITableauAuthorizationService tableauAuthorizationService
            )
            : base(tenantFinder, rdb)
        {
            TableauSignInOptions = tableauSigninOptions.Value;
            Stringer = stringer;
            TenantFinder = tenantFinder;
            Rdb = rdb;
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            TableauAuthorizationService = tableauAuthorizationService;
        }

        string ITableauUserCredentials.UserName => TableauAuthorizationService.GetTableauUserCredentials(User, Tenant.TenantSettings.TableauTenantId).UserName;

        string ITableauUserCredentials.Password => TableauAuthorizationService.GetTableauUserCredentials(User, Tenant.TenantSettings.TableauTenantId).Password;

    }
}
