﻿using System;

namespace Traffk.Bal.Data.Rdb
{
    public partial class Address : IAddress
    {
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
