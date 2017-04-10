using System.Collections.Generic;

namespace Traffk.Tableau.REST.Models
{
    /// <summary>
    /// Object Allows the setting of the Data Connections
    /// </summary>
    public interface IEditDataConnectionsSet
    {
        void SetDataConnections(IEnumerable<SiteConnection> connections);
    }
}