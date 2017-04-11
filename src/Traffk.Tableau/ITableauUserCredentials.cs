using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Tableau
{
    public interface ITableauUserCredentials
    {
        string UserName { get; }
        string Password { get; }
    }
}
