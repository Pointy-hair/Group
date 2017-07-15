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
using RevolutionaryStuff.Core.Caching;
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
using System.Collections.Generic;
using System.Linq;

namespace Traffk.Bal.ApplicationParts
{
    public abstract partial class JobRunnerProgram : CommandLineProgram
    {
        public static new void Main<TProgram>(string[] args) where TProgram : CommandLineProgram => CommandLineProgram.Main<TProgram>(args);

        public ILogger JobRunnerLogger { get; private set; }

        public class HangfireServerConfig
        {
            public const string ConfigSectionName = "HangfireServerOptions";

            public string ConnectionStringName { get; set; }
            public BackgroundJobServerOptions BackgroundOptions { get; set; }
        }

        protected override Task OnGoAsync()
        {
            var o = this.ServiceProvider.GetService<IOptions<HangfireServerConfig>>().Value;
            JobRunnerLogger = this.ServiceProvider.GetService<ILogger>();

            GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString(o.ConnectionStringName));
            GlobalConfiguration.Configuration.UseActivator(new MyActivator(this));
            GlobalJobFilters.Filters.Add(new TraffkJobFilterAttribute(Configuration));

            var bo = o.BackgroundOptions ?? new BackgroundJobServerOptions();
            var queues = new List<string>();
            queues.AddRange(bo.Queues.Where(q => !q.StartsWith("-")).ToArray());
            bo.Queues.Where(q => q.StartsWith("-")).ForEach(q => queues.Remove(q.Substring(1)));
            bo.Queues = queues.ToArray();

            using (var s = new BackgroundJobServer(bo))
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

        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.AddOptions();
            services.Configure<BlobStorageServices.Config>(Configuration.GetSection(BlobStorageServices.Config.ConfigSectionName));
            services.Configure<HangfireServerConfig>(Configuration.GetSection(HangfireServerConfig.ConfigSectionName));
            services.Configure<TableauSignInOptions>(Configuration.GetSection(TableauSignInOptions.ConfigSectionName));
            services.Configure<TableauAdminCredentials>(Configuration.GetSection(TableauAdminCredentials.ConfigSectionName));

            services.AddSingleton(this);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<ITraffkTenantFinder, MyTraffkTenantFinder>();
            services.AddScoped<IJobInfoFinder, MyJobInfoFinder>();
            services.AddScoped<ICurrentUser, MyCurrentUser>();
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

            services.AddScoped<ITableauTenantFinder, TableauTenantFinder>();
            services.AddScoped<ITableauUserCredentials, MyTableauAdminCredentials>();
            services.AddScoped<ITrustedTicketGetter, TrustedTicketGetter>();
            services.AddScoped<ITableauVisualServices, TableauVisualServices>();
            services.AddScoped<ITableauAdminService, TableauAdminService>();
            services.AddScoped<IReportVisualService, ReportVisualService>();

            services.AddScoped<IBackgroundJobClient, TenantedBackgroundJobClient>();
            services.AddScoped<ITraffkRecurringJobManager, TenantedBackgroundJobClient>();
            services.AddScoped<IRecurringJobManager, RecurringJobManager>();
            services.Add(new ServiceDescriptor(typeof(ICacher), Cache.DataCacher));

            services.Configure<HttpClientFactory.Config>(Configuration.GetSection(HttpClientFactory.Config.ConfigSectionName));
            services.AddScoped<IHttpClientFactory, HttpClientFactory>();

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
