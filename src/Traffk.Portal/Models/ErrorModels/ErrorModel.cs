using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data;

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

        public List<ErrorModel> InnerErrorModels { get; set; }

        public ErrorModel(string statusCode, ExceptionError exception)
        {
            StatusCode = statusCode;
            Type = exception.ErrorType;
            Message = exception.ErrorMessage;
            StackTrace = exception.ErrorStackTrace;
            if (exception.InnerErrors != null)
            {
                InnerErrorModels = new List<ErrorModel>();
                foreach (var innerError in exception.InnerErrors)
                {
                    InnerErrorModels.Add(new ErrorModel(statusCode, innerError));
                }
            }
        }
    }
}
