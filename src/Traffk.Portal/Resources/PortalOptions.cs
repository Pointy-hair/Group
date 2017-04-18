using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using Traffk.Bal.Settings;
using TraffkPortal.Services;

namespace TraffkPortal.Resources
{
    public class PortalResourceServiceBuilder : IOptions<PortalOptions>
    {
        private readonly IOptions<PortalOptions> Options;
        private readonly CurrentContextServices CurrentContextServices;

        public PortalOptions Value
        {
            get
            {
                if (Value_p == null)
                {
                    var builder = new ConfigurationBuilder();
                    builder.AddObject(Options.Value, nameof(PortalOptions), true);
                    builder.AddObject(CurrentContextServices.Application.AppSettings.PortalOptions, nameof(PortalOptions), true);
                    var u = CurrentContextServices.User;
                    if (u != null && u.Settings != null)
                    {
                        builder.AddObject(u.Settings.PortalOptions, nameof(PortalOptions), true);
                    }
                    var root = builder.Build();
//                    Value_p = root.GetValue<PortalResourceServiceOptions>("");
//                    Value_p = root.GetValue<PortalOptions>(nameof(PortalOptions));
                    Value_p = root.Get<PortalOptions>(nameof(PortalOptions));
                }
                return Value_p;
            }
        }

        private PortalOptions Value_p;

        public PortalResourceServiceBuilder(CurrentContextServices currentContextServices, IOptions<PortalOptions> options)
        {
            CurrentContextServices = currentContextServices;
            Options = options;
        }
    }

    public class PortalResourceService
    {
        private readonly PortalResourceServiceBuilder OptionsBuilder;
        private readonly CurrentContextServices CurrentContextServices;

        private PortalOptions O
        {
            get { return OptionsBuilder.Value; }
        }

        public PortalResourceService(PortalResourceServiceBuilder optionsBuilder, CurrentContextServices currentContextServices)
        {
            OptionsBuilder = optionsBuilder;
            CurrentContextServices = currentContextServices;

        }

        public HtmlString RegisterMessage { get { return new HtmlString(O.RegisterMessage); } }

        public HtmlString LoginMessage { get { return new HtmlString(O.LoginMessage); } }

        public HtmlString HomeMessage { get { return new HtmlString(O.HomeMessage); } }

        public HtmlString CopyrightMessage { get { return new HtmlString(O.CopyrightMessage); } }

        public HtmlString AboutMessage { get { return new HtmlString(O.AboutMessage); } }

        public HtmlString SupportMessage { get { return new HtmlString(O.SupportMessage);} }

        public Uri LogoLink { get { return O.LogoLink; } }

        public Uri FaviconLink { get { return O.FaviconLink; } }

        public Uri CssLink { get { return O.CssLink; } }

        public bool HasBasicColors => !string.IsNullOrEmpty(PrimaryColor) && !string.IsNullOrEmpty(SecondaryColor) && PrimaryColor!=SecondaryColor;

        public string PrimaryColor { get { return O.PrimaryColor; } }

        public string SecondaryColor { get { return O.SecondaryColor; } }

        public Uri JavascriptLink { get { return O.JavascriptLink; } }

        public string SystemAdminName { get { return O.SystemAdminName; } }

        public string SystemAdminEmailAddress { get { return O.SystemAdminEmailAddress; } }

        private IDictionary<string, ReusableValue> ReusableValueByKey_p;

        private IDictionary<string, ReusableValue> ReusableValueByKey
        {
            get
            {
                if (ReusableValueByKey_p == null)
                {
                    ReusableValueByKey_p = new Dictionary<string, ReusableValue>();
                    CurrentContextServices.Tenant.TenantSettings.ReusableValues.ForEach(z => ReusableValueByKey_p[z.Key] = z);
                    CurrentContextServices.Application.AppSettings.ResourceValues.ForEach(z => ReusableValueByKey_p[z.Key] = z);
                }
                return ReusableValueByKey_p;
            }
        }

        public object this[string key]
        {
            get
            {
                var rv = ReusableValueByKey[key];
                if (rv == null) return null;
                switch (rv.ResourceType)
                {
                    case ReusableValueTypes.Html:
                        return new HtmlString(rv.Value);
                    default:
                        return rv.Value;
                }
            }
        }
    }
}
