﻿using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace TraffkPortal.Models.CrmModels
{
    public class ContactModel
    {
        public Contact Contact { get; set; }

        public CommunicationPiece[] CommunicationPieces { get; set; } = CommunicationPiece.None;

        public IList<Note> Notes { get; set; } = new List<Note>();

//        public CommunicationOpting[] Optings { get; set; }

        public ContactModel() { }

        public ContactModel(Contact contact, IEnumerable<CommunicationPiece> communicationPieces = null)//, IEnumerable <CommunicationOpting> optings=null)
        {
            if (contact == null) return;
            Contact = contact;
            CommunicationPieces = communicationPieces?.ToArray() ?? CommunicationPiece.None;
//            Optings = optings?.ToArray() ?? CommunicationOpting.None;
        }
    }
}
