using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Settings;

namespace TraffkPortal.Models.TenantModels
{
    public class SmtpSettingsModel : SmtpOptions
    {
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(SmtpPassword), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmSmtpPassword { get; set; }

        public SmtpSettingsModel()
        { }

        public SmtpSettingsModel(SmtpOptions settings)
            : base(settings)
        {
            ConfirmSmtpPassword = SmtpPassword;
        }
    }
}
