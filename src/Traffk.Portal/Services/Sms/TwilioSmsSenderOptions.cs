﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraffkPortal.Services.Sms
{
    public class TwilioSmsSenderOptions
    {
        public string SID { get; set; }
        public string AuthToken { get; set; }
        public string SendNumber { get; set; }
        public string BaseUri { get; set; }
        public string RequestUri { get; set; }
    }
}
