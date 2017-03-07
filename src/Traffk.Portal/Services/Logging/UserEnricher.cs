using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RevolutionaryStuff.Core;
using Serilog.Core;
using Serilog.Events;
using System;

namespace TraffkPortal.Services.Logging
{
    public class UserEnricher : ILogEventEnricher
    {
        private static bool InitializeCalled;
        public static void Initialize(IServiceProvider provider)
        {
            Requires.SingleCall(ref InitializeCalled);
            Provider = provider;
        }

        private static IServiceProvider Provider;

        private const string TenantIdPropertyName = "TenantId";
        private const string PrincipalNamePropertyName = "PrincipalName";

        private void AddProperty(LogEvent logEvent, ILogEventPropertyFactory propertyFactory, string name, object val)
            => logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(name, val));

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            try
            {
                using (var p = Provider.CreateScope())
                {
                    var acc = p.ServiceProvider.GetService<IHttpContextAccessor>();
                    if (acc != null)
                    {
                        var httpContext = acc.HttpContext;
                        if (httpContext != null)
                        {
                            int tenantId = (int)httpContext.Items[TenantServices.TenantFinderService.HttpContextTenantIdKey];
                            logEvent.AddPropertyValueIfAbsent(propertyFactory, TenantIdPropertyName, tenantId);
                            var principal = httpContext.User;
                            if (principal != null && principal.Identity != null && principal.Identity.Name != null)
                            {
                                logEvent.AddPropertyValueIfAbsent(propertyFactory, PrincipalNamePropertyName, principal.Identity.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}
