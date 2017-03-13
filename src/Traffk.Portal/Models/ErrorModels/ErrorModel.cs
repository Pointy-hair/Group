using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TraffkPortal.Models.ErrorModels
{
    public class ErrorModel
    {
        [Required]
        [DisplayName("Status Code")]
        public string StatusCode { get; set; }

        [DisplayName("Exception Type")]
        public string Type { get; set; }

        public string Message { get; set; }

        [DisplayName("Stack Trace")]
        public string StackTrace { get; set; }

        public ErrorModel(string statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
