using Hangfire;
using Hangfire.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Traffk.Utility;
using ILogger = Serilog.ILogger;

namespace Traffk.Bal.ApplicationParts
{
    public abstract partial class JobRunnerProgram : CommandLineProgram
    {
        public static new void Main<TProgram>(string[] args) where TProgram : CommandLineProgram => CommandLineProgram.Main<TProgram>(args);

        public class HangfireServerConfig
        {
            public const string ConfigSectionName = "HangfireServerConfig";

            public string ConnectionStringName { get; set; }
            public BackgroundJobServerOptions BackgroundOptions { get; set; }
        }

        protected override void OnBuildConfiguration(IConfigurationBuilder builder)
        {
            var config = builder.Build();

            builder.AddAzureKeyVault(
                $"https://{config["Vault"]}.vault.azure.net/",
                config["ClientId"],
                config["ClientSecret"],
                new TraffkSecretManager(config["ClientAppName"]));

            builder.Build();
        }

        protected override Task OnGoAsync()
        {
            if (int.TryParse(Configuration["HardcodedJobId"], out var jobId))
            {
                GoHardcoded(jobId);
            }
            else
            {
                GoHangfireServer();
            }
            return Task.CompletedTask;
        }

        private void GoHardcoded(int jobId)
        {
            var a = new MyActivator(this);
            using (var scope = (MyActivator.MyScope) a.BeginScope(jobId))
            {
                scope.Run();
            }
        }

        private void GoHangfireServer()
        {
            var o = this.ServiceProvider.GetService<IOptions<HangfireServerConfig>>().Value;
            var logger = this.ServiceProvider.GetService<ILogger>();

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
                    logger.Information("Job Runner Heartbeat.");
                }
                while (!ShutdownRequested.WaitOne(timeout));
            }
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

            var logger = Traffk.Bal.Logging.Initalizer.InitLogger(Configuration);
            services.AddScoped<ILogger>(provider => logger.ForContext<JobRunnerProgram>());
        }
    }
}
