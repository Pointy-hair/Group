using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Database;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Bal.Data.Rdb.TraffkTenantShards
{
    public partial class TraffkTenantShardsDbContext : DbContext
    {
        public const string DefaultDatabaseConnectionStringName = "TraffkTenantShards";

        public TraffkTenantShardsDbContext(DbContextOptions<TraffkTenantShardsDbContext> options)
            : base(options)
        { }

        public DbSet<Tenant> Tenants { get; set; } //dbo.Tenants

        public async Task<ConnectionHelpers.Result<AppHostItem>> AppFindByHostname(string hostName = null, TraffkTenantModel.AppTypes? appType = null, string loginDomain = null)
        {
            var ps = new SqlParameter[]
                {
                    new SqlParameter("@hostName", hostName==null ? DBNull.Value:(object) hostName){Direction=ParameterDirection.Input},
                    new SqlParameter("@appType", appType==null ? DBNull.Value:(object) appType.ToString()){Direction=ParameterDirection.Input},
                    new SqlParameter("@loginDomain", loginDomain==null ? DBNull.Value:(object) loginDomain){Direction=ParameterDirection.Input},
                };
            var conn = Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            return await conn.ExecuteReaderAsync<AppHostItem>(null, "dbo.AppFindByHostname", null, ps);
        }

        public async Task<ConnectionHelpers.Result<Tenant>> TenantFindByTenantId(int tenantId)
        {
            var ps = new SqlParameter[]
                {
                    new SqlParameter("@tenantId", tenantId){Direction=ParameterDirection.Input},
                };
            var conn = Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            return await conn.ExecuteReaderAsync<Tenant>(null, "dbo.TenantFindByTenantId", null, ps);
        }

        [Table("tenants", Schema = "dbo")]
        public class Tenant
        {
            [DisplayName("Tenant Id")]
            [Key]
            [Column("TenantId")]
            public int TenantId { get; set; }

            [DisplayName("Tenant Name")]
            [NotNull]
            [Required]
            [MaxLength(255)]
            [Column("TenantName")]
            public string TenantName { get; set; }

            [DisplayName("Login Domain")]
            [MaxLength(80)]
            [Column("LoginDomain")]
            public string LoginDomain { get; set; }

            [DisplayName("Host Database Name")]
            [MaxLength(128)]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Column("HostDatabaseName")]
            public string HostDatabaseName { get; set; }
        }

        public async Task<int> TenantIdReserveAsync(string hostDatabaseName)
        {
            var ps = new SqlParameter[]
                {
                    new SqlParameter("@hostDatabaseName", hostDatabaseName){Direction=ParameterDirection.Input},
                };
            var conn = Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            var res = await conn.ExecuteReaderAsync<int>(null, "dbo.TenantIdReserve", null, ps);
            return res.First();
        }
    }
}
