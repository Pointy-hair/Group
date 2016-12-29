using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Linq;
using Traffk.Bal;

namespace TraffkPortal.Models.ApplicationModels
{
    public class SystemCommunicationsModel
    {
        public SystemCommunicationPurposes Purpose { get; set; }

        public Traffk.Bal.CommunicationModelTypes ModelType { get; set; }

        public int CommunicationId { get; set; }

        public SystemCommunicationsModel() { }

        public SystemCommunicationsModel(SystemCommunicationPurposes purpose, int communicationId, Traffk.Bal.CommunicationModelTypes modelType)
        {
            Purpose = purpose;
            CommunicationId = communicationId;
            ModelType = modelType;
        }

        public static IList<SystemCommunicationsModel> Create(IEnumerable<KeyValuePair<SystemCommunicationPurposes, int>> items)
        {
            var d = Stuff.GetEnumValues<SystemCommunicationPurposes>().ToDictionary(e => e, e => new SystemCommunicationsModel(e, 0, AttributeStuff.GetCustomAttribute<CommunicationModelAttribute>(e).Model));
            foreach (var item in items)
            {
                d[item.Key].CommunicationId = item.Value;
            }
            return d.Values.OrderBy(item => item.Purpose).ToList();
        }
    }
}
