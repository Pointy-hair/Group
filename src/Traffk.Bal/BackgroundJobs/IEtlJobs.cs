using Hangfire;
using System.Threading.Tasks;

namespace Traffk.Bal.BackgroundJobs
{
    /// <remarks>As hangfire serialized enumerations as ints, it is critical to number these so that a refactor does not screw up anything</remarks>
    public enum EtlPackages
    {
        CmsGov = 1,
        InternationalClassificationDiseases = 2,
        NationalDrugCodes = 4,
        ZipCodes = 3,
    }

    [Queue(BackgroundJobHelpers.QueueNames.SsisQueue)]
    public interface IEtlJobs
    {
        Task ExecuteAsync(EtlPackages etlPackage, int dataSourceFetchId);
    }
}
