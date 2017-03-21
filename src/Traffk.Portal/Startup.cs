using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Serilog.Core;
using Serilog.Events;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Email;
using Traffk.Bal.Identity;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Portal.Services;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using TraffkPortal.Permissions;
using TraffkPortal.Services;
using TraffkPortal.Services.Logging;
using TraffkPortal.Services.Sms;
using TraffkPortal.Services.TenantServices;

namespace TraffkPortal
{
    public class Startup
    {
        public static bool IsSigninPersistent = true;
        private readonly bool IsDevelopment;
        private readonly bool RequireHttps;
        private readonly TimeSpan IdleLogout;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                IsDevelopment = true;

                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            RequireHttps = Parse.ParseBool(Configuration["RequireHttps"], true);
            IdleLogout = Parse.ParseTimeSpan(Configuration["IdleLogout"], TimeSpan.FromMinutes(15));
            TraffkHttpHeadersFilter.IdleLogout = IdleLogout;

            Log.Logger = new LoggerConfiguration()
                    .Enrich.WithProperty("ApplicationName", Configuration["RevolutionaryStuffCoreOptions:ApplicationName"])
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.With<EventTimeEnricher>()
                    .Enrich.With<UserEnricher>()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Trace()
                    .WriteTo.AzureTableStorageWithProperties(Configuration["BlobStorageServicesOptions:ConnectionString"], 
                        storageTableName: Configuration["Serilog:TableName"], 
                        writeInBatches: Parse.ParseBool(Configuration["Serilog:WriteInBatches"], true), 
                        period: Parse.ParseTimeSpan(Configuration["Serilog:LogInterval"], TimeSpan.FromSeconds(2)))
                    .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));

            /*
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = IdleLogout;
            });
            */

            services.AddOptions();
            services.Configure<TenantFinderService.TenantServiceFinderOptions>(Configuration.GetSection(nameof(TenantFinderService.TenantServiceFinderOptions)));
            services.Configure<CachingServices.CachingServicesOptions>(Configuration.GetSection(nameof(CachingServices.CachingServicesOptions)));
            services.Configure<PreferredHostnameFilter.PreferredHostnameFilterOptions>(Configuration.GetSection(nameof(PreferredHostnameFilter.PreferredHostnameFilterOptions)));
            services.Configure<PortalOptions>(Configuration.GetSection(nameof(PortalOptions)));
            services.Configure<NoTenantMiddleware.NoTenantMiddlewareOptions>(Configuration.GetSection(nameof(NoTenantMiddleware.NoTenantMiddlewareOptions)));
            services.Configure<BlobStorageServices.BlobStorageServicesOptions>(Configuration.GetSection(nameof(BlobStorageServices.BlobStorageServicesOptions)));
            services.Configure<TwilioSmsSenderOptions>(Configuration.GetSection(nameof(TwilioSmsSenderOptions)));
            services.Configure<TableauSignInOptions>(Configuration.GetSection(nameof(TableauSignInOptions)));
            services.Configure<DataProtectionTokenProviderOptions>(Configuration.GetSection(nameof(DataProtectionTokenProviderOptions)));
            services.Configure<TraffkHttpHeadersFilter.TraffkHttpHeadersFilterOptions>(Configuration.GetSection(nameof(TraffkHttpHeadersFilter.TraffkHttpHeadersFilterOptions)));


            services.AddSingleton<CachingServices>();

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<TenantRdbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TenantServices")), ServiceLifetime.Singleton);

            services.AddDbContext<TraffkRdbContext>((sp,options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("TenantXPortal"));
            }, ServiceLifetime.Scoped);

            services.Configure<IdentityOptions>(options => 
            {
                options.Cookies.ApplicationCookie.SlidingExpiration = true;
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromSeconds(IdleLogout.TotalSeconds*2); //due to sliding expiration, things can be cut in half...
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<TraffkRdbContext>()
                .AddDefaultTokenProviders();

            /*
             * To change password validation, sub out the following....
+		[49]	Lifetime = Scoped, ServiceType = {Microsoft.AspNetCore.Identity.IPasswordValidator`1[TraffkPortal.Models.ApplicationUser]}, ImplementationType = {Microsoft.AspNetCore.Identity.PasswordValidator`1[TraffkPortal.Models.ApplicationUser]}	Microsoft.Extensions.DependencyInjection.ServiceDescriptor             
             */

            services.Substitute<TraffkUserManager>();
            services.Substitute<TraffkUserStore>();
            services.Substitute<TraffkPasswordValidator>();

            services.AddPermissions();

            services.AddMvc(config =>
            {
                config.Filters.Add(new GlobalExceptionFilter());
                config.Filters.Add(typeof(PreferredHostnameFilter));
                config.Filters.Add(typeof(TraffkHttpHeadersFilter));
                if (RequireHttps)
                {
                    config.Filters.Add(new RequireHttpsAttribute());
                }
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();


            // Add application services.
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<IEmailer, RawEmailer>();
            services.AddScoped<ITrackingEmailer, TrackingEmailer>();
            services.AddScoped<ICommunicationBlastFinder, SystemCommunicationCommunicationBlastFinder>();
            services.AddScoped<Traffk.Bal.Communications.ICreativeSettingsFinder, TraffkRdbContext>();
            services.AddScoped<IEmailSender, AuthMessageSender>();
            services.AddScoped<ISmsSender, AuthMessageSender>();
            services.AddSingleton<ITwilioSmsSender, TwilioSmsSender>();
            services.AddScoped<CurrentTenantServices>();
            services.AddScoped<TenantFinderService>();
            services.AddScoped<ITraffkTenantFinder, TenantFinderService>();
            services.AddScoped<CurrentContextServices>();
            services.AddScoped<ICurrentUser, CurrentContextServices>();
            services.AddScoped<Resources.PortalResourceServiceBuilder>();
            services.AddScoped<Resources.PortalResourceService>();
            services.AddScoped<BlobStorageServices>();
            services.AddScoped<ConfigStringFormatter>();
            services.AddScoped<ITableauServices, TableauServices>();
            services.AddScoped<ITrustedTicketGetter, TrustedTicketGetter>();
            services.AddScoped<ITableauRestService, TableauRestService>();
            services.AddScoped<ITableauTenantFinder, TableauTenantFinder>();
            services.AddScoped<IReportVisualService, ReportVisualService>();

            services.AddScoped<TableauTrustedTicketActionFilter>();

            services.Add(new ServiceDescriptor(typeof(ICacher), Cache.DataCacher));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog(Log.Logger);
            //loggerFactory.AddProvider();
            //ILoggerProvider fds;

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/E");
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/E");
            }

            app.UseMiddleware<NoTenantMiddleware>();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            if (RequireHttps)
            {
                app.UseCookiePolicy(new CookiePolicyOptions { Secure = CookieSecurePolicy.Always });
            }

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseSession();


            CachingServices.Initialize(app.ApplicationServices);
            UserEnricher.Initialize(app.ApplicationServices);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
