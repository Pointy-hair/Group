using RevolutionaryStuff.Core;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    public enum RestApiVersions
    {
        [EnumeratedStringValue("2016-05-30")]
        V2016_05_30=6,
        [EnumeratedStringValue("2015-12-16")]
        V2015_12_16=5,
        [EnumeratedStringValue("2015-08-06")]
        V2015_08_06=4,
        [EnumeratedStringValue("2015-06-03")]
        V2015_06_03=3,
        [EnumeratedStringValue("2015-04-08")]
        V2015_04_08=2,
        [EnumeratedStringValue("2014-08-21")]
        V2014_08_21=1,

        [EnumeratedStringValue("2016-05-30")]
        Current = V2016_05_30,
        FirstParitionKeyVersion = V2015_12_16
    }
}
