using System;

namespace Traffk.Tableau.REST.RestRequests
{
    public class TableauSignInOptions
    {
        private const string SiteUrlPortion = @"/site/";

        public TableauSignInOptions()
        { }

        public TableauSignInOptions(string url)
        {
            Url = url;
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

    }
}
