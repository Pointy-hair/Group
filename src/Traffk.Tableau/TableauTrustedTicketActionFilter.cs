using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RevolutionaryStuff.Core;

namespace Traffk.Tableau
{
    public class SetTableauTrustedTicketAttribute : ServiceFilterAttribute
    {
        public SetTableauTrustedTicketAttribute() 
            : base(typeof(TableauTrustedTicketActionFilter))
        {
            
        }
    }

    public sealed class TableauTrustedTicketActionFilter : IAsyncActionFilter
    {
        private const string TrustedTicketCookieName = "trustedTicket";
        private readonly ITableauServices TableauServices;

        public TableauTrustedTicketActionFilter(ITableauServices tableauServices)
        {
            TableauServices = tableauServices;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            await next();
            var trustedTicket = await TableauServices.GetTrustedTicket();
            if (trustedTicket == null)
            {
                context.HttpContext.Response.Cookies.Delete(TrustedTicketCookieName);
            }
            else
            {
                context.HttpContext.Response.Cookies.Append(TrustedTicketCookieName, trustedTicket);
            }
        }
    }
}
