using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Storage;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Portal.Models.ReportingModels
{
    public class ScheduledReportViewModel
    {
        public Communication Communication { get; set; }
        public RecurringJobDto RecurringJob { get; set; }
    }
}
