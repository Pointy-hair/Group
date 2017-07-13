using Hangfire;
using System.Threading.Tasks;

namespace Traffk.Bal.BackgroundJobs
{
    [Queue(BackgroundJobHelpers.QueueNames.DataSyncQueue)]
    public interface IDataSourceSyncJobs
    {
        Task DataSourceFetchAsync(int dataSourceId);
    }
}
