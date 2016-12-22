using RevolutionaryStuff.Core;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    public enum ResourceTypes
    {
        [EnumeratedStringValue("")]
        Account,
        [EnumeratedStringValue("dbs")]
        Database,
        [EnumeratedStringValue("users")]
        User,
        [EnumeratedStringValue("permissions")]
        Permission,
        [EnumeratedStringValue("sprocs")]
        StoredProcedure,
        [EnumeratedStringValue("triggers")]
        Trigger,
        [EnumeratedStringValue("colls")]
        Collection,
        [EnumeratedStringValue("docs")]
        Document,
        [EnumeratedStringValue("attachments")]
        Attachments,
        [EnumeratedStringValue("offers")]
        Offer,
        [EnumeratedStringValue("docs")]
        Query,
    }
}
