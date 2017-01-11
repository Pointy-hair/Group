using Newtonsoft.Json;

namespace Traffk.Bal.Data
{
    public interface IAddress
    {
        [JsonProperty("addressLine1")]
        string AddressLine1 { get; set; }

        [JsonProperty("addressLine2")]
        string AddressLine2 { get; set; }

        [JsonProperty("city")]
        string City { get; set; }

        [JsonProperty("state")]
        string State { get; set; }

        [JsonProperty("postalCode")]
        string PostalCode { get; set; }

        [JsonProperty("country")]
        string Country { get; set; }
    }
}
