using Hangfire;
using System.Threading.Tasks;

namespace Traffk.Bal.BackgroundJobs
{
    [Queue(BackgroundJobHelpers.QueueNames.SsisQueue)]
    public interface IEtlJobs
    {
        Task LoadCmsGovAsync();
        Task LoadInternationalClassificationDiseasesAsync();
    }
}
