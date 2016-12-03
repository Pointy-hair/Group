using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Traffk.Bal.Email
{
    public interface IEmailer
    {
        Task SendEmailAsync(
            IEnumerable<MimeMessage> messages,
            Action<PreSendEventArgs> preSend = null,
            Action<PostSendEventArgs> postSend = null,
            object context = null);
    }
}
