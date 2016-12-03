using MimeKit;
using System.ComponentModel;

namespace Traffk.Bal.Email
{
    public class PreSendEventArgs : CancelEventArgs
    {
        public MimeMessage Message { get; }
        public object Context { get; }
        public int MessageNumber { get; }

        public PreSendEventArgs(MimeMessage message, object context, int number)
        {
            Message = message;
            Context = context;
            MessageNumber = number;
        }
    }
}
