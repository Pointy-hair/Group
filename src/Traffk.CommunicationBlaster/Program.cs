using Traffk.Bal.Data.Rdb;
using Traffk.Bal.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Traffk.Bal.Email;
using Microsoft.Extensions.Options;
using Traffk.Bal.Settings;

namespace TraffkCommunicationBlastRunner
{
    public class Program : JobRunnerProgram<CommunicationBlastRunner>
    {
        protected override JobTypes JobType => JobTypes.CommunicationBlast;

        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<IEmailer, TrackingEmailer>();
        }

        public static void Main(string[] args) => JobRunnerProgram.Main<Program>(args);
    }
}
