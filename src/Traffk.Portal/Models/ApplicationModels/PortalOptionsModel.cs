using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using Traffk.Bal.Settings;

namespace TraffkPortal.Models.ApplicationModels
{
    public class PortalOptionsModel : PortalConfig
    {
        [DisplayName("CSS File")]
        public IFormFile CssFile { get; set; }

        [DisplayName("JavaScript File")]
        public IFormFile JavascriptFile { get; set; }

        [DisplayName("Logo File")]
        public IFormFile LogoFile { get; set; }

        [DisplayName("FavIcon File")]
        public IFormFile FaviconFile { get; set; }

        public PortalOptionsModel() { }

        public PortalOptionsModel(PortalConfig po)
            : base(po)
        { }
    }
}
