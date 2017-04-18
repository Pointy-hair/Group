using Microsoft.AspNetCore.Hosting;
using RevolutionaryStuff.Core;
using System.Collections.Generic;

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
            s = Replace(s, Env as IEnumerable<KeyValuePair<string, object>>);
            s = Replace(s, Finder as IEnumerable<KeyValuePair<string, object>>);
            return s;
        }

        private string Replace(string s, IEnumerable<KeyValuePair<string, object>> kvps)
        {
            if (kvps != null)
            {
                foreach (var kvp in kvps)
                {
                    s = s.Replace("{" + kvp.Key + "}", Stuff.ToString(kvp.Value));
                }
            }
            return s;
        }
    }
}
