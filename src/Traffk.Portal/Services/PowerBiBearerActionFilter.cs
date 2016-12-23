﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace TraffkPortal.Services
{
    public class SetPowerBiBearerAttribute : ServiceFilterAttribute
    {
        public SetPowerBiBearerAttribute()
            : base(typeof(SetPowerBiBearerActionFilter))
        { }
    }

    public sealed class SetPowerBiBearerActionFilter : IAsyncActionFilter
    {
        private const string BearerCookieName = "powerBiBearer";

        private readonly CurrentContextServices Current;

        public SetPowerBiBearerActionFilter(CurrentContextServices current)
        {
            Current = current;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            var bearer = await Current.PowerBi.GetBearer();
            if (bearer == null)
            {
                context.HttpContext.Response.Cookies.Delete(BearerCookieName);
            }
            else
            {
                context.HttpContext.Response.Cookies.Append(BearerCookieName, bearer);
            }
        }
    }
}
