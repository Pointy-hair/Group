using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.CrmModels
{
    public class ContactModel
    {
        public Contact Contact { get; set; }

        public CommunicationLog[] CommunicationLogs { get; set; }

        public CommunicationOpting[] Optings { get; set; }

        public ContactModel() { }

        public ContactModel(Contact contact, IEnumerable<CommunicationLog> communicationLogs = null, IEnumerable < CommunicationOpting> optings=null)
        {
            if (contact == null) return;
            Contact = contact;
            CommunicationLogs = communicationLogs?.ToArray() ?? CommunicationLog.None;
            Optings = optings?.ToArray() ?? CommunicationOpting.None;
        }
    }
}
