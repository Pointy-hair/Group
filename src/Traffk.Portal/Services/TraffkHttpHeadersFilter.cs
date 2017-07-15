using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System;
using System.Threading.Tasks;

namespace TraffkPortal.Services
{
    public class TraffkHttpHeadersFilter : IAsyncActionFilter
    {
        public static TimeSpan IdleLogout;

        public class Config
        {
            public const string ConfigSectionName = "TraffkHttpHeadersFilterOptions";

            public bool IncludeMachineName { get; set; } = true;
            public bool IncludeServerTime { get; set; } = true;
            public bool IncludeEnvironmentInformation { get; set; } = true;
            public string VendorName { get; set; } = "Traffk, LLC";
        }

        private readonly IOptions<Config> Options;
        private readonly IHostingEnvironment Host;

        public TraffkHttpHeadersFilter(IOptions<Config> options, IHostingEnvironment host)
        {
            Requires.NonNull(options, nameof(options));
            Options = options;
            Host = host;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var o = Options.Value;
            var headers = context.HttpContext.Response.Headers;
            if (!headers.ContainsKey(WebHelpers.HeaderStrings.CacheControl))
            {
                headers[WebHelpers.HeaderStrings.CacheControl] = "no-cache, no-store";
            }
            if (o.IncludeMachineName)
            {
                headers["x-MachineName"] = Environment.MachineName;
            }
            if (o.IncludeServerTime)
            {
                headers["x-ServerTime"] = DateTime.Now.ToString();
                headers["x-ServerTimeUtc"] = DateTime.UtcNow.ToString();
            }
            if (o.IncludeEnvironmentInformation)
            {
                headers["x-Environment"] = Host.EnvironmentName;
                headers["x-ApplicationName"] = Host.ApplicationName;
            }
            if (!string.IsNullOrWhiteSpace(o.VendorName))
            {
                headers["x-VendorName"] = o.VendorName;
            }
            context.HttpContext.Response.Cookies.Append("loggedIn", context.HttpContext.User.Identity.IsAuthenticated ? "true" : "false");
            context.HttpContext.Response.Cookies.Append("sessionTimeoutInSeconds", IdleLogout.TotalSeconds.ToString());
            await next();
        }
    }
}
