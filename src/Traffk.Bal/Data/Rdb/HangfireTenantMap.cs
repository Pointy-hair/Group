using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data.Rdb
{
    [Table("HangfireTenantMap", Schema = "dbo")]
    public partial class HangfireTenantMap : IRdbDataEntity, ITraffkTenanted, IPrimaryKey<int>
    {
        public static readonly HangfireTenantMap[] None = new HangfireTenantMap[0];

        object IPrimaryKey.Key { get { return HangfireTenantMapId; } }

        int IPrimaryKey<int>.Key { get { return HangfireTenantMapId; } }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("HangfireTenantMapId")]
        public int HangfireTenantMapId { get; set; }

        /// <summary>
        /// Foreign key to the tenant that owns this account
        /// </summary>
        [Description("Foreign key to the tenant that owns this account")]
        [DisplayName("Tenant Id")]
        [Column("TenantId")]
        public int TenantId { get; set; }

        [Description("Foreign key to the hangfire job id")]
        [DisplayName("Job Id")]
        [Column("JobId")]
        public int JobId { get; set; }
    }
}
