using System.ComponentModel.DataAnnotations;

namespace TraffkPortal.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
