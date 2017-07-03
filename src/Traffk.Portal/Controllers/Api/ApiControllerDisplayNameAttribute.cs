using System;

namespace Traffk.Portal.Controllers.Api
{
    public class ApiControllerDisplayNameAttribute : Attribute
    {
        public string GroupName { get; set; }

        public ApiControllerDisplayNameAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public override string ToString()
        {
            return GroupName;
        }
    }
}
