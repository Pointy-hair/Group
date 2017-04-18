using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core.Database;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Traffk.Bal.Data.Rdb
{
    public class TenantRdbContext : DbContext
    {
        public TenantRdbContext(DbContextOptions<TenantRdbContext> options)
            : base(options)
        { }

        public async Task<ConnectionHelpers.Result<AppHostItem>> AppFindByHostname(string hostName = null, AppTypes? appType = null)
        {
            var ps = new SqlParameter[]
                {
                    new SqlParameter("@hostName", hostName==null ? DBNull.Value:(object) hostName){Direction=ParameterDirection.Input},
                    new SqlParameter("@appType", appType==null ? DBNull.Value:(object) appType.ToString()){Direction=ParameterDirection.Input},
                };
            var conn = Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            return await conn.ExecuteReaderAsync<AppHostItem>(null, "dbo.AppFindByHostname", null, ps);
        }
    }
}
