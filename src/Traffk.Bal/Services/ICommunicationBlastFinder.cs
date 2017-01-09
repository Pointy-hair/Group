using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Services
{
    public interface ICommunicationBlastFinder
    {
        Task<CommunicationBlast> FindAsync(Creative creative);
    }
}
