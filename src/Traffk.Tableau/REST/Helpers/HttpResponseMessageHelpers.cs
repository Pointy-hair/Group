using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Traffk.Tableau.REST.Helpers
{
    public static class HttpResponseMessageHelpers
    {
        public static async Task<Stream> GetResponseStreamAsync(this HttpResponseMessage responseMessage)
        {
            return await responseMessage.Content.ReadAsStreamAsync();
        }
    }
}
