using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Settings;

namespace Traffk.Portal.Models.TenantModels
{
    public class FiscalYearSettingsModel : FiscalYearSettings
    {
        public FiscalYearSettingsModel()
        { }

        public FiscalYearSettingsModel(FiscalYearSettings other) : base(other)
        {
            
        }
    }
}
