using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
            string trustedTicket = null;
            try
            {
                trustedTicket = await TableauServices.GetTrustedTicket();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
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
