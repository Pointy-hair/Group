using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;

namespace Traffk.Bal.Data.Rdb
{
    public partial class Template
    {
        public static class TemplateEngineTypes
        {
            public const string Default = TraffkDollarString;

            public const string TraffkDollarString = "DollarString1";
            public const string Razor = "RazorEngine";

            public static readonly ICollection<string> All = new List<string> { TraffkDollarString, Razor }.AsReadOnly();
        }

        public static class ModelTypes
        {
            public const string CallbackUrl = "CallbackUrlModel";
            public const string SimpleCodeModel = "SimpleCodeModel";
            public const string SimpleContentModel = "SimpleContentModel";
            public const string ContactSummaryPhiModel = "CreateContactSummaryPhiModel";

            public static object CreateCallbackUrlModel(string callbackUrl, string contextMessage=null)
            {
                return new { callbackUrl = new Uri(callbackUrl), contextMessage = contextMessage };
            }

            public static object CreateSimpleCodeModel(string code)
            {
                return new { code = code };
            }

            public static object CreateSimpleContentModel(string subjectContent, string bodyContent)
            {
                return new { subjectContent = subjectContent, bodyContent = bodyContent };
            }

            public static object CreateContactSummaryPhiModel(Eligibility e)
            {
                return new { eligibility = e };
            }

            public static readonly ICollection<string> All = new List<string> { CallbackUrl, SimpleCodeModel, SimpleContentModel, ContactSummaryPhiModel }.AsReadOnly();

            public static void RequiresPredefined(string modelType, string modelTypeArgName)
            {
                Requires.SetMembership(All, nameof(ModelTypes) + "." + nameof(All), modelType, modelTypeArgName, true);
            }
        }
    }
}
