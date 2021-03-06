﻿namespace Traffk.Tableau.REST.Helpers
{
    internal partial class DatasourcePublishSettings
    {
        /// <summary>
        /// The name of the owner of the content (NULL if unknown)
        /// </summary>
        public readonly string OwnerName;

        /// <summary>
        /// Constructor
        /// </summary>
        public DatasourcePublishSettings(string ownerName)
        {
            this.OwnerName = ownerName;
        }
    }
}
