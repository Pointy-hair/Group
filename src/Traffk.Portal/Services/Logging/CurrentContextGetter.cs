using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Services;

namespace TraffkPortal.Services.Logging
{
    public static class CurrentContextGetter
    {
        private static ICurrentUser _currentContext;

        public static void Configure(ICurrentUser currentContextServices)
        {
            _currentContext = currentContextServices;
        }

        public static ICurrentUser CurrentContext => _currentContext;
    }
}
