using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog.Core;
using Serilog.Events;

namespace TraffkPortal.Services.Logging
{
    public class UserEnricher : ILogEventEnricher
    {
        private const string UserIdPropertyName = "UserId";
        private const string UserNamePropertyName = "UserName";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = HttpContextGetter.Current;
            var currentContext = CurrentContextGetter.CurrentContext;

            if (httpContext != null)
            {
                var userName = propertyFactory.CreateProperty(UserNamePropertyName, httpContext.Items[UserNamePropertyName] ?? "");
                logEvent.AddPropertyIfAbsent(userName);

                var userId = propertyFactory.CreateProperty(UserIdPropertyName, httpContext.Items[UserIdPropertyName] ?? "");
                logEvent.AddPropertyIfAbsent(userId);
            }
            else
            {
                var userName = propertyFactory.CreateProperty(UserNamePropertyName, "None");
                logEvent.AddPropertyIfAbsent(userName);
            }

            //if (httpContext?.Session != null)
            //{
            //    var session = httpContext.Session;

            //    var userId = propertyFactory.CreateProperty(UserIdPropertyName, session.GetString(UserIdPropertyName) ?? "");
            //    logEvent.AddPropertyIfAbsent(userId);

            //    var userName = propertyFactory.CreateProperty(UserNamePropertyName, session.GetString(UserNamePropertyName) ?? "");
            //    logEvent.AddPropertyIfAbsent(userName);
            //}
        }
    }
}
