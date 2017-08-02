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

        public TableauSignInOptions(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
            Host = url.Replace("https://", "");
        }

        public string TrustedUrl => Url + "/trusted/";
        public string RestApiUrl => Url + "/#/";
        public string RestApiTrustedUrl => Url + "/#/trusted/";
        public string Host { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

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
