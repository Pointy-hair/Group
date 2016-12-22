using System;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentCollectionAttribute : Attribute
    {
        public readonly string DatabaseName;
        public readonly string CollectionName;
        public DocumentCollectionAttribute(string databaseName, string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }
    }
}
