using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Traffk.BackgroundJobServer;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Email;
using Traffk.Bal.Settings;

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
