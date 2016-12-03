using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffk.Bal.Data.Ddb
{
    public class ExceptionError
    {
        [JsonProperty("errorType")]
        public string ErrorType { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorCode")]
        public object ErrorCode { get; set; }

        [JsonProperty("innerErrors")]
        public IList<ExceptionError> InnerErrors { get; set; }

        public ExceptionError()
        { }

        public ExceptionError(Exception ex)
        {
            ErrorType = ex.GetType().Name;
            ErrorMessage = ex.Message;
            ErrorCode = BaseCodedException.GetCode(ex);
            var kids = new List<ExceptionError>();
            if (ex.InnerException != null)
            {
                kids.Add(new ExceptionError(ex.InnerException));
            }
            if (ex is AggregateException)
            {
                ((AggregateException)ex).InnerExceptions.ForEach(iex => kids.Add(new ExceptionError(iex)));
            }
            if (kids.Count > 0)
            {
                InnerErrors = kids;
            }
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
