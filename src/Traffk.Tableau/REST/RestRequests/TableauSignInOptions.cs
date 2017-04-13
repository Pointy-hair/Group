using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Tableau.REST.RestRequests
{
    public class TableauSignInOptions
    {
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
            Url = RestApiUrl + "site/" + tenantId;
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
        public string BaseUrl
        {
            get
            {
                if (Url.Contains("site"))
                {
                    return Url.Split(new string[] { @"/site/" }, StringSplitOptions.None)[0];
                }
                else
                {
                    return Url;
                }
            }
        }

    }
}
