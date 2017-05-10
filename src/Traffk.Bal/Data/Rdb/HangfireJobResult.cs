using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data.Rdb
{
    public class HangfireJobResult : IRdbDataEntity, IPrimaryKey<int>
    {
        int IPrimaryKey<int>.Key { get { return JobId; } }

        object IPrimaryKey.Key { get { return JobId; } }

        [Key]
        [Description("Foreign key to the hangfire job id")]
        [DisplayName("Job Id")]
        [Column("Id")]
        public int JobId { get; set; }

        [Column("JobResultDetails")]
        public string JobResultDetailsJson { get; set; }
    }
}
