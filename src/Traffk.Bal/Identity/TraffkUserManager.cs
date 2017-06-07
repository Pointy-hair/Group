using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Services;

namespace Traffk.Bal.Identity
{
    public class TraffkUserManager : UserManager<ApplicationUser>
    {
        private readonly CurrentTenantServices Cts;

        public TraffkUserManager(CurrentTenantServices cts, IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, new ConfigurableOptions<IdentityOptions>(optionsAccessor, cts), passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            Cts = cts;
        }
    }
}
