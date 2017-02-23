using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace TraffkPortal.Services.Logging
{
    public static class LoggerHelpers
    {
        public static void AttachLogContextProperty(string name, string val)
        {
            LogContext.PushProperty(name, val);
        }
    }
}
