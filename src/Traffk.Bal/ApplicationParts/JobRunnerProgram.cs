using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using Serilog;
using System;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Logging;
using Traffk.Bal.Services;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using ILogger = Serilog.ILogger;


namespace Traffk.Bal.ApplicationParts
{
    public abstract partial class JobRunnerProgram : CommandLineProgram, ICurrentUser
    {
        public static new void Main<TProgram>(string[] args) where TProgram : CommandLineProgram => CommandLineProgram.Main<TProgram>(args);

        public class HangfireServerOptions
        {
            public string ConnectionStringName { get; set; }
            public BackgroundJobServerOptions BackgroundOptions { get; set; }
        }

        private TraffkGlobalContext GDB;
        private ILogger Logger;

        protected override Task OnGoAsync()
        {
            GDB = this.ServiceProvider.GetService<TraffkGlobalContext>();
            var o = this.ServiceProvider.GetService<IOptions<HangfireServerOptions>>().Value;

            GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString(o.ConnectionStringName));
            GlobalConfiguration.Configuration.UseActivator(new MyActivator(this));

            var loggerFactory = this.ServiceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddSerilog();

            Log.Logger = new LoggerConfiguration()
                    .Enrich.WithProperty("ApplicationName", Configuration["RevolutionaryStuffCoreOptions:ApplicationName"])
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.With<EventTimeEnricher>()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Trace()
                    .WriteTo.AzureTableStorageWithProperties(Configuration["BlobStorageServicesOptions:ConnectionString"],
                        storageTableName: Configuration["Serilog:TableName"],
                        writeInBatches: Parse.ParseBool(Configuration["Serilog:WriteInBatches"], true),
                        period: Parse.ParseTimeSpan(Configuration["Serilog:LogInterval"], TimeSpan.FromSeconds(2)))
                    .CreateLogger();

            Logger = Log.Logger;

            using (var s = new BackgroundJobServer(o.BackgroundOptions ?? new BackgroundJobServerOptions()))
            {
                var timeoutInSeconds = int.Parse(Configuration["JobRunner:TimeoutInSeconds"]);
                do
                {
                    Logger.Information("Job runner started.");
                    //Console.WriteLine("Job runner started.");
                }
                while (!ShutdownRequested.WaitOne(TimeSpan.FromSeconds(timeoutInSeconds)));
            }

            //do
            //{
            //    using (var s = new BackgroundJobServer(o.BackgroundOptions ?? new BackgroundJobServerOptions()))
            //    {
            //        Console.WriteLine("Job runner started. Thread ID {0}", Thread.CurrentThread.ManagedThreadId);
            //        //Logger.Information("Job runner started.");
            //    }
            //} while (!ShutdownRequested.WaitOne(TimeSpan.FromSeconds(Convert.ToInt32(Configuration["JobRunner:TimeoutInSeconds"]))));
            
            return Task.CompletedTask;
        }

        [ThreadStatic]
        private MyActivator.MyScope CurrentThreadScope;

        ApplicationUser ICurrentUser.User => null;

        public int? JobId => CurrentThreadScope.JobId;

        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.Configure<BlobStorageServices.BlobStorageServicesOptions>(Configuration.GetSection(nameof(BlobStorageServices.BlobStorageServicesOptions)));

            services.AddSingleton(this);
            services.AddSingleton<ITraffkTenantFinder, MyTraffkTenantFinder>();
            services.AddSingleton<ICurrentUser>(this);
            services.AddScoped<ConfigStringFormatter>();
            services.AddDbContext<TenantRdbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(TenantRdbContext.DefaultDatabaseConnectionStringName)), ServiceLifetime.Singleton);
            services.AddDbContext<TraffkRdbContext>((sp, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(TraffkRdbContext.DefaultDatabaseConnectionStringName));
            }, ServiceLifetime.Scoped);
            services.AddDbContext<TraffkGlobalContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(TraffkGlobalContext.DefaultDatabaseConnectionStringName)), ServiceLifetime.Singleton);
            services.AddScoped<CurrentTenantServices>();
            services.AddScoped<BlobStorageServices>();
            services.Configure<HangfireServerOptions>(Configuration.GetSection(nameof(HangfireServerOptions)));


            services.Configure<TableauSignInOptions>(Configuration.GetSection(nameof(TableauSignInOptions)));
            services.Configure<TableauAdminCredentials>(Configuration.GetSection(nameof(TableauAdminCredentials)));
            services.AddScoped<ITableauAdminService, TableauAdminService>();
        }
    }
}
