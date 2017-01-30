using System.ComponentModel.DataAnnotations;

namespace TraffkPortal.Models.ManageViewModels
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [RegularExpression(@"^(\+?\d{0,1}\s?)?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "The phone number is not valid.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
