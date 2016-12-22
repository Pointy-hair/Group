using System;

namespace Traffk.Bal
{
    /// <remarks>http://cphs.berkeley.edu/hipaa/hipaa18.html</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ProtectedHealthInformationIdentifierAttribute : Attribute
    {
    }
}
