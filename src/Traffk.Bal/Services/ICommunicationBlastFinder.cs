using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Services
{
    public interface ICommunicationBlastFinder
    {
        Task<CommunicationBlast> FindAsync(Creative creative);
    }
}
