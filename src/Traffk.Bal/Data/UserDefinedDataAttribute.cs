using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Reflection;

namespace Traffk.Bal.Data
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class UserDefinedDataAttribute : AppliesToPropertyAttribute
    {
        public UserDefinedDataAttribute(params string[] propertyNames)
            : base(propertyNames)
        { }

        public static bool IsUserDefined(PropertyInfo pi) => IsApplied<UserDefinedDataAttribute>(pi);
    }
}
