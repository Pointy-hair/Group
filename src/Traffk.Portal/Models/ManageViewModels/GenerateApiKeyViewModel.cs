using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Portal.Models.ManageViewModels
{
    public class GenerateApiKeyViewModel
    {
        [DisplayName("API Key")]
        public string ApiKey { get; set; }
    }
}
