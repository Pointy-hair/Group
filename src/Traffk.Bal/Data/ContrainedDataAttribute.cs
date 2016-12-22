using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Reflection;

namespace Traffk.Bal.Data
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Class)]
    public class ConstrainedDataAttribute : AppliesToPropertyAttribute
    {
        public ConstrainedDataAttribute(params string[] propertyNames)
            : base(propertyNames)
        { }

        public static bool IsContrained(PropertyInfo pi) => IsApplied<ConstrainedDataAttribute>(pi);
    }
}
