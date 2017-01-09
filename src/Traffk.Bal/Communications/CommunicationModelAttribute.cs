using System;

namespace Traffk.Bal.Communications
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CommunicationModelAttribute : Attribute
    {
        public CommunicationModelTypes Model { get; }

        public CommunicationModelAttribute(CommunicationModelTypes model)
        {
            Model = model;
        }
    }
}
