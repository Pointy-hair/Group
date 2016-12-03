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
        protected override string JobType => Job.JobTypes.CommunicationBlast;

        protected override void OnBuildServices(IServiceCollection services)
        {
            base.OnBuildServices(services);
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<IEmailer, TrackingEmailer>();
        }

        public static void Main(string[] args) => JobRunnerProgram.Main<Program>(args);
    }
}
