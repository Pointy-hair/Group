using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Traffk.Bal.ExternalApis;
using Traffk.Bal.Permissions;
using Traffk.Orchestra.Models;
using Traffk.Portal.Permissions;
using DrugResponse = Traffk.Bal.Data.ApiModels.Rx.DrugResponse;
using PharmacyResponse = Traffk.Bal.Data.ApiModels.Rx.PharmacyResponse;

namespace Traffk.Portal.Controllers.Api
{
    [Authorize]
    [ApiAuthorize(ApiNames.Base)]
    [ApiAuthorize(ApiNames.Rx)]
    [Produces("application/json")]
    [Route("api/v1")]
    public class OrchestraController : BaseApiController
    {
        private readonly OrchestraApiService OrchestraApiService;

        public OrchestraController(ILogger logger, OrchestraApiService orchestraApiService) : base(logger)
        {
            OrchestraApiService = orchestraApiService;
        }

        [HttpGet]
        [Route("Pharmacies")]
        public PharmacyResponse SearchPharmacies(string zip, int radius)
        {
            Log();

            return new PharmacyResponse(OrchestraApiService.PharmacySearch(zip, radius));
        }

        [HttpGet]
        [Route("Drugs")]
        public DrugResponse SearchDrugs(string query)
        {
            Log();

            return OrchestraApiService.DrugSearch(query);
        }

        [HttpGet]
        [Route("Drugs/{ndcReference}")]
        public DrugDetailResponse DrugDetail(string ndcReference)
        {
            Log();

            return OrchestraApiService.DrugDetail(ndcReference);
        }

        [HttpGet]
        [Route("Drugs/{ndcReference}/Alternative/{metricQuantity}/{daysOfSupply}")]
        public DrugAlternativeResponse DrugAlternative(string ndcReference, double metricQuantity, double daysOfSupply)
        {
            Log();

            var query = new DrugAlternativeSearchQuery
            {
                DaysOfSupply = daysOfSupply,
                MetricQuantity = metricQuantity,
                NDC = ndcReference
            };
            return OrchestraApiService.DrugAlternativeSingleSearch(query);
        }

        [HttpGet]
        [Route("Drugs/Alternatives/{drugId}")]
        public DrugDetailResponse DrugDosageAlternatives(string drugId)
        {
            Log();

            return OrchestraApiService.DrugDosageAlternatives(drugId);
        }

        [HttpGet]
        [Route("Plans")]
        public IEnumerable<Plan> GetPlans()
        {
            var plans = new List<Plan>
            {
                new Plan {Name = "Test Plan"}
            };

            Log();

            return plans;
        }

        public class Plan
        {
            public string Name { get; set; }
        }
    }
}