﻿using System;
using System.Xml;

namespace Traffk.Tableau.REST.Models
{
    /// <summary>
    /// Information about a User in a Server's site
    /// </summary>
    public class SiteUser : IHasSiteItemId
    {
        public readonly string Name;
        public readonly string Id;
        public readonly string SiteRole;
        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userNode"></param>
        public SiteUser(XmlNode userNode)
        {
            if (userNode.Name.ToLower() != "user")
            {
                throw new Exception("Unexpected content - not user");
            }

            this.Id = userNode.Attributes["id"].Value;
            this.Name = userNode.Attributes["name"].Value;
            this.SiteRole = userNode.Attributes["siteRole"].Value;
        }

        public override string ToString()
        {
            return "User: " + this.Name + "/" + this.Id + "/" + this.SiteRole;
        }

        string IHasSiteItemId.Id
        {
            get { return this.Id; }
        }
    }
}
