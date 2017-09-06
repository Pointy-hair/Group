﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RevolutionaryStuff.Core.Caching;
using Serilog;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.ExternalApis;
using Traffk.Bal.ReportVisuals;
using Traffk.Tableau.REST;
using TraffkPortal;
using TraffkPortal.Controllers;
using TraffkPortal.Services;
using Traffk.Bal.Caches;

namespace Traffk.Portal.Controllers
{
    public class PingdomController : BasePageController
    {
        protected readonly ITableauStatusService TableauStatusService;
        protected readonly OrchestraApiService OrchestraApiService;
        protected readonly IRedisCache Redis;

        public PingdomController( 
            TraffkTenantModelDbContext db, 
            CurrentContextServices current, 
            ILogger logger,
            ITableauStatusService tableauStatusService,
            OrchestraApiService orchestraApiService,
            IRedisCache redis,
            ICacher cacher = null) 
            : base(AspHelpers.MainNavigationPageKeys.NotSpecified, db, current, logger, cacher)
        {
            TableauStatusService = tableauStatusService;
            OrchestraApiService = orchestraApiService;
            Redis = redis;
        }

        [Route("/ping")]
        public IActionResult Index()
        {
            try
            {
                var contact = Rdb.Contacts.FirstOrDefault();
                var isTableauOnline = TableauStatusService.IsOnline;
                var pharmacy = OrchestraApiService.PharmacyTestSearch();
                var isRedisOnline = Redis.Connection.IsConnected;

                if (contact != null && isTableauOnline && isRedisOnline && pharmacy != null)
                {
                    return Content("Success");
                }
            }
            catch
            {

            }

            return Content("Fail");
        }
    }
}