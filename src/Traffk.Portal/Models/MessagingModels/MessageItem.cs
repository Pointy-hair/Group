using System;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.MessagingModels
{
    public class MessageItem
    {
        public int MessageTemplateId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        public string MessageTemplateTitle { get; set; }

        public bool HasSubject => SubjectTemplateId.HasValue;

        public int? SubjectTemplateId { get; set; }

        public string SubjectModelType { get; set; }

        public bool HasHtmlBody => HtmlBodyTemplateId.HasValue;

        public int? HtmlBodyTemplateId { get; set; }

        public string HtmlBodyModelType { get; set; }

        public bool HasTextBody => TextBodyTemplateId.HasValue;

        public int? TextBodyTemplateId { get; set; }

        public string TextBodyModelType { get; set; }

        public MessageItem() { }

        public MessageItem(MessageTemplate item)
        {
            MessageTemplateId = item.MessageTemplateId;
        }
    }
}
