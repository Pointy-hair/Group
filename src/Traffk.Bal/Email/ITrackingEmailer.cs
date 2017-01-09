using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Email
{
    public interface ITrackingEmailer
    {
        Task SendEmailAsync(
            Creative creative,
            IEnumerable<MimeMessage> messages,
            Action<PreSendEventArgs> preSend = null,
            Action<PostSendEventArgs> postSend = null,
            object context = null);
    }
}
