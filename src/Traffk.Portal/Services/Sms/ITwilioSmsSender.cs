using System.Threading.Tasks;

namespace TraffkPortal.Services.Sms
{
    public interface ITwilioSmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}