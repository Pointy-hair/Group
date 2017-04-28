using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Bal.BackgroundJobs
{
    public interface IBackgroundJobTenantFinder : ITraffkTenantFinder
    {
        int GetTenantIdUsingJobId(string jobId);
    }
}
