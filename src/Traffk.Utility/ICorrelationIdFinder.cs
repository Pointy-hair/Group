namespace Traffk.Utility
{
    public interface ICorrelationIdFinder
    {
        string Key { get; set; }
        string FindOrCreate();
    }
}
