using Microsoft.Extensions.Configuration;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.Caching;
using System.Linq;
using Traffk.Bal.Data.Rdb;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Threading.Tasks;
using Traffk.Bal;

namespace TraffkPortal.Services.TenantServices
{
    public class TenantFinderService : ITraffkTenantFinder
    {
        public const string HttpContextTenantIdKey = "TenantId";

        public class TenantServiceFinderOptions
        {
            public int? TenantId { get; set; }
            public Dictionary<string, int> TenantIdByHostname { get; } = new Dictionary<string, int>(Comparers.CaseInsensitiveStringComparer);

            public TenantServiceFinderOptions()
            {

            }
        }

        public override string ToString() => $"{base.ToString()}: tenantId={TenantId}, preferredHostname=[{PreferredHostname}], actualHostname=[{ActualHostname}]";

        Task<int> ITenantFinder<int>.GetTenantIdAsync() => Task.FromResult(TenantId);

        private static readonly ICache<string, ApplicationHostItem> HostMapCache = CachingServices.Instance.CreateSynchronized<string, ApplicationHostItem>();

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

        public TenantFinderService(IOptions<TenantServiceFinderOptions> options, TenantRdbContext db, IHttpContextAccessor acc)
        {
            PreferredHostname = ActualHostname = acc.HttpContext==null ? null : acc.HttpContext.Request.Host.Host;
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
                var z = HostMapCache.Do(ActualHostname, () =>
                {
                    var res = db.ApplicationFindByHostAsync(ActualHostname, ApplicationTypes.Portal).ExecuteSynchronously();
                    return res.FirstOrDefault();
                });
                if (z != null)
                {
                    TenantId_p = z.TenantId;
                    PreferredHostname = z.PreferredHostname;
                }
            }
            acc.HttpContext.Items[HttpContextTenantIdKey] = TenantId_p;
        }
    }
}
