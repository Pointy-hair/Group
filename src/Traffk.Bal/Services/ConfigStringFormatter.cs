using Microsoft.AspNetCore.Hosting;
using RevolutionaryStuff.Core;
using System.Collections.Generic;

namespace Traffk.Bal.Services
{
    public class ConfigStringFormatter
    {
        public static class CommonTerms
        {
            public const string DatabaseName = "DATABASENAME";
            public const string TenantId = "TENANTID";
            public const string Env = "ENV";

            public static string ToStringReplacementToken(string term)
                => "{" + term + "}";
        }

        private readonly IHostingEnvironment Env;
        private readonly ITraffkTenantFinder Finder;

        public ConfigStringFormatter(ITraffkTenantFinder finder, IHostingEnvironment env)
            : this(finder)
        {
            Env = env;
        }

        public ConfigStringFormatter(ITraffkTenantFinder finder)
        {
            Finder = finder;
        }

        public string Transform(string s)
        {
            if (s == null) return null;
            var tenantId = Finder.GetTenantIdAsync().ExecuteSynchronously();
            if (Env != null)
            {
                s = s.Replace(CommonTerms.ToStringReplacementToken(CommonTerms.Env), Env.EnvironmentName);
                s = Replace(s, Env as IEnumerable<KeyValuePair<string, object>>);
            }
            s = s.Replace(CommonTerms.ToStringReplacementToken(CommonTerms.TenantId), tenantId.ToString());
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
