using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System;
using TraffkPortal.Services.TenantServices;

namespace TraffkPortal.Services
{
    public sealed class PreferredHostnameFilter : IAuthorizationFilter
    {
        public class Config
        {
            public const string ConfigSectionName = "PreferredHostnameFilterOptions";

            public bool RedirectToPreferredHostname { get; set; } = true;
        }

        private readonly TenantFinderService TenantFinder;
        private readonly IOptions<Config> Options;

        public PreferredHostnameFilter(TenantFinderService tenantFinder, IOptions<Config> options)
        {
            TenantFinder = tenantFinder;
            Options = options;
        }

        void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
        {
            Requires.NonNull(context, nameof(context));

            if (TenantFinder.ActualHostname != TenantFinder.PreferredHostname && Options.Value.RedirectToPreferredHostname)
            {
                HandleNotIdealHostname(context);
            }
        }

        private void HandleNotIdealHostname(AuthorizationFilterContext context)
        {
            // only redirect for GET requests, otherwise the browser might not propagate the verb and request
            // body correctly.
            if (!string.Equals(context.HttpContext.Request.Method, WebHelpers.Methods.Get, StringComparison.OrdinalIgnoreCase))
            {
                //do nothing, being on the wrong host wont kill the user for a request or two
            }
            else
            {
                var request = context.HttpContext.Request;

                var host = request.Host;
                if (host.Port.HasValue)
                {
                    host = new HostString(TenantFinder.PreferredHostname, host.Port.Value);
                }
                else
                {
                    host = new HostString(TenantFinder.PreferredHostname);
                }

                var newUrl = string.Concat(
                    context.HttpContext.Request.Scheme,
                    "://",
                    host.ToUriComponent(),
                    request.PathBase.ToUriComponent(),
                    request.Path.ToUriComponent(),
                    request.QueryString.ToUriComponent());

                // redirect to HTTPS version of page
                context.Result = new RedirectResult(newUrl, true);
            }
        }

    }
}
