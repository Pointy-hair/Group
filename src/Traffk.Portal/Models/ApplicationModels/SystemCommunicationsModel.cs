using RevolutionaryStuff.Core;
using System.Collections.Generic;
using Traffk.Bal;

namespace TraffkPortal.Models.ApplicationModels
{
    public class SystemCommunicationsModel
    {
        public SystemCommunicationPurposes Purpose { get; set; }

        public int CommunicationId { get; set; }

        public SystemCommunicationsModel() { }

        public SystemCommunicationsModel(SystemCommunicationPurposes purpose, int communicationId)
        {
            Purpose = purpose;
            CommunicationId = communicationId;
        }

        public static IList<SystemCommunicationsModel> Create(IEnumerable<KeyValuePair<SystemCommunicationPurposes, int>> items)
            => items.ConvertAll(i => new SystemCommunicationsModel(i.Key, i.Value));
    }
}
