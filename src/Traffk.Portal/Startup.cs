using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using System;
using System.Linq;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Traffk.Bal;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Caches;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Bal.Email;
using Traffk.Bal.ExternalApis;
using Traffk.Bal.Identity;
using Traffk.Bal.Logging;
using Traffk.Bal.Permissions;
using Traffk.Bal.ReportVisuals;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using Traffk.Orchestra;
using Traffk.Portal.Controllers.Api;
using Traffk.Portal.Permissions;
using Traffk.Portal.Services;
using Traffk.Tableau;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Utility;
using TraffkPortal.Permissions;
using TraffkPortal.Services;
using TraffkPortal.Services.Logging;
using TraffkPortal.Services.Sms;
using TraffkPortal.Services.TenantServices;
using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TraffkPortal
{
    public class Startup
    {
        public static bool IsSigninPersistent = true;
        private readonly bool RequireHttps;
        private readonly TimeSpan IdleLogout;

        private readonly ILogger Logger;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            var config = builder.Build();

            builder.AddAzureKeyVault(
                $"https://{config["Vault"]}.vault.azure.net/",
                config["ClientId"],
                config["ClientSecret"],
                new TraffkSecretManager(config["ClientAppName"]));

            if (env.IsDevelopment())
            {
                builder.AddAzureKeyVault(
                    $"https://{config["Vault"]}.vault.azure.net/",
                    config["ClientId"],
                    config["ClientSecret"],
                    new TraffkSecretManager(config["ClientAppName"] + config["ClientAppNameEnvironment"]));
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            RequireHttps = Parse.ParseBool(Configuration["RequireHttps"], true);
            IdleLogout = Parse.ParseTimeSpan(Configuration["IdleLogout"], TimeSpan.FromMinutes(15));
            TraffkHttpHeadersFilter.IdleLogout = IdleLogout;

            var loggerConfiguration = new LoggerConfiguration()
                    .Enrich.WithProperty("ApplicationName", Configuration["RevolutionaryStuffCoreOptions:ApplicationName"])
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.With<EventTimeEnricher>()
                    .Enrich.With<UserEnricher>()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Trace()
                    .WriteTo.AzureTableStorageWithProperties(Configuration.GetSection("BlobStorageServicesOptions")["ConnectionString"], 
                        storageTableName: Configuration["Serilog:TableName"], 
                        writeInBatches: Parse.ParseBool(Configuration["Serilog:WriteInBatches"], true), 
                        period: Parse.ParseTimeSpan(Configuration["Serilog:LogInterval"], TimeSpan.FromSeconds(2)));

            Logger = loggerConfiguration.CreateLogger();
            Log.Logger = Logger;

            GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString("TraffkGlobal"));
            GlobalJobFilters.Filters.Add(new TraffkJobFilterAttribute(Configuration));
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
                options.IdleTimeout = IdleLogout
            });
            */

            services.AddOptions();
            services.Configure<TenantFinderService.Config>(Configuration.GetSection(TenantFinderService.Config.ConfigSectionName));
            services.Configure<PreferredHostnameFilter.Config>(Configuration.GetSection(PreferredHostnameFilter.Config.ConfigSectionName));
            services.Configure<PortalConfig>(Configuration.GetSection(PortalConfig.ConfigSectionName));
            services.Configure<NoTenantMiddleware.Config>(Configuration.GetSection(NoTenantMiddleware.Config.ConfigSectionName));
            services.Configure<BlobStorageServices.Config>(Configuration.GetSection(BlobStorageServices.Config.ConfigSectionName));
            services.Configure<TwilioSmsSenderConfig>(Configuration.GetSection(TwilioSmsSenderConfig.ConfigSectionName));
            services.Configure<TableauSignInOptions>(Configuration.GetSection(TableauSignInOptions.ConfigSectionName));
            services.Configure<TableauAdminCredentials>(Configuration.GetSection(TableauAdminCredentials.ConfigSectionName));
            services.Configure<DataProtectionTokenProviderOptions>(Configuration.GetSection(nameof(DataProtectionTokenProviderOptions)));
            services.Configure<TraffkHttpHeadersFilter.Config>(Configuration.GetSection(TraffkHttpHeadersFilter.Config.ConfigSectionName));
            services.Configure<TokenProviderConfig>(Configuration.GetSection(TokenProviderConfig.ConfigSectionName));
            services.Configure<OrchestraRxConfig>(Configuration.GetSection(OrchestraRxConfig.ConfigSectionName));
            services.Configure<RedisCache.Config>(Configuration.GetSection(RedisCache.Config.ConfigSectionName));
            services.Configure<ActiveDirectoryApplicationIdentificationConfig>(Configuration.GetSection(ActiveDirectoryApplicationIdentificationConfig.ConfigSectionName));
            services.Configure<OrchestraApiService.Config>(Configuration.GetSection(OrchestraApiService.Config.ConfigSectionName));

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            const string connectionStringKey = "ConnectionStrings";

            services.AddDbContext<TraffkTenantShardsDbContext>(options =>
                options.UseSqlServer(Configuration.GetSection(connectionStringKey)[TraffkTenantShardsDbContext.DefaultDatabaseConnectionStringName]), ServiceLifetime.Singleton);

            services.AddDbContext<TraffkTenantModelDbContext>((sp, options) =>
                options.UseSqlServer(Configuration.GetSection(connectionStringKey)[TraffkTenantModelDbContext.DefaultDatabaseConnectionStringName]), ServiceLifetime.Scoped);

            services.AddDbContext<TraffkGlobalDbContext>(options =>
                options.UseSqlServer(Configuration.GetSection(connectionStringKey)[TraffkGlobalDbContext.DefaultDatabaseConnectionStringName]), ServiceLifetime.Scoped);

            services.AddAuthentication().AddCookie("TraffkAuth", o =>
            {
                o.ExpireTimeSpan = TimeSpan.FromSeconds(IdleLogout.TotalSeconds * 2); //due to sliding expiration, things can be cut in half...
                o.SlidingExpiration = true;
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<TraffkTenantModelDbContext>()
                .AddDefaultTokenProviders();

            /*
             * To change password validation, sub out the following....
+		[49]	Lifetime = Scoped, ServiceType = {Microsoft.AspNetCore.Identity.IPasswordValidator`1[TraffkPortal.Models.ApplicationUser]}, ImplementationType = {Microsoft.AspNetCore.Identity.PasswordValidator`1[TraffkPortal.Models.ApplicationUser]}	Microsoft.Extensions.DependencyInjection.ServiceDescriptor             
             */

            services.Substitute<TraffkUserManager>();
            services.Substitute<TraffkUserStore>();
            services.Substitute<TraffkPasswordValidator>();

            services.AddPermissions();
            services.AddApis();

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
            
            services.AddScoped<IVault, Vault>();
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<IEmailer, RawEmailer>();
            services.AddScoped<ITrackingEmailer, TrackingEmailer>();
            services.AddScoped<ICommunicationBlastFinder, SystemCommunicationCommunicationBlastFinder>();
            services.AddScoped<Traffk.Bal.Communications.ICreativeSettingsFinder, TraffkTenantModelDbContext>();
            services.AddScoped<IEmailSender, AuthMessageSender>();
            services.AddScoped<ISmsSender, AuthMessageSender>();
            services.AddSingleton<ITwilioSmsSender, TwilioSmsSender>();
            services.AddScoped<CurrentTenantServices>();
            services.AddScoped<TenantFinderService>();
            services.AddScoped<ITraffkTenantFinder, TenantFinderService>();
            services.AddScoped<CurrentContextServices>();
            services.AddScoped<ICurrentUser, CurrentContextServices>();
            services.AddScoped<IPhiAuthorizer, PhiAuthorizer>();
            services.AddScoped<Resources.PortalResourceServiceBuilder>();
            services.AddScoped<Resources.PortalResourceService>();
            services.AddScoped<BlobStorageServices>();
            services.AddScoped<ConfigStringFormatter>();
            services.AddScoped<ITableauStatusService, TableauStatusService>();
            services.AddScoped<ITableauVisualServices, TableauVisualServices>();
            services.AddScoped<ITrustedTicketGetter, TrustedTicketGetter>();
            services.AddScoped<ITableauViewerService, TableauViewerService>();
            services.AddScoped<ITableauAdminService, TableauAdminService>();
            services.AddScoped<ITableauTenantFinder, TableauTenantFinder>();
            services.AddScoped<ITableauUserCredentials, CurrentContextServices>();
            services.AddScoped<ITableauAuthorizationService, TableauAuthorizationService>();

            services.AddScoped<IReportVisualService, ReportVisualService>();

            services.AddScoped<IBackgroundJobClient, TenantedBackgroundJobClient>();
            services.AddScoped<ITraffkRecurringJobManager, TenantedBackgroundJobClient>();

            services.AddScoped<OrchestraRxApiClient>();
            services.AddScoped<IRedisCache, RedisCache>();
            services.AddScoped<ISynchronizedRedisCache, SynchronizedRedisCache>();
            services.AddScoped<OrchestraApiService>();

            services.AddScoped<TableauTrustedTicketActionFilter>();

            services.Configure<HttpClientFactory.Config>(Configuration.GetSection(HttpClientFactory.Config.ConfigSectionName));
            services.AddScoped<IHttpClientFactory, HttpClientFactory>();
            services.AddScoped<ICorrelationIdFactory, CorrelationIdFactory>();
            services.AddScoped<ICorrelationIdFinder, CorrelationIdFinder>();

            services.AddScoped<ICacher, TraffkCache>();

            services.AddScoped<ILogger>(provider => Logger.ForContext<Startup>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Traffk", Version = "v1" });
                c.TagActionsBy(api =>
                {
                    return api.ControllerAttributes().FirstOrDefault(a => a.GetType() == typeof(ApiControllerDisplayNameAttribute)).ToString();
                });
            });

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenProviderOptions:SecretKey").Value));

            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenProviderOptions:Issuer").Value,
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenProviderOptions:Audience").Value,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Audience = Configuration.GetSection("TokenProviderOptions:Audience").Value;
                options.ClaimsIssuer = Configuration.GetSection("TokenProviderOptions:Issuer").Value;
                options.TokenValidationParameters = tokenValidationParameters;
                options.SaveToken = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

#if(!DEBUG)
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog(Logger);
#endif
            
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

            app.UseStaticFiles();

            //Obsolete in .NET Core 2.0
            //app.UseIdentity();

            app.UseAuthentication();

            if (RequireHttps)
            {
                app.UseCookiePolicy(new CookiePolicyOptions { Secure = CookieSecurePolicy.Always });
            }

            app.UseSession();
            
            UserEnricher.Initialize(app.ApplicationServices);

            //Needs to be after app.Authentication()
            app.UseMiddleware<TokenAuthenticationMiddleware>();
            app.UseCorrelationId();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Traffk");
            });
        }
    }
}
