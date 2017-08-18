using Microsoft.AspNetCore.Identity;

namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
{
    public partial class RoleClaim : IdentityRoleClaim<string>
    {
        public RoleClaim(IdentityRoleClaim<string> other=null, bool copyKey = false)
        {
            if (other != null)
            {
                if (copyKey)
                {
                    Id = other.Id;
                }
                ClaimType = other.ClaimType;
                ClaimValue = other.ClaimValue;
                RoleId = other.RoleId;
            }
        }
    }
}
