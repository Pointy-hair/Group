using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System;
using System.Threading.Tasks;

namespace TraffkPortal.Services.TenantServices
{
    public class NoTenantMiddleware
    {
        public class NoTenantMiddlewareOptions
        {
            public Uri RedirectUrl { get; set; }
        }

        private readonly IOptions<NoTenantMiddlewareOptions> Options;
        private readonly RequestDelegate Next;

        public NoTenantMiddleware(RequestDelegate next, IOptions<NoTenantMiddlewareOptions> options)
        {
            Options = options;
            Next = next;
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (TenantFinderServiceException tsex)
            {
                if (Options.Value.RedirectUrl != null)
                {
                    var u = Options.Value.RedirectUrl.AppendParameter("from", context.Request.Host.Host).AppendParameter("code", tsex.Code.ToString());
                    context.Response.Redirect(u.ToString(), false);
                    return;
                }
                throw;
            }
        }
    }
}
