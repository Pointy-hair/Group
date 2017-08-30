using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Caching;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Bal.Services;

namespace TraffkPortal.Services.TenantServices
{
    public class TenantFinderService : ITraffkTenantFinder, IEnumerable<KeyValuePair<string, object>>
    {
        public const string HttpContextTenantIdKey = "TenantId";

        public class Config
        {
            public const string ConfigSectionName = "TenantServiceFinderConfig";
            public string Hostname { get; set; }
            public string DefaultTenantFinderHostPattern { get; set; }
            public Config()
            {

            }
        }

        public override string ToString() => $"{base.ToString()}: tenantId={TenantId}, preferredHostname=[{PreferredHostname}], actualHostname=[{ActualHostname}]";

        Task<int> ITenantFinder<int>.GetTenantIdAsync() => Task.FromResult(TenantId);

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            yield return new KeyValuePair<string, object>(ConfigStringFormatter.CommonTerms.DatabaseName, this.DatabaseName);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => CollectionHelpers.GetEnumerator(this);

        public int TenantId
        {
            get
            {
                if (TenantId_p < 1) throw new TenantFinderServiceException(TenantServiceExceptionCodes.TenantNotFound, $"host=[{PreferredHostname}]");
                return TenantId_p;
            }
        }

        private int TenantId_p = 0;

        public string ActualHostname { get; private set; }

        public string PreferredHostname { get; private set; }

        public string DatabaseName { get; private set; }

        public TenantFinderService(IOptions<Config> configOptions, ICacher cacher, TraffkTenantShardsDbContext db, IHttpContextAccessor acc)
        {
            var config = configOptions.Value;
            PreferredHostname = ActualHostname = StringHelpers.Coalesce(config?.Hostname, acc.HttpContext?.Request?.Host.Host);
            var defaultTenantFinderHostPattern = config.DefaultTenantFinderHostPattern;
            var defaultHostNamePattern = defaultTenantFinderHostPattern.Replace(@"{0}", @"([^\.]*)");
            var defaultHostNameRegexPattern = new Regex(defaultHostNamePattern, RegexOptions.Compiled);

            AppHostItem z = null;

            if (defaultHostNameRegexPattern.IsMatch(ActualHostname))
            {
                var m = defaultHostNameRegexPattern.Match(ActualHostname);
                var loginDomain = m.Success ? m.Groups[1].Value : null;

                z = cacher.FindOrCreateValWithSimpleKey(
                    Cache.CreateKey("LoginDomain", loginDomain),
                    () =>
                    {
                        var matchedTenant = db.AppFindByHostname(null, AppTypes.Portal, loginDomain).ExecuteSynchronously().FirstOrDefault();
                        if (matchedTenant == null) return null;
                        return new AppHostItem
                        {
                            TenantId = matchedTenant.TenantId,
                            PreferredHostname = ActualHostname,
                            HostDatabaseName = matchedTenant.HostDatabaseName
                        };
                    });
            }

            if (z == null)
            {
                z = cacher.FindOrCreateValWithSimpleKey(
                    Cache.CreateKey("ActualHostname", ActualHostname),
                    () => db.AppFindByHostname(ActualHostname, AppTypes.Portal).ExecuteSynchronously().FirstOrDefault()
                    );
            }
                
            if (z != null)
            {
                TenantId_p = z.TenantId;
                PreferredHostname = z.PreferredHostname;
                DatabaseName = z.HostDatabaseName;
            }

            acc.HttpContext.Items[HttpContextTenantIdKey] = TenantId_p;
        }
    }
}
