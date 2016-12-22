using System.ComponentModel.DataAnnotations;
using Traffk.Bal;
using Traffk.Bal.Settings;

namespace TraffkPortal.Models.ApplicationModels
{
    public class RegistrationModel
    {
        [Display(Name = "Can users self register?")]
        public bool UsersCanSelfRegister { get; set; }

        [Display(Name = "List of allowable email hostnames for self registration (space delineated)")]
        public string SelfRegistrationMandatoryEmailAddressHostnamesString { get; set; }

        public string[] SelfRegistrationMandatoryEmailAddressHostnames
        {
            get
            {
                return SelfRegistrationMandatoryEmailAddressHostnamesString.ToArrayFromHumanDelineatedString(true);
            }
            set
            {
                SelfRegistrationMandatoryEmailAddressHostnamesString = value.ToHumanSeparatedEntryString();
            }
        }

        public RegistrationModel() { }
        public RegistrationModel(RegistrationSettings settings)
        {
            if (settings == null) return;
            UsersCanSelfRegister = settings.UsersCanSelfRegister;
            SelfRegistrationMandatoryEmailAddressHostnames = settings.SelfRegistrationMandatoryEmailAddressHostnames;
        }
    }
}
