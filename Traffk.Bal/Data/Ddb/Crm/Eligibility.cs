#if false
using Newtonsoft.Json;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public partial class Eligibility
    {
        [JsonIgnore]
        [HideInPortal]
        public Address Address => new Address
        {
            AddressLine1 = mbr_street_1,
            AddressLine2 = mbr_street_2,
            State = mbr_state,
            Country = mbr_county,
            PostalCode = mbr_zip,
            City = mbr_city
        };
    }
}
#endif