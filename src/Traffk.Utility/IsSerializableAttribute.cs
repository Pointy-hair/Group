using System;

namespace Traffk.Utility
{
    public class IsSerializableAttribute : Attribute
    {
        public bool IsSerializable { get; private set; }

        public IsSerializableAttribute(bool isSerializable)
        {
            IsSerializable = isSerializable;
        }
    }
}
