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
        }

        public string TrustedUrl => Url + "/trusted/";
        public string RestApiUrl => Url + "/#/";
        public string RestApiTrustedUrl => Url + "/#/trusted/";
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
