using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
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
            catch (Exception e)
            {
                var tenantFinderServiceException =
                    e.InnerExceptions().FirstOrDefault(x => x.GetType() == typeof(TenantFinderServiceException)) as TenantFinderServiceException;

                if (tenantFinderServiceException == null) throw;

                if (Options.Value.RedirectUrl != null)
                {
                    var u = Options.Value.RedirectUrl.AppendParameter("from", context.Request.Host.Host).AppendParameter("code", tenantFinderServiceException.Code.ToString());
                    context.Response.Redirect(u.ToString(), false);
                    return;
                }

                throw;
            }
        }
    }
}
