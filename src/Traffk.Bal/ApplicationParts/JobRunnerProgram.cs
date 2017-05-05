﻿using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Threading.Tasks;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Services;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

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

        protected override Task OnGoAsync()
        {
            GDB = this.ServiceProvider.GetService<TraffkGlobalContext>();
            var o = this.ServiceProvider.GetService<IOptions<HangfireServerOptions>>().Value;

            GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString(o.ConnectionStringName));
            GlobalConfiguration.Configuration.UseActivator(new MyActivator(this));
            using (var s = new BackgroundJobServer(o.BackgroundOptions ?? new BackgroundJobServerOptions()))
            {
                //Send something to Serilog
                //WaitOne that has a delay (TimeSpan) object
                //TimeSpan is in AppConfig
                //Add for or do while loop and check the result of the WaitOne
                ShutdownRequested.WaitOne();
            }
            return Task.CompletedTask;
        }

        [ThreadStatic]
        private MyActivator.MyScope CurrentThreadScope;

        ApplicationUser ICurrentUser.User => null;

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
