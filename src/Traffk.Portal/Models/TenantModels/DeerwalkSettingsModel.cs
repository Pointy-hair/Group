using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Settings;

namespace TraffkPortal.Models.TenantModels
{
    public class DeerwalkSettingsModel : DeerwalkSettings
    {
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(FtpPassword), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmFtpPassword { get; set; }

        public DeerwalkSettingsModel()
        { }

        public DeerwalkSettingsModel(DeerwalkSettings settings)
            : base(settings)
        {
            ConfirmFtpPassword = FtpPassword;
        }
    }
}
