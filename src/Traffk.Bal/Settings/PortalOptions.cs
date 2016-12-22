using System;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class PortalOptions
    {        
        [DataType(DataType.Html)]
        public string RegisterMessage { get; set; }

        [DataType(DataType.Html)]
        public string LoginMessage { get; set; }

        [DataType(DataType.Html)]
        public string HomeMessage { get; set; }

        [DataType(DataType.Html)]
        public string CopyrightMessage { get; set; }

        [DataType(DataType.Html)]
        public string AboutMessage { get; set; }

        [DataType(DataType.Url)]
        public Uri CssLink { get; set; }

        [DataType(DataType.Url)]
        public Uri JavascriptLink { get; set; }

        [DataType(DataType.ImageUrl)]
        public Uri LogoLink { get; set; }

        [DataType(DataType.ImageUrl)]
        public Uri FaviconLink { get; set; }

        [DataType(TraffkHelpers.CustomDataTypes.HtmlColor)]
        public string PrimaryColor { get; set; }

        [DataType(TraffkHelpers.CustomDataTypes.HtmlColor)]
        public string SecondaryColor { get; set; }

        public string SystemAdminName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string SystemAdminEmailAddress { get; set; }

        public PortalOptions() { }

        public PortalOptions(PortalOptions other)
        {
            if (other == null) return;
            RegisterMessage = other.RegisterMessage;
            LoginMessage = other.LoginMessage;
            HomeMessage = other.HomeMessage;
            CopyrightMessage = other.CopyrightMessage;
            AboutMessage = other.AboutMessage;
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
