using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Reflection;

namespace Traffk.Bal.Data
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class FreeFormDataAttribute : AppliesToPropertyAttribute
    {
        public FreeFormDataAttribute(params string[] propertyNames)
            : base(propertyNames)
        { }

        public static bool IsFreeForm(PropertyInfo pi) => IsApplied<ConstrainedDataAttribute>(pi);
    }
}
