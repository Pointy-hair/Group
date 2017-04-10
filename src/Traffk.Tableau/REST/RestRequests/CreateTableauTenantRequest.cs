using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Tableau.REST.RestRequests
{
    public class CreateTableauTenantRequest
    {
        public string TenantName { get; }
        public string MasterDatabaseUserName { get; }
        public string MasterDatabasePassword { get; }
        public string NewDatabaseServerAddress { get; }
        public string NewDatabaseUserName { get; }
        public string NewDatabasePassword { get; }
        public string TemporaryFilePath { get; }

        public CreateTableauTenantRequest(string tenantName,
            string masterDatabaseUserName,
            string masterDatabasePassword,
            string newDatabaseServerAddress,
            string newDatabaseUserName,
            string newDatabasePassword,
            string temporaryFilePath)
        {
            TenantName = tenantName;
            MasterDatabaseUserName = masterDatabaseUserName;
            MasterDatabasePassword = masterDatabasePassword;
            NewDatabaseServerAddress = newDatabaseServerAddress;
            NewDatabaseUserName = newDatabaseUserName;
            NewDatabasePassword = newDatabasePassword;
            TemporaryFilePath = temporaryFilePath;
        }
    }
}
