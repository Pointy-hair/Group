﻿using System;
using System.Xml;

namespace Traffk.Tableau.REST.Models
{
    /// <summary>
    /// Information about a Site in Server
    /// </summary>
    public class SiteInfo
    {
        public readonly string Id;
        public readonly string Name;
        public string TenantId => IdentifierForUrl;
        public readonly string IdentifierForUrl;
        public readonly string AdminMode;
        public readonly string State;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        public SiteInfo(XmlNode content)
        {
            if(content.Name.ToLower() != "site")
            {
                //AppDiagnostics.Assert(false, "Not a site");
                throw new Exception("Unexpected content - not site");
            }

            this.Name = content.Attributes["name"].Value;
            this.Id = content.Attributes["id"].Value;
            this.IdentifierForUrl = content.Attributes["contentUrl"].Value;
            this.AdminMode = content.Attributes["adminMode"].Value;
            this.State = content.Attributes["state"].Value;
        }
    }
}
