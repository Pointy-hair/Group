using System;

namespace Traffk.Bal
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CommunicationModelAttribute : Attribute
    {
        public CommunicationModels Model { get; }

        public CommunicationModelAttribute(CommunicationModels model)
        {
            Model = model;
        }
    }
}
