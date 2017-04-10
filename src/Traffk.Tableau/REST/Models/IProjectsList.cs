namespace Traffk.Tableau.REST.Models
{
    /// <summary>
    /// Questions everything that manages a set of projects needs to be able to answer
    /// </summary>
    public interface IProjectsList
    {
        SiteProject FindProjectWithId(string projectId);
        SiteProject FindProjectWithName(string projectName);
    }
}
