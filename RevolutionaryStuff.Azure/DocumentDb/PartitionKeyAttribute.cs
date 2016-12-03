using System;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PartitionKeyAttribute : Attribute
    {
    }
}
