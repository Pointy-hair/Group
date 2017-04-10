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
        private readonly ITableauVisualServices TableauVisualServices;

        public TableauTrustedTicketActionFilter(ITableauVisualServices tableauVisualServices)
        {
            TableauVisualServices = tableauVisualServices;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            await next();
            string trustedTicket = null;
            try
            {
                trustedTicket = await TableauVisualServices.GetTrustedTicket();
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
