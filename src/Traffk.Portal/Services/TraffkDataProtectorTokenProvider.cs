using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Traffk.Portal.Services
{
    public class TraffkDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser:class
    {
        public TraffkDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<TraffkDataProtectorTokenProviderOptions> options) : base(dataProtectionProvider, options)
        {
        }
    }

    public class TraffkDataProtectorTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public static string ConfirmationTokenProviderName = "ConfirmEmail";
    }
}
