using Traffk.Bal.Data.Rdb;
using Traffk.Bal.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Traffk.Bal.Email;
using Microsoft.Extensions.Options;
using Traffk.Bal.Settings;
using Traffk.Bal.BackgroundJobs;

namespace Traffk.BackgroundJobRunner
{
    public class Program : JobRunnerProgram
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<ITrackingEmailer, TrackingEmailer>();
            services.AddScoped<ITenantJobs, TenantedJobRunner>();
        }

        public static void Main(string[] args) => JobRunnerProgram.Main<Program>(args);
    }
}
