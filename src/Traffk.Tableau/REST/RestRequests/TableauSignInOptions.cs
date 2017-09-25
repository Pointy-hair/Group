using System;
using RevolutionaryStuff.Core;

namespace Traffk.Tableau.REST.RestRequests
{
    public class TableauSignInOptions
    {
        public const string ConfigSectionName = "TableauSignInOptions";

        private const string SiteUrlPortion = @"/site/";

        public TableauSignInOptions()
        { }

        public TableauSignInOptions(string url)
        {
            Url = url;
            Requires.True(url.Contains("https"), nameof(url));
            Host = url.Replace("https://", "");
        }

        public void UpdateForTenant(string tenantId)
        {
            TenantId = tenantId;
            Url = BaseRestApiUrl + SiteUrlPortion + tenantId;
        }

        public string TrustedUrl => Url + "/trusted/";

        public string RestApiUrl
        {
            get
            {
                if (TenantId != null)
                {
                    return Url;
                }
                else
                {
                    return Url + "/#/";
                }
            }
        }
        public string Host { get; set; }
        public string Url { get; set; }
        public string TenantId { get; set; }
        public string BaseRestApiUrl
        {
            get
            {
                if (Url.Contains("site"))
                {
                    return Url.Split(new string[] { SiteUrlPortion }, StringSplitOptions.None)[0];
                }
                else
                {
                    return Url + @"/#";
                }
            }
        }

        public TimeSpan LoginCacheTimeout
        {
            get
            {
                if (LoginCacheTimeout_p == null)
                {
                    return TimeSpan.FromMinutes(5);
                }
                return LoginCacheTimeout_p.Value;
            }
            set { LoginCacheTimeout_p = value; }
        }

        private TimeSpan? LoginCacheTimeout_p;

        public TimeSpan ReportIndexCacheTimeout
        {
            get
            {
                if (ReportIndexCacheTimeout_p == null)
                {
                    return TimeSpan.FromMinutes(5);
                }
                return ReportIndexCacheTimeout_p.Value;
            }
            set { ReportIndexCacheTimeout_p = value; }
        }

        private TimeSpan? ReportIndexCacheTimeout_p;
    }
}
