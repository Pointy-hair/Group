using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Bal.BackgroundJobs
{
    public interface IGlobalJobs
    {
        void DataSourceFetch(int dataSourceId);
    }
}
