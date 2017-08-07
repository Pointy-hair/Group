using Microsoft.AspNetCore.Builder;

namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
{
    public partial class Tenant
    {
        public enum TenantTypes
        {
            Normal,
            Globals,
            Placeholder,
        }

        public static class StatusNames
        {
            public const string Normal = "Normal";
        }

        public void Configure(IdentityOptions options)
        {
            options.Password.RequireDigit = TenantSettings.Password.RequireDigit;
            options.Password.RequiredLength = TenantSettings.Password.RequiredLength;
            options.Password.RequireLowercase = TenantSettings.Password.RequireLowercase;
            options.Password.RequireNonAlphanumeric = TenantSettings.Password.RequireNonAlphanumeric;
            options.Password.RequireUppercase = TenantSettings.Password.RequireUppercase;
        }

        partial void OnTenantSettingsDeserialized()
        {
            TenantSettings.EnsureMemberClasses();
        }
    }
}
