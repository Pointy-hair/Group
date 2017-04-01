using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevolutionaryStuff.Core
{
    public static class Stuff
    {
        [Conditional("DEBUG")]
        public static void Noop(params object[] args)
        {
        }
    }
}
