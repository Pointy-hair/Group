using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Traffk.Bal.Services;
using System.Collections.Generic;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Identity
{
    public class TraffkPasswordValidator : PasswordValidator<ApplicationUser>
    {
        private readonly CurrentTenantServices Cts;

        public TraffkPasswordValidator(CurrentTenantServices cts, IdentityErrorDescriber errors = null)
            : base(errors)
        {
            Cts = cts;
        }

        public async override Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            var res = await base.ValidateAsync(manager, user, password);
            var t = await Cts.GetTenantAsync();
            bool hasNewError = false;
            var errors = new List<IdentityError>(res.Errors);
            if (t.TenantSettings.Password.IsPasswordProhibited(password))
            {
                errors.Add(new IdentityError { Code = "ProhibitedPassword", Description = "That password was on the prohibited list" });
                hasNewError = true;
            }
            if (t.TenantSettings.Password.PasswordContainsUnallowedWord(password))
            {
                errors.Add(new IdentityError { Code = "UnallowedWord", Description = "That password contains a word that was unallowed" });
                hasNewError = true;
            }
            return hasNewError ? IdentityResult.Failed(errors.ToArray()) : res;
        }
    }
}
