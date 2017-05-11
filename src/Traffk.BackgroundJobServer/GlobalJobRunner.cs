using System;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.BackgroundJobServer
{
    public class GlobalJobRunner : IGlobalJobs
    {
        private readonly TraffkGlobalsContext Gdb;

        public GlobalJobRunner(TraffkGlobalsContext gdb)
        {
            Gdb = gdb;
        }

        void IGlobalJobs.DataSourceFetch(int dataSourceId)
        {
            throw new NotImplementedException();
        }
    }
}
