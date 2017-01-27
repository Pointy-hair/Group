using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TraffkPortal.Services.Logging
{

    public static class HttpContextGetter
    {
        private static IHttpContextAccessor httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContext)
        {
            httpContextAccessor = httpContext;
        }

        public static HttpContext Current => httpContextAccessor.HttpContext;

    }
}
