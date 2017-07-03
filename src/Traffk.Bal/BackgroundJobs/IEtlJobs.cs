using Hangfire;
using System.Threading.Tasks;

namespace Traffk.Bal.BackgroundJobs
{
    [Queue(BackgroundJobHelpers.QueueNames.SsisQueue)]
    public interface IEtlJobs
    {
        [Queue(BackgroundJobHelpers.QueueNames.SsisQueue)]
        Task LoadInternationalClassificationDiseasesAsync();
    }
}
