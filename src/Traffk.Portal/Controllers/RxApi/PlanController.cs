using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Traffk.Bal.Permissions;
using Traffk.Portal.Permissions;

namespace Traffk.Portal.Controllers
{
    [Authorize]
    [ApiAuthorize(ApiNames.Base)]
    [ApiAuthorize(ApiNames.Rx)]
    [Produces("application/json")]
    [Route("api/v1")]
    public class PlanController : Controller
    {
        [HttpGet]
        [Route("Plans")]
        public IEnumerable<Plan> GetPlans()
        {
            var plans = new List<Plan>
            {
                new Plan {Name = "Test Plan"}
            };

            return plans;
        }
    }

    public class Plan
    {
        public string Name { get; set; }
    }
}