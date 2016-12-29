using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.CommunicationModels
{
    public class CommunicationCreativeModel : Creative
    {
        public int CommunicationId { get; set; }

        public CommunicationCreativeModel() { }

        public CommunicationCreativeModel(int communicationId, Creative creative)
            : base(creative)
        {
            CommunicationId = communicationId;
        }
    }
}
