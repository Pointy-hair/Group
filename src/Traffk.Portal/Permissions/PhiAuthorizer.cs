using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RevolutionaryStuff.Core;
using Traffk.Bal.Permissions;
using TraffkPortal.Permissions;

namespace Traffk.Portal.Permissions
{
    //This is in Traffk.Portal because if we place in Traffk.Bal, we won't have access to GetCanAccessProtectedHealthInformationAsync method
    public class PhiAuthorizer : IPhiAuthorizer
    {
        private readonly IAuthorizationService AuthorizationService;
        private readonly IHttpContextAccessor HttpContextAccessor;
        
        public PhiAuthorizer(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            AuthorizationService = authorizationService;
            HttpContextAccessor = httpContextAccessor;
        }
        bool IPhiAuthorizer.CanSeePhi => HttpContextAccessor.HttpContext.User.GetCanAccessProtectedHealthInformationAsync(AuthorizationService).ExecuteSynchronously();
    }
}
