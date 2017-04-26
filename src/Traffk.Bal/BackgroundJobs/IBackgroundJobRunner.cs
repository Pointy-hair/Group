using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.BackgroundJobs
{
    public interface IBackgroundJobRunner
    {
        void ConvertFiscalYear(Job job);
    }
}