using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using Serilog;
using System;
using System.Threading.Tasks;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Bal.Logging;
using Traffk.Bal.ReportVisuals;
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

        public ILogger JobRunnerLogger { get; private set; }

        public class HangfireServerOptions
        {
            public string ConnectionStringName { get; set; }
            public BackgroundJobServerOptions BackgroundOptions { get; set; }
        }

        private TraffkGlobalDbContext GDB;

        protected override Task OnGoAsync()
        {
            GDB = this.ServiceProvider.GetService<TraffkGlobalDbContext>();
            var o = this.ServiceProvider.GetService<IOptions<HangfireServerOptions>>().Value;
            JobRunnerLogger = this.ServiceProvider.GetService<ILogger>();

            GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString(o.ConnectionStringName));
            GlobalConfiguration.Configuration.UseActivator(new MyActivator(this));
            GlobalJobFilters.Filters.Add(new TraffkJobFilterAttribute(Configuration));

            using (var s = new BackgroundJobServer(o.BackgroundOptions ?? new BackgroundJobServerOptions()))
            {
                var timeout = Parse.ParseTimeSpan(Configuration["JobRunner:TimeOut"], TimeSpan.FromSeconds(60));
                do
                {
                    JobRunnerLogger.Information("Job Runner Heartbeat.");
                }
                while (!ShutdownRequested.WaitOne(timeout));
            }

            return Task.CompletedTask;
        }

        [ThreadStatic]
        private MyActivator.MyScope CurrentThreadScope;

        ApplicationUser ICurrentUser.User => null;

        public int? JobId => CurrentThreadScope.JobId;

        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.AddOptions();

            services.Configure<BlobStorageServices.BlobStorageServicesOptions>(Configuration.GetSection(nameof(BlobStorageServices.BlobStorageServicesOptions)));

            services.AddSingleton(this);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ITraffkTenantFinder, MyTraffkTenantFinder>();
            services.AddSingleton<ICurrentUser>(this);
            services.AddScoped<ConfigStringFormatter>();
            services.AddDbContext<TraffkTenantShardsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(TraffkTenantShardsDbContext.DefaultDatabaseConnectionStringName)), ServiceLifetime.Scoped);
            services.AddDbContext<TraffkTenantModelDbContext>((sp, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(TraffkTenantModelDbContext.DefaultDatabaseConnectionStringName));
            }, ServiceLifetime.Scoped);
            services.AddDbContext<TraffkGlobalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(TraffkGlobalDbContext.DefaultDatabaseConnectionStringName)), ServiceLifetime.Scoped);
            services.AddScoped<CurrentTenantServices>();
            services.AddScoped<BlobStorageServices>();
            services.Configure<HangfireServerOptions>(Configuration.GetSection(nameof(HangfireServerOptions)));


            services.Configure<TableauSignInOptions>(Configuration.GetSection(nameof(TableauSignInOptions)));
            services.Configure<TableauAdminCredentials>(Configuration.GetSection(nameof(TableauAdminCredentials)));

            services.AddScoped<ITableauTenantFinder, TableauTenantFinder>();
            services.AddScoped<ITableauUserCredentials, MyTableauAdminCredentials>();
            services.AddScoped<ITrustedTicketGetter, TrustedTicketGetter>();
            services.AddScoped<ITableauVisualServices, TableauVisualServices>();
            services.AddScoped<ITableauAdminService, TableauAdminService>();

            var logger = new LoggerConfiguration()
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

            services.AddScoped<ILogger>(provider => logger.ForContext<JobRunnerProgram>());
        }
    }
}
