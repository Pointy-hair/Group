using System;
using System.Collections.Generic;
using System.Text;

namespace Traffk.Orchestra.Models
{
    public class MedicarePlan
    {
        
    }

    public class Plan
    {
        public string ContractID { get; set; }
        public string ID { get; set; }
        public string PlanID { get; set; }
        public int PlanYear { get; set; }
        public string SegmentID { get; set; }
        public string ClientPlanID { get; set; }
    }
}
