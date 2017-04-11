using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Traffk.Tableau;

namespace Traffk.Portal.Controllers
{
    public class DataVisualizationController : Controller
    {
        private readonly ITableauVisualServices TableauVisualServices;

        public DataVisualizationController(ITableauVisualServices tableauVisualServices)
        {
            TableauVisualServices = tableauVisualServices;
        }

        [Produces("text/html")]
        [Route("DataVisualization/{workbook}/{view}")]
        public HttpContent Index(string workbook, string view, string trustedTicket = "", string[] options = null)
        {
            if (String.IsNullOrEmpty(trustedTicket))
            {
                trustedTicket = Request.Cookies["tableauTicket"];
            }
            var reportHttpContent = TableauVisualServices.GetVisualization(workbook, view, trustedTicket);
            return reportHttpContent.Result;
        }

    }
}