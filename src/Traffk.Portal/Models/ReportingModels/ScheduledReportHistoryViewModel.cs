using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Portal.Models.ReportingModels
{
    public class ScheduledReportHistoryViewModel
    {
        public Communication Communication { get; set; }
        public IEnumerable<Job> Jobs { get; set; }

    }
}
