using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Logging
{
    public abstract class LoggedService
    {
        protected readonly ILogger Logger;

        protected LoggedService(ILogger logger)
        {
            Logger = logger.ForContext(new ILogEventEnricher[]
            {
                new PropertyEnricher(typeof(Type).Name, this.GetType().Name),
            });
        }

        protected void Log(EventType.LoggingEventTypes eventType = EventType.LoggingEventTypes.Default, string message = null)
        {
            Logger.Information("{@EventType} " + (message ?? ""), eventType.ToString() ?? "");
        }
    }
}
