using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.Communications;

namespace TraffkPortal.Models.ApplicationModels
{
    public class SystemCommunicationsModel
    {
        public SystemCommunicationPurposes Purpose { get; set; }

        public CommunicationModelTypes ModelType { get; set; }

        public int CreativeId { get; set; }

        public SystemCommunicationsModel() { }

        public SystemCommunicationsModel(SystemCommunicationPurposes purpose, int creativeId, CommunicationModelTypes modelType)
        {
            Purpose = purpose;
            CreativeId = creativeId;
            ModelType = modelType;
        }

        public static IList<SystemCommunicationsModel> Create(IEnumerable<KeyValuePair<SystemCommunicationPurposes, int>> items)
        {
            var d = Stuff.GetEnumValues<SystemCommunicationPurposes>().ToDictionary(e => e, e => new SystemCommunicationsModel(e, 0, AttributeStuff.GetCustomAttribute<CommunicationModelAttribute>(e).Model));
            foreach (var item in items)
            {
                d[item.Key].CreativeId = item.Value;
            }
            return d.Values.OrderBy(item => item.Purpose).ToList();
        }
    }
}
