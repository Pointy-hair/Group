using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Traffk.Bal.Settings;

namespace TraffkPortal.Services
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger Logger;

        public GlobalExceptionFilter()
        {
            Logger = Log.ForContext(typeof(EventType).Name, EventType.LoggingEventTypes.Exception.ToString());
        }

        public void OnException(ExceptionContext context)
        {
            Logger.Error("Exception {@Exception}", context.Exception);
        }
    }
}
