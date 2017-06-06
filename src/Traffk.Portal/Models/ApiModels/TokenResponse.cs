using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Portal.Models.ApiModels
{
    public class TokenResponse
    {
        public string access_token { get; set; }

        public string token_type => "Bearer";

        public int expires_in { get; set; }

        public string resource { get; set; }
    }
}
