using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using RevolutionaryStuff.PowerBiToys;
using System.Linq;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;

namespace TraffkPortal.Services
{
    public class CurrentContextServices : CurrentTenantServices, ICurrentUser
    {
        private const string PowerBiUsernameKey = "PowerBiUsername";
        private const string PowerBiPasswordKey = "PowerBiPassword";

        private readonly TraffkPortalSecrets TraffkPortalSecrets;
        private readonly ITraffkTenantFinder TenantFinder;
        private readonly TraffkRdbContext Rdb;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IOptions<PowerBiWebApplicationOptions> PowerBiWebAppOptions;
        private readonly IOptions<PowerBiEndpointOptions> PowerBiEndpointOptions;

        private static readonly ICache<string, AzureActiveDirectoryResourceAuthorizationGetter.AuthenticationResult>
            BearerCache = CachingServices.Instance.CreateSynchronized<string, AzureActiveDirectoryResourceAuthorizationGetter.AuthenticationResult>(CachingServices.FlushPeriods.Medium);

        public PowerBiServices PowerBi
        {
            get
            {
                if (PowerBi_p == null)
                {
                    var app = PowerBiWebAppOptions.Value;
                    PowerBi_p = new PowerBiServices(
                        PowerBiEndpointOptions,
                        new AzureActiveDirectoryResourceAuthorizationGetter(
                            app.ClientID,
                            app.ClientSecretKey,
                            PowerBiEndpointOptions.Value.RestApiResourceUrl,
                            TraffkPortalSecrets.PowerBiUsername,
                            TraffkPortalSecrets.PowerBiPassword,
                            BearerCache)
                        );
                }
                return PowerBi_p;
            }
        }
        private PowerBiServices PowerBi_p;

        public Application Application
        {
            get
            {
                return Rdb.Applications.First(a => a.TenantId == TenantId && a.ApplicationType==Application.ApplicationTypes.Portal);
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

        public CurrentContextServices(
            TraffkPortalSecrets traffkPortalSecrets,
            ITraffkTenantFinder tenantFinder,
            TraffkRdbContext rdb, 
            IHttpContextAccessor httpContextAccessor, 
            UserManager<ApplicationUser> userManager,
            IOptions<PowerBiWebApplicationOptions> powerBiWebAppOptions, 
            IOptions<PowerBiEndpointOptions> powerBiEndpointOptions
            )
            : base(tenantFinder, rdb)
        {
            TraffkPortalSecrets = traffkPortalSecrets;
            TenantFinder = tenantFinder;
            Rdb = rdb;
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            PowerBiWebAppOptions = powerBiWebAppOptions;
            PowerBiEndpointOptions = powerBiEndpointOptions;
        }
    }
}
