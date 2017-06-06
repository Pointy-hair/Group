using System;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Communications
{
    public static class CommunicationModelFactory
    {
        public static object CreateCallbackUrlModel(string callbackUrl, string contextMessage = null)
            => new { callbackUrl = new Uri(callbackUrl), contextMessage = contextMessage };

        public static object CreateSimpleCodeModel(string code)
            => new { code = code };

        public static object CreateSimpleContentModel(string subjectContent, string bodyContent)
            => new { subjectContent = subjectContent, bodyContent = bodyContent };

        public static object CreateContactSummaryPhiModel(Eligibility e)
            => new { eligibility = e };
    }
}
