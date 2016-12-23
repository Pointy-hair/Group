using System;

namespace RevolutionaryStuff.PowerBiToys
{
    public class PowerBiWebApplicationOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientID { get; set; }
        public string ClientSecretKey { get; set; }
        public Uri RedirectUrl { get; set; }
    }
}
