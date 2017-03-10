using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RevolutionaryStuff.Core.Caching;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Models.ErrorModels;
using TraffkPortal.Services;

namespace TraffkPortal.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BasePageController
    {
        private IHostingEnvironment HostingEnvironment_p;

        public ErrorController(
            TraffkRdbContext db,
            CurrentContextServices current,
            ILoggerFactory loggerFactory,
            IHostingEnvironment hostingEnvironment
        )
            : base(AspHelpers.MainNavigationPageKeys.Main, db, current, loggerFactory)
        {
            HostingEnvironment_p = hostingEnvironment;
        }

        [Route("/Error")]
        public IActionResult Index()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var errorModel = new ErrorModel(HttpContext.Response.StatusCode.ToString());
            if (HostingEnvironment_p.IsDevelopment())
            {
                errorModel.Type = exception.GetType().Name;
                errorModel.Message = exception.Error.Message;
                errorModel.StackTrace = exception.Error.StackTrace;
            }
            return View(errorModel);
        }
    }
}