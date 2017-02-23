using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Traffk.Bal.Settings;
using TraffkPortal.Services.Logging;

namespace TraffkPortal.Services
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger logger;

        public GlobalExceptionFilter()
        {
            logger = Log.ForContext(typeof(EventType).Name, EventType.LoggingEventTypes.Exception.ToString());
        }

        public void OnException(ExceptionContext context)
        {
            logger.Error("Exception {@Exception}", context.Exception);
        }
    }
}
