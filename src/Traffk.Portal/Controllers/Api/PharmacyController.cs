using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Traffk.Bal.ExternalApis;
using Traffk.Bal.Permissions;
using Traffk.Orchestra.Models;
using Traffk.Portal.Permissions;

namespace Traffk.Portal.Controllers.Api
{
    [Authorize]
    [ApiAuthorize(ApiNames.Base)]
    [ApiAuthorize(ApiNames.Rx)]
    [Produces("application/json")]
    [Route("api/v1")]
    public class PharmacyController : BaseApiController
    {
        private readonly OrchestraApiService OrchestraApiService;

        public PharmacyController(ILogger logger, OrchestraApiService orchestraApiService) : base(logger)
        {
            OrchestraApiService = orchestraApiService;
        }

        [HttpGet]
        [Route("Pharmacies")]
        public IEnumerable<Pharmacy> GetPharmacies()
        {
            return null;
        }

        [HttpGet]
        [Route("Pharmacies/{zip}/{radius}")]
        public PharmacyResponse SearchPharmacies(string zip, int radius)
        {
            Log();

            return OrchestraApiService.PharmacySearch(zip, radius);
        }

        [HttpGet]
        [Route("Drugs")]
        public DrugResponse SearchDrugs(string query)
        {
            Log();

            return OrchestraApiService.DrugSearch(query);
        }

        public class Pharmacy
        {
            [DataMember(Name = "Name")]
            public string PharmacyName { get; set; }

            [DataMember(Name = "PharmacyId")]
            public string Id { get; set; }
        }
    }
}