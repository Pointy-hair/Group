using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace TraffkPortal.Services.Logging
{
    public class EventTimeEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var eventTime = propertyFactory.CreateProperty("EventTime", DateTime.UtcNow);
            logEvent.AddPropertyIfAbsent(eventTime);
        }
    }
}
