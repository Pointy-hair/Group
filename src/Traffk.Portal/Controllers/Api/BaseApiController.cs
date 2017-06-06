using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System;
using Traffk.Bal.Settings;

namespace Traffk.Portal.Controllers.Api
{
    public abstract class BaseApiController : Controller
    {
        private readonly ILogger Logger;

        protected BaseApiController(ILogger logger)
        {
            Logger = logger.ForContext(new ILogEventEnricher[]
            {
                new PropertyEnricher(typeof(Type).Name, this.GetType().Name),
            });
        }

        protected void Log()
        {
            Log("");
        }

        protected void Log(string message)
        {
            string referer = Request.Path.Value;
            Logger.Information("{@EventType} {@Referer} " + message, EventType.LoggingEventTypes.ApiCall.ToString(), referer);
        }
    }
}