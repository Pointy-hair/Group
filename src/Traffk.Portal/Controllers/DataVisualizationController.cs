using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Traffk.Tableau;

namespace TraffkPortal.Controllers
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