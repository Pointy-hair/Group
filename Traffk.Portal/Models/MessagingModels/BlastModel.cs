using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.MessagingModels
{
    public class BlastModel
    {
        public CommunicationBlast Blast { get; set; }

        public string ModelType { get; set; }

        public string SubjectCode { get; set; }

        [DataType(DataType.Html)]
        public string HtmlBodyCode { get; set; }

        public string TextBodyCode { get; set; }

        public BlastModel()
            : this(null)
        { }

        public BlastModel(CommunicationBlast blast)
        {
            Blast = blast ?? new CommunicationBlast();
            ModelType = Blast.MessageTemplate?.SubjectTemplate?.ModelType;
            SubjectCode = Blast.MessageTemplate?.SubjectTemplate?.Code;
            HtmlBodyCode = Blast.MessageTemplate?.HtmlBodyTemplate?.Code;
            TextBodyCode = Blast.MessageTemplate?.TextBodyTemplate?.Code;
        }
    }
}
