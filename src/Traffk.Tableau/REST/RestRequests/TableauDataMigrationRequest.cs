namespace Traffk.Tableau.REST.RestRequests
{
    public class TableauDataMigrationRequest
    {
        public string DestTableauTenantId { get; }
        public string DestDbServerAddress { get; }
        public string DestDbName { get; }
        public string DestDbUsername { get; }
        public string DestDbPassword { get; }
        public string TempFilePath { get; }

        public TableauDataMigrationRequest(string destTableauTenantId, string destDbServerAddress, string destDbName, string destDbUsername,
            string destDbPassword, string tempFilePath)
        {
            DestTableauTenantId = destTableauTenantId;
            DestDbServerAddress = destDbServerAddress;
            DestDbName = destDbName;
            DestDbUsername = destDbUsername;
            DestDbPassword = destDbPassword;
            TempFilePath = tempFilePath;
        }

        public TableauDataMigrationRequest(CreateTableauTenantRequest createTableauTenantRequest)
        {
            DestTableauTenantId = createTableauTenantRequest.TenantName;
            DestDbServerAddress = createTableauTenantRequest.NewDatabaseServerAddress;
            DestDbName = createTableauTenantRequest.NewDatabaseName;
            DestDbUsername = createTableauTenantRequest.NewDatabaseUsername;
            DestDbPassword = createTableauTenantRequest.NewDatabasePassword;
            TempFilePath = createTableauTenantRequest.TemporaryFilePath;
        }
    }
}
