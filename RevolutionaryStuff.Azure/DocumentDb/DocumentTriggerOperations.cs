using System;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    [Flags]
    public enum DocumentTriggerOperations
    {
        Create = 1,
        Replace = 2,
        Delete = 4,
        All = Create | Replace | Delete,
    }
}
