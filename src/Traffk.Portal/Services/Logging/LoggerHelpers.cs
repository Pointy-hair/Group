using Serilog.Context;
using Serilog.Events;
using Serilog.Core;

namespace TraffkPortal.Services.Logging
{
    public static class LoggerHelpers
    {
        public static void AttachLogContextProperty(string name, string val)
        {
            LogContext.PushProperty(name, val);
        }

        public static void AddPropertyValueIfAbsent(this LogEvent logEvent, ILogEventPropertyFactory propertyFactory, string name, object val)
            => logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(name, val));
    }
}
