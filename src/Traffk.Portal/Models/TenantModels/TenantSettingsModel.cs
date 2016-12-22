using System.ComponentModel.DataAnnotations;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.TenantModels
{
    public class TenantSettingsModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string SenderAddress { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string LoginDomain { get; set; }

        [Required]
        public string TenantName { get; set; }

        public bool RequiresEmailAccountValidation { get; set; }

        public bool RequiresTwoFactorAuthentication { get; set; }

        public string ProtectedHealthInformationViewableByEmailAddressHostnamesString { get; set; }

        public string[] ProtectedHealthInformationViewableByEmailAddressHostnames
        {
            get
            {
                return ProtectedHealthInformationViewableByEmailAddressHostnamesString.ToArrayFromHumanDelineatedString(true);
            }
            set
            {
                ProtectedHealthInformationViewableByEmailAddressHostnamesString = value.ToHumanSeparatedEntryString();
            }
        }

        public int? ParentTenantId { get; set; }

        public int TenantId { get; set; }

        public TenantSettingsModel() { }

        public TenantSettingsModel(Tenant tenant)
        {
            SenderAddress = tenant.TenantSettings.EmailSenderAddress;
            SenderName = tenant.TenantSettings.EmailSenderName;
            ProtectedHealthInformationViewableByEmailAddressHostnames = tenant.TenantSettings.ProtectedHealthInformationViewableByEmailAddressHostnames;
            LoginDomain = tenant.LoginDomain;
            TenantName = tenant.TenantName;
            ParentTenantId = tenant.ParentTenantId;
            TenantId = tenant.TenantId;
            RequiresEmailAccountValidation = tenant.TenantSettings.RequiresEmailAccountValidation;
            RequiresTwoFactorAuthentication = tenant.TenantSettings.RequiresTwoFactorAuthentication;
        }
    }
}
