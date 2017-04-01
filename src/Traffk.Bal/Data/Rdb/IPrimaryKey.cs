namespace Traffk.Bal.Data.Rdb
{
    interface IPrimaryKey
    {
        object Key { get; }
    }

    interface IPrimaryKey<TKey> : IPrimaryKey
    {
        new TKey Key { get; }
    }
}
