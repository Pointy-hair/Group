using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using Serilog;
using Traffk.Bal.Data;
using TraffkPortal.Models.ErrorModels;
using TraffkPortal.Services;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

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

        public static class ErrorSources
        {
            public const string RazorError = "Microsoft.AspNetCore.Mvc.Razor";
        }

        public ErrorController(
            TraffkTenantModelDbContext db,
            CurrentContextServices current,
            ILogger logger,
            IHostingEnvironment hostingEnvironment
        )
            : base(AspHelpers.MainNavigationPageKeys.Main, db, current, logger)
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
                //TODO: Refactor Error Index view to not be dependent on _LayoutUniversal
                if (exception.Error.Source == ErrorSources.RazorError || exception.Error.Source.Contains("Razor"))
                {
                    var simpleException = new
                    {
                        Message = exception.Error.Message,
                        StackTrace = exception.Error.StackTrace,
                    };
                    return new JsonResult(simpleException);
                }
            }
            var exceptionError = ExceptionError.CreateFromJson(TempData[Name].ToString());

            return View(ActionNames.Index, new ErrorModel(ViewData[ErrorKeys.StatusCode]?.ToString() ?? "", exceptionError));
        }

        [Route("/Error")]
        [ActionName(ActionNames.Index)]
        public IActionResult Index(ErrorModel errorModel = null)
        {
            var exception = ExceptionError.CreateFromJson(TempData[Name].ToString());

            if (errorModel == null)
            {
                errorModel = new ErrorModel(ViewData[ErrorKeys.StatusCode]?.ToString() ?? "", exception);
            }
            
            return View(errorModel);
        }

        protected void SetException(Exception exception)
        {
            TempData[Name] = new ExceptionError(exception).ToJson();
            TempData[AspHelpers.ViewDataKeys.ExceptionStatusCode] = HttpContext.Response.StatusCode;
        }
    }
}