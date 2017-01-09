using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data;

namespace Traffk.Bal.Settings
{
    public class CommunicationPieceData
    {
        public static CommunicationPieceData CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CommunicationPieceData>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        [JsonProperty("deliveryError")]
        public ExceptionError DeliveryError { get; set; }

        [JsonProperty("deliveryEndpoint")]
        public string DeliveryEndpoint { get; set; }
    }
}
