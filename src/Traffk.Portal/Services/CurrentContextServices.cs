using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System.Linq;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Tableau.REST.RestRequests;

namespace TraffkPortal.Services
{
    public class CurrentContextServices : CurrentTenantServices, ICurrentUser, ICurrentContextServices
    {
        private readonly IHostingEnvironment Env;
        private readonly ITraffkTenantFinder TenantFinder;
        private readonly TraffkRdbContext Rdb;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly UserManager<ApplicationUser> UserManager;

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
            TraffkRdbContext rdb, 
            IHttpContextAccessor httpContextAccessor, 
            UserManager<ApplicationUser> userManager,
            IOptions<TableauSignInOptions> tableauSigninOptions
            )
            : base(tenantFinder, rdb)
        {
            TableauSignInOptions = tableauSigninOptions.Value;
            Stringer = stringer;
            TenantFinder = tenantFinder;
            Rdb = rdb;
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
        }
    }
}
