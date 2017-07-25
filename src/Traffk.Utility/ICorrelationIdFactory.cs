using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Utility
{
    public interface ICorrelationIdFactory
    {
        string Key { get; set; }
        string Create();
    }
}
