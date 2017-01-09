using System.Threading.Tasks;
using Traffk.Bal.Communications;

namespace TraffkPortal.Services.Sms
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
        Task SendSmsCommunicationAsync(SystemCommunicationPurposes purpose, object model, string number);
    }
}
