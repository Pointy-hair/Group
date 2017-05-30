using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.BackgroundJobs
{
    public class JobStateDetails
    {
        [NotMapped]
        [DisplayName("Parent Job ID")]
        public int ParentId { get; set; }

        public static JobStateDetails CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<JobStateDetails>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
