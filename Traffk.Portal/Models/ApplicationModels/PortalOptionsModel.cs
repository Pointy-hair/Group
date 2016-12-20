using Microsoft.AspNetCore.Http;
using Traffk.Bal.Settings;

namespace TraffkPortal.Models.ApplicationModels
{
    public class PortalOptionsModel : PortalOptions
    {
        public IFormFile CssFile { get; set; }
        public IFormFile JavascriptFile { get; set; }
        public IFormFile LogoFile { get; set; }
        public IFormFile FaviconFile { get; set; }

        public PortalOptionsModel() { }

        public PortalOptionsModel(PortalOptions po)
            : base(po)
        { }
    }
}
