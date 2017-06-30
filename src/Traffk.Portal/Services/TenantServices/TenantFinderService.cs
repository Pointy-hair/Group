using System;
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

        public class TenantServiceFinderOptions
        {
            public int? TenantId { get; set; }
            public Dictionary<string, int> TenantIdByHostname { get; } = new Dictionary<string, int>(Comparers.CaseInsensitiveStringComparer);
            public string DefaultTenantFinderHostPattern { get; set; }
            public TenantServiceFinderOptions()
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

        private static readonly ICache<string, AppHostItem> HostMapCache = CachingServices.Instance.CreateSynchronized<string, AppHostItem>();

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

        public TenantFinderService(IOptions<TenantServiceFinderOptions> options, TraffkTenantShardsDbContext db, IHttpContextAccessor acc)
        {
            PreferredHostname = ActualHostname = acc.HttpContext == null ? null : acc.HttpContext.Request.Host.Host;
            var tso = options.Value;
            if (tso != null)
            {
                if (tso.TenantId.HasValue)
                {
                    TenantId_p = tso.TenantId.Value;
                }
                else if (tso.TenantIdByHostname != null && acc.HttpContext != null)
                {
                    TenantId_p = tso.TenantIdByHostname.GetValue(PreferredHostname, TenantId_p);
                }
            }
            if (TenantId_p == 0)
            {
                var defaultTenantFinderHostPattern = tso.DefaultTenantFinderHostPattern;
                var defaultHostNamePattern = defaultTenantFinderHostPattern.Replace(@"{0}", @"([^\.]*)");
                var defaultHostNameRegexPattern = new Regex(defaultHostNamePattern, RegexOptions.Compiled);

                AppHostItem z = null;

                if (defaultHostNameRegexPattern.IsMatch(ActualHostname))
                {
                    var m = defaultHostNameRegexPattern.Match(ActualHostname);
                    var loginDomain = m.Success ? m.Groups[1].Value : null;

                    var matchedTenant = db.Tenants.SingleOrDefault(x => StringHelpers.IsSameIgnoreCase(x.LoginDomain, loginDomain));
                    if (matchedTenant != null)
                    {
                        z = HostMapCache.Do(ActualHostname, () =>
                        {
                            var app = new AppHostItem
                            {
                                TenantId = matchedTenant.TenantId,
                                PreferredHostname = ActualHostname,
                                HostDatabaseName = matchedTenant.HostDatabaseName
                            };
                            return app;
                        });
                    }
                }

                if (z == null)
                {
                    z = HostMapCache.Do(ActualHostname, () =>
                    {
                        var res = db.AppFindByHostname(ActualHostname, AppTypes.Portal).ExecuteSynchronously().FirstOrDefault();
                        return res;
                    });
                }
                
                if (z != null)
                {
                    TenantId_p = z.TenantId;
                    PreferredHostname = z.PreferredHostname;
                    DatabaseName = z.HostDatabaseName;
                }
            }
            acc.HttpContext.Items[HttpContextTenantIdKey] = TenantId_p;
        }
    }
}
