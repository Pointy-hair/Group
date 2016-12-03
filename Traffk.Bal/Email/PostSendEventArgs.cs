using MimeKit;
using System;

namespace Traffk.Bal.Email
{
    public class PostSendEventArgs : EventArgs
    {
        public MimeMessage Message { get; }
        public object Context { get; }
        public int MessageNumber { get; }
        public Exception SendException { get; }
        public bool CancelledDuringPresend { get; }
        public TimeSpan SendDuration { get; }


        public PostSendEventArgs(MimeMessage message, object context, int number, TimeSpan sendDuration, bool cancelledDuringPresend, Exception ex)
        {
            Message = message;
            Context = context;
            MessageNumber = number;
            SendDuration = sendDuration;
            SendException = ex;
            CancelledDuringPresend = cancelledDuringPresend;
        }
    }
}
