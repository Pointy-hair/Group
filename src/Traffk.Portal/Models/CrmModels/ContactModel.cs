using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.CrmModels
{
    public class ContactModel
    {
        public Contact Contact { get; set; }

        public CommunicationPiece[] CommunicationPieces { get; set; } = CommunicationPiece.None;

        public CommunicationOpting[] Optings { get; set; }

        public ContactModel() { }

        public ContactModel(Contact contact, IEnumerable<CommunicationPiece> communicationPieces = null, IEnumerable <CommunicationOpting> optings=null)
        {
            if (contact == null) return;
            Contact = contact;
            CommunicationPieces = communicationPieces?.ToArray() ?? CommunicationPiece.None;
            Optings = optings?.ToArray() ?? CommunicationOpting.None;
        }
    }
}
