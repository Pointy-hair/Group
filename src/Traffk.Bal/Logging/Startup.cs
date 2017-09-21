using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RevolutionaryStuff.Core;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Traffk.Bal.Logging
{
    public class Startup
    {

        public LoggerConfiguration InitLoggerConfiguration()
        {
            var loggerConfiguration = new LoggerConfiguration()
                    .Enrich.WithProperty("ApplicationName", Configuration["RevolutionaryStuffCoreOptions:ApplicationName"])
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.With<EventTimeEnricher>()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Trace()
                    .WriteTo.AzureTableStorageWithProperties(Configuration.GetSection("BlobStorageServicesOptions")["ConnectionString"],
                        storageTableName: Configuration["Serilog:TableName"],
                        writeInBatches: Parse.ParseBool(Configuration["Serilog:WriteInBatches"], true),
                        period: Parse.ParseTimeSpan(Configuration["Serilog:LogInterval"], TimeSpan.FromSeconds(2)));
            return loggerConfiguration;
        }

        public ILogger InitLogger(LoggerConfiguration loggerConfiguration = null)
        {
            loggerConfiguration = loggerConfiguration ?? InitLoggerConfiguration();
            var logger = loggerConfiguration.CreateLogger();
            Log.Logger = logger;
            return logger;
        }
    }
}
