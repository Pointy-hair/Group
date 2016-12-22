using System;

namespace Traffk.Bal.Data
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class SampleDataAttribute : Attribute
    {
        public readonly string Data;

        public SampleDataAttribute(string data)
        {
            Data = data;
        }
    }
}
