using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.MessagingModels
{
    public class MessageTemplateModel
    {
        public string ModelType { get; set; }

        public string SubjectCode { get; set; }

        [DataType(DataType.Html)]
        public string HtmlBodyCode { get; set; }

        public string TextBodyCode { get; set; }

        [DataType(DataType.Url)]
        public IList<AssetPreviewModel> AttachmentAssets { get; set; } = new List<AssetPreviewModel>();

        public IList<IFormFile> NewAttachments { get; set; } = new List<IFormFile>();

        public MessageTemplateModel()
        { }

        public MessageTemplateModel(MessageTemplate mt)
        {
            ModelType = mt?.SubjectTemplate?.ModelType;
            SubjectCode = mt?.SubjectTemplate?.Code;
            HtmlBodyCode = mt?.HtmlBodyTemplate?.Code;
            TextBodyCode = mt?.TextBodyTemplate?.Code;
        }
    }
}
