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

        public const string Name = "Error";
        public static class ActionNames
        {
            public const string Index = "Index";
        }

        public static class ErrorKeys
        {
            public const string StatusCode = "ExceptionStatusCode";
            public const string Type = "ExceptionType";
            public const string Message = "ExceptionMessage";
            public const string StackTrace = "ExceptionStackTrace";
        }
        
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

        [Route("/E")]
        public IActionResult E()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewData[ErrorKeys.StatusCode] = HttpContext.Response.StatusCode;

            if (HostingEnvironment_p.IsDevelopment() && exception != null)
            {
                SetException(exception.Error);
            }

            return RedirectToAction(ActionNames.Index, ErrorController.Name);
        }

        [Route("/Error")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index()
        {
            var errorModel = new ErrorModel(ViewData[ErrorKeys.StatusCode]?.ToString() ?? "");
            errorModel.Message = ViewData[ErrorKeys.Message]?.ToString() ?? "";
            errorModel.Type = ViewData[ErrorKeys.Type]?.ToString() ?? "";
            errorModel.StackTrace = ViewData[ErrorKeys.StackTrace]?.ToString() ?? "";
            return View(errorModel);
        }

        [Route("/Te")]
        public IActionResult Te()
        {
            throw new NotImplementedException();
        }
    }
}