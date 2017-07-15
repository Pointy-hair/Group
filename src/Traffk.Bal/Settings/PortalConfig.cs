using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class PortalConfig
    {
        public const string ConfigSectionName = "PortalConfig";

        [DisplayName("Register Page Message")]
        [DataType(DataType.Html)]
        public string RegisterMessage { get; set; }

        [DisplayName("Login Page Message")]
        [DataType(DataType.Html)]
        public string LoginMessage { get; set; }

        [DisplayName("Home Page Message")]
        [DataType(DataType.Html)]
        public string HomeMessage { get; set; }

        [DisplayName("Copyright Message")]
        [DataType(DataType.Html)]
        public string CopyrightMessage { get; set; }

        [DisplayName("About Page Message")]
        [DataType(DataType.Html)]
        public string AboutMessage { get; set; }

        [DisplayName("Support Page Message")]
        [DataType(DataType.Html)]
        public string SupportMessage { get; set; }

        [DataType(DataType.Url)]
        public Uri CssLink { get; set; }

        [DataType(DataType.Url)]
        public Uri JavascriptLink { get; set; }

        [DataType(DataType.ImageUrl)]
        public Uri LogoLink { get; set; }

        [DataType(DataType.ImageUrl)]
        public Uri FaviconLink { get; set; }

        [DisplayName("Primary Color")]
        [DataType(TraffkHelpers.CustomDataTypes.HtmlColor)]
        public string PrimaryColor { get; set; }

        [DisplayName("Secondary Color")]
        [DataType(TraffkHelpers.CustomDataTypes.HtmlColor)]
        public string SecondaryColor { get; set; }

        public string SystemAdminName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string SystemAdminEmailAddress { get; set; }

        public PortalConfig() { }

        public PortalConfig(PortalConfig other)
        {
            if (other == null) return;
            RegisterMessage = other.RegisterMessage;
            LoginMessage = other.LoginMessage;
            HomeMessage = other.HomeMessage;
            CopyrightMessage = other.CopyrightMessage;
            AboutMessage = other.AboutMessage;
            SupportMessage = other.SupportMessage;
            CssLink = other.CssLink;
            JavascriptLink = other.JavascriptLink;
            LogoLink = other.LogoLink;
            FaviconLink = other.FaviconLink;
            PrimaryColor = other.PrimaryColor;
            SecondaryColor = other.SecondaryColor;
            SystemAdminName = other.SystemAdminName;
            SystemAdminEmailAddress = other.SystemAdminEmailAddress;
        }
    }
}
