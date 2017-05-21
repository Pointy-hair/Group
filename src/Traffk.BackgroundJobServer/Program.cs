using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using Traffk.BackgroundJobServer;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Email;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;

namespace Traffk.BackgroundJobRunner
{
    public class Program : JobRunnerProgram
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.Configure<ActiveDirectoryApplicationIdentificationSettings>(Configuration.GetSection("ActiveDirectoryApplicationIdentificationOptions"));

            services.AddSingleton(Cache.DataCacher);
            services.AddSingleton<IVault, Vault>();
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<ITrackingEmailer, TrackingEmailer>();
            services.AddScoped<ITenantJobs, TenantedJobRunner>();
            services.AddScoped<IGlobalJobs, GlobalJobRunner>();
            services.AddScoped<IDataSourceSyncJobs, DataSourceSyncRunner>();
        }

        public static void Main(string[] args) => JobRunnerProgram.Main<Program>(args);
    }
}
