using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Serilog;
using Traffk.Tableau.REST.RestRequests;
using Traffk.Utility;

namespace Traffk.Tableau.REST
{
    public interface ITableauStatusService
    {
        bool IsOnline { get; }
    }

    public class TableauStatusService : TableauAdminService, ITableauStatusService
    {
        public TableauStatusService(IOptions<TableauSignInOptions> options, 
            IOptions<TableauAdminCredentials> adminCredentials, 
            IHttpClientFactory httpClientFactory, 
            ILogger logger) 
            : base(options, adminCredentials, httpClientFactory, logger)
        {
            var login = SignIn(Urls, TableauUserCredentials.UserName, TableauUserCredentials.Password, logger);
            IsSignedIn = login.IsSignedIn;
        }

        bool ITableauStatusService.IsOnline => IsSignedIn;

        private new TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, ILogger logger)
        {
            var l = new TableauServerSignIn(onlineUrls, userName, password, HttpClientFactory, logger);
            l.ExecuteRequest();
            return l;
        }
    }
}
