using Microsoft.AspNetCore.Hosting;
using RevolutionaryStuff.Core;

namespace Traffk.Bal.Services
{
    public class ConfigStringFormatter
    {
        private readonly IHostingEnvironment Env;
        private readonly ITraffkTenantFinder Finder;

        public ConfigStringFormatter(IHostingEnvironment env, ITraffkTenantFinder finder)
        {
            Env = env;
            Finder = finder;
        }

        public string Transform(string s)
        {
            if (s == null) return null;
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            s = s.Replace("{TENANTID}", tenantId.ToString());
            s = s.Replace("{ENV}", Env.EnvironmentName);
            return s;
        }
    }
}
