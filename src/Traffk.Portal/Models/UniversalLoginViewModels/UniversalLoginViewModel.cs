using System.ComponentModel.DataAnnotations;

namespace Traffk.Portal.Models.UniversalLoginViewModels
{
    public class UniversalLoginViewModel
    {
        [Required]
        [Display(Name = "Company Name")]
        public string TenantName { get; set; }
    }
}
