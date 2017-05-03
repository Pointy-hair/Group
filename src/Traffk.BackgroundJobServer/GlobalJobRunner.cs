using System;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.BackgroundJobServer
{
    public class GlobalJobRunner : IGlobalJobs
    {
        private readonly ITableauAdminService TableauAdminService;

        public GlobalJobRunner(ITableauAdminService tableauAdminService)
        {
            TableauAdminService = tableauAdminService;
        }

        void IGlobalJobs.MigrateTableauDataset(TableauDataMigrationRequest request)
        {
            TableauAdminService.MigrateDataset(request);
        }
    }
}
