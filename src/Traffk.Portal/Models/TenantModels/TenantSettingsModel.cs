using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Portal.Models.TenantModels
{
    public class TenantSettingsModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Sender Address")]
        public string SenderAddress { get; set; }

        [Required]
        [DisplayName("Sender Name")]
        public string SenderName { get; set; }

        [Required]
        [DisplayName("Login Domain")]
        public string LoginDomain { get; set; }

        [Required]
        [DisplayName("Tenant Name")]
        public string TenantName { get; set; }

        [DisplayName("Requires Email Validation")]
        public bool RequiresEmailAccountValidation { get; set; }

        [DisplayName("Requires 2 Factor Authentication")]
        public bool RequiresTwoFactorAuthentication { get; set; }

        [DisplayName("Email Hosts Able To View PHI")]
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

        [DisplayName("Parent ID")]
        public int? ParentTenantId { get; set; }

        [DisplayName("Tenant ID")]
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
