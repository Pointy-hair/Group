using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Traffk.Bal.Permissions;
using Traffk.Orchestra;
using Traffk.Orchestra.Models;
using Traffk.Portal.Permissions;
using Traffk.Portal.Services;

namespace Traffk.Portal.Controllers.Api
{
    [Authorize]
    [ApiAuthorize(ApiNames.Base)]
    [ApiAuthorize(ApiNames.Rx)]
    [Produces("application/json")]
    [Route("api/v1")]
    public class PharmacyController : BaseApiController
    {
        private readonly OrchestraRxApiClient OrchestraRxApiClient;
        private readonly RedisCachingServices Cache;

        public PharmacyController(OrchestraRxApiClient orchestraRxApiClient, ILogger logger) : base(logger)
        {
            Cache = new RedisCachingServices();
            OrchestraRxApiClient = orchestraRxApiClient;
        }

        [HttpGet]
        [Route("Pharmacies")]
        public IEnumerable<Pharmacy> GetPharmacies()
        {
            OrchestraRxApiClient.Authenticate();

            return null;

            //const string cacheKey = "pharmacyList";

            //IDatabase cache = Cache.Connection.GetDatabase();
            //string cachedPharmacyList = cache.StringGet(cacheKey);

            //var pharmacies = new List<Pharmacy>();

            //if (!string.IsNullOrEmpty(cachedPharmacyList))
            //{
            //    pharmacies = JsonConvert.DeserializeObject<List<Pharmacy>>(cachedPharmacyList);
            //}
            //else
            //{
            //    pharmacies = new List<Pharmacy>
            //    {
            //        new Pharmacy {PharmacyName = "Test Pharmacy" + DateTime.UtcNow}
            //    };
            //}

            //Log();

            //return pharmacies;
        }

        [HttpGet]
        [Route("Pharmacies/{zip}/{radius}")]
        public PharmacyResponse SearchPharmacies(string zip, int radius)
        {
            return OrchestraRxApiClient.PharmacySearch(zip, radius).Result;
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