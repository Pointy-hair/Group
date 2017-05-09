using System;
using Serilog.Core;
using Serilog.Events;

namespace Traffk.Bal.Logging
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
