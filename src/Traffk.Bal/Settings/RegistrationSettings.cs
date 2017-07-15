using Newtonsoft.Json;

namespace Traffk.Bal.Settings
{
    public class RegistrationSettings
    {
        [JsonProperty("UsersCanSelfRegister")]
        public bool UsersCanSelfRegister { get; set; }

        [JsonProperty("SelfRegistrationMandatoryEmailAddressHostnames")]
        public string[] SelfRegistrationMandatoryEmailAddressHostnames { get; set; }
    }
}
