using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Traffk.Bal.Data
{
    [DataContract]
    public class ExceptionError
    {
        [JsonProperty("errorType")]
        [DataMember(Name = "errorType")]
        public string ErrorType { get; set; }

        [JsonProperty("errorMessage")]
        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorCode")]
        [DataMember(Name = "errorCode")]
        public object ErrorCode { get; set; }

        [JsonProperty("innerErrors")]
        [DataMember(Name = "innerErrors")]
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
