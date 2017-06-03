using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Traffk.Bal.Permissions;
using Traffk.Portal.Permissions;

namespace TraffkPortal.Permissions
{
    public static class PortalPermissionHelpers
    {
        public static class ExplicitPolicyNames
        {
            public static readonly string ProtectedHealthInformation = CreatePolicyName("ProtectedHealthInformation");
        }

        public static Task<bool> HasPermission(this ClaimsPrincipal principal, IAuthorizationService auth, PermissionNames permission)
        {
            return auth.AuthorizeAsync(principal, CreatePolicyName(permission));
        }

        public static Task<bool> HasPermission(this ClaimsPrincipal principal, IAuthorizationService auth, ApiNames api)
        {
            return auth.AuthorizeAsync(principal, CreatePolicyName(api));
        }

        public static Task<bool> GetCanAccessProtectedHealthInformationAsync(this ClaimsPrincipal principal, IAuthorizationService auth)
        {
            return auth.AuthorizeAsync(principal, ExplicitPolicyNames.ProtectedHealthInformation);
        }

        public static string CreatePolicyName(PermissionNames permission)
        {
            return $"{nameof(PermissionAuthorizeAttribute)}.{permission}";
        }

        public static string CreatePolicyName(ApiNames api)
        {
            return $"{nameof(PermissionAuthorizeAttribute)}.{api}";
        }

        public static string CreatePolicyName(string name)
        {
            return $"TraffkPolicy.{name}";
        }

        public static void AddPermissions(this IServiceCollection services)
        {
            services.AddAuthorization(o =>
            {
                foreach (PermissionNames p in Enum.GetValues(typeof(PermissionNames)))
                {
                    o.AddPolicy(CreatePolicyName(p), policy => policy.AddRequirements(new PermissionRequirement(p)));
                }
                o.AddPolicy(ExplicitPolicyNames.ProtectedHealthInformation, policy => policy.AddRequirements(new ProtectedHealthInformationRequirement()));
            });
            services.AddTransient<IAuthorizationHandler, PermissionHandler>();
            services.AddTransient<IAuthorizationHandler, ProtectedHealthInformationHandler>();
            
        }

        public static void AddApis(this IServiceCollection services)
        {
            services.AddAuthorization(o =>
            {
                foreach (ApiNames a in Enum.GetValues(typeof(ApiNames)))
                {
                    o.AddPolicy(CreatePolicyName(a), policy => policy.AddRequirements(new ApiRequirement(a)));
                }
            });

            services.AddTransient<IAuthorizationHandler, ApiHandler>();
        }
    }
}
