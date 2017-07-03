using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using Traffk.BackgroundJobServer;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Email;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;
using RevolutionaryStuff.Core.ApplicationParts;
using Microsoft.AspNetCore.Identity;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Microsoft.Extensions.Configuration;
using Traffk.Bal.Data.Rdb.TraffkTenantShardManager;

namespace Traffk.BackgroundJobRunner
{
    public class Program : JobRunnerProgram
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.Configure<ActiveDirectoryApplicationIdentificationSettings>(Configuration.GetSection("ActiveDirectoryApplicationIdentificationOptions"));
            services.Configure<TenantManagementJobsRunner.TenantManagementJobsRunnerConfiguration>(Configuration.GetSection("TenantManagementJobsRunnerConfiguration"));

            services.AddSingleton<IAsyncGetter<OpenIdConfiguration>>(new OpenIdConfigurationFinder(Configuration["AzureOpenIdConfigurationUrl"]));
            services.AddSingleton(Cache.DataCacher);
            services.AddScoped<ServiceClientCredentialFactory>();
            services.AddSingleton<IVault, Vault>();
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<ITrackingEmailer, TrackingEmailer>();
            services.AddScoped<ITenantJobs, TenantedJobRunner>();
            services.AddScoped<IGlobalJobs, GlobalJobRunner>();
            services.AddScoped<IEtlJobs, EtlJobRunner>();
            services.AddScoped<IDataSourceSyncJobs, DataSourceSyncRunner>();
            services.AddScoped<ITenantManagementJobs, TenantManagementJobsRunner>();
            services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
            services.AddDbContext<TraffkTenantShardManagerDbContext>((sp, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(TraffkTenantShardManagerDbContext.DefaultDatabaseConnectionStringName));
            }, ServiceLifetime.Scoped);
        }

        //BUGBUG: requires newtonsoft 9.0.1!  to prevent binder problem... yuck!  [Cannot get SerializationBinder because an ISerializationBinder was previously set.]
        //https://github.com/Azure/azure-sdk-for-net/issues/2552 -- requires newtonsoft 9.0.1!  to prevent binder problem... yuck!  [Cannot get SerializationBinder because an ISerializationBinder was previously set.]
        //https://stackoverflow.com/questions/35409905/how-do-i-create-an-azure-credential-that-will-give-access-to-the-websitemanageme
        //https://login.windows.net/traffk.onmicrosoft.com/.well-known/openid-configuration
        public static void Main(string[] args) => JobRunnerProgram.Main<Program>(args);
    }
}
