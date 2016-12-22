using System.Threading.Tasks;

namespace TraffkPortal.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
        Task SendSmsCommunicationAsync(string communicationPurpose, object model, string number);
    }
}
