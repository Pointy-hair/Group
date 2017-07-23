using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Traffk.Bal.Services
{
    public class CorrelationIdMiddleware
    {
        public class Config
        {
            public const string ConfigSectionName = "CorrelationIdOptions";

            private const string DefaultHeader = "X-Correlation-ID";

            public string Header { get; set; } = DefaultHeader;
            public bool IncludeInResponse { get; set; } = true;
        }

        private readonly RequestDelegate Next;
        private readonly IOptions<Config> Options;

        public CorrelationIdMiddleware(RequestDelegate next, IOptions<Config> options)
        {
            Options = options;
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(Options.Value.Header, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }

            if (Options.Value.IncludeInResponse)
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(Options.Value.Header, new[] { context.TraceIdentifier });
                    return Task.CompletedTask;
                });
            }

            await Next(context);
        }
    }

    public static class CorrelationIdExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, string header)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseCorrelationId(new CorrelationIdMiddleware.Config()
            {
                Header = header
            });
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, CorrelationIdMiddleware.Config options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<CorrelationIdMiddleware>(Options.Create(options));
        }
    }
}
