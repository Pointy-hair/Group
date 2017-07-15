using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.ExternalApis;
using Traffk.Bal.Permissions;
using Traffk.Orchestra.Models;
using Traffk.Portal.Permissions;
using Traffk.Bal.Data.ApiModels.Rx;
using DrugResponse = Traffk.Bal.Data.ApiModels.Rx.DrugResponse;
using PharmacyResponse = Traffk.Bal.Data.ApiModels.Rx.PharmacyResponse;

namespace Traffk.Portal.Controllers.Api
{
    [Authorize]
    [ApiAuthorize(ApiNames.Base)]
    [ApiAuthorize(ApiNames.Rx)]
    [Produces("application/json")]
    [Route("api/v1")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiControllerDisplayName("Rx")]
    public class OrchestraController : BaseApiController
    {
        private readonly OrchestraApiService OrchestraApiService;

        public OrchestraController(ILogger logger, OrchestraApiService orchestraApiService) : base(logger)
        {
            OrchestraApiService = orchestraApiService;
        }

        [HttpGet]
        [Route("pharmacies")]
        public PharmacyResponse SearchPharmacies(string zip, int radius)
        {
            Log();

            return new PharmacyResponse(OrchestraApiService.PharmacySearch(zip, radius));
        }

        [HttpGet]
        [Route("pharmacies/nabp/{nabp}")]
        public Pharmacy PharmacyNABPDetail(string nabp)
        {
            Log();

            return new Pharmacy(OrchestraApiService.Pharmacy(nabp));
        }

        [HttpGet]
        [Route("pharmacies/{id}")]
        public Pharmacy PharmacyDetail(string id)
        {
            Log();

            id = id.Replace("OP", "");

            return new Pharmacy(OrchestraApiService.Pharmacy(id));
        }

        [HttpGet]
        [Route("drugs")]
        public DrugResponse SearchDrugs(string query)
        {
            Log();

            return OrchestraApiService.DrugSearch(query);
        }

        [HttpGet]
        [Route("drugs/ndc/{ndcReference}")]
        public Bal.Data.ApiModels.Rx.Drug DrugDetailNdc(string ndcReference)
        {
            Log();

            return OrchestraApiService.DrugDetail(ndcReference);
        }

        [HttpGet]
        [Route("drugs/{id}")]
        public Bal.Data.ApiModels.Rx.Drug DrugDetail(string id)
        {
            Log();

            return OrchestraApiService.DrugDetail(id);
        }

        [HttpGet]
        [Route("drugs/{ndcReference}/alternative/{metricQuantity}/{daysOfSupply}")]
        public DrugAlternativeResponse DrugAlternative(string ndcReference, double metricQuantity, double daysOfSupply)
        {
            Log();

            var queries = new List<DrugAlternativeSearchQuery>
            {
                new DrugAlternativeSearchQuery
                {
                    DaysOfSupply = daysOfSupply,
                    MetricQuantity = metricQuantity,
                    NDC = ndcReference
                }
            };
            
            var orchestraResponse = OrchestraApiService.DrugAlternativeSearch(queries);
            return new DrugAlternativeResponse(orchestraResponse.First());
        }

        [HttpGet]
        [Route("plans")]
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