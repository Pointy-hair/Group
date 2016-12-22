using Newtonsoft.Json;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public class Address
    {
        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        public Address() { }

        public Address(Address other)
        {
            AddressLine1 = other.AddressLine1;
            AddressLine2 = other.AddressLine2;
            City = other.City;
            State = other.State;
            PostalCode = other.PostalCode;
            Country = other.Country;
        }

        public override string ToString() => $"{AddressLine1} {AddressLine2}; {City}, {State} {PostalCode}; {Country}";
    }
}
