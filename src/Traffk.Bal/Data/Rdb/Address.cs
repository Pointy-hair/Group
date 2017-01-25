using System;
using Newtonsoft.Json;

namespace Traffk.Bal.Data.Rdb
{
    public partial class Address : IAddress
    {
        [JsonProperty("addressLine1")]
        string IAddress.AddressLine1
        {
            get { return Address1; }
            set { Address1 = value; }
        }

        string IAddress.AddressLine2
        {
            get { return Address2; }
            set { Address2 = value; }
        }

        string IAddress.Country
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
