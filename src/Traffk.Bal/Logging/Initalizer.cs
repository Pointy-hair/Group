using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RevolutionaryStuff.Core;
using Serilog;
using Serilog.Sinks.SumoLogic;
using ILogger = Serilog.ILogger;


namespace Traffk.Bal.Logging
{
    public class Initalizer
    {
        private static bool InitLoggerCalled;

        public static LoggerConfiguration InitLoggerConfiguration(IConfigurationRoot configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                    .Enrich.WithProperty("ApplicationName", configuration["RevolutionaryStuffCoreOptions:ApplicationName"])
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.With<EventTimeEnricher>()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Trace()
                    .WriteTo.AzureTableStorageWithProperties(configuration.GetSection("BlobStorageServicesOptions")["ConnectionString"],
                        storageTableName: configuration["Serilog:TableName"],
                        writeInBatches: Parse.ParseBool(configuration["Serilog:WriteInBatches"], true),
                        period: Parse.ParseTimeSpan(configuration["Serilog:LogInterval"], TimeSpan.FromSeconds(2)));

		    var sumoLogicUrl = configuration["Serilog:SumoLogicUrl"];
		    if (sumoLogicUrl != null)
		    {
		    	loggerConfiguration = loggerConfiguration.WriteTo.SumoLogic(sumoLogicUrl);
		    }
			return loggerConfiguration;
        }

        public static ILogger InitLogger(IConfigurationRoot configuration, LoggerConfiguration loggerConfiguration = null)
        {
            Requires.SingleCall(ref InitLoggerCalled);

            loggerConfiguration = loggerConfiguration ?? InitLoggerConfiguration(configuration);
            var logger = loggerConfiguration.CreateLogger();
            Log.Logger = logger;
            return logger;
        }
	}
}
