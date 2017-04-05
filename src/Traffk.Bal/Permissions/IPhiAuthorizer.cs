using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Bal.Permissions
{
    public interface IPhiAuthorizer
    {
        bool CanSeePhi { get; }
    }
}
