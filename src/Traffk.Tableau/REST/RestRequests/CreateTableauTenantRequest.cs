using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Tableau.REST.RestRequests
{
    public class CreateTableauTenantRequest
    {
        public string TenantName { get; }
        public string SourceDatabaseUserName { get; }
        public string SourceDatabasePassword { get; }
        public string NewDatabaseServerAddress { get; }
        public string NewDatabaseName { get; }
        public string NewDatabaseUsername { get; }
        public string NewDatabasePassword { get; }
        public string TemporaryFilePath { get; }

        public CreateTableauTenantRequest(string tenantName,
            string sourceDatabaseUserName,
            string sourceDatabasePassword,
            string newDatabaseServerAddress,
            string newDatabaseName,
            string newDatabaseUsername,
            string newDatabasePassword,
            string temporaryFilePath)
        {
            TenantName = tenantName;
            SourceDatabaseUserName = sourceDatabaseUserName;
            SourceDatabasePassword = sourceDatabasePassword;
            NewDatabaseServerAddress = newDatabaseServerAddress;
            NewDatabaseName = newDatabaseName;
            NewDatabaseUsername = newDatabaseUsername;
            NewDatabasePassword = newDatabasePassword;
            TemporaryFilePath = temporaryFilePath;
        }
    }
}
