using System;
using Traffk.Bal.Data.Ddb.Crm;

namespace TraffkPortal.Models.CrmModels
{
    public class RelatedContactModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Related Related { get; set; }
        public string RelatedContactFullName { get; set; }
        public string RelatedContactType { get; set; }
    }
}
