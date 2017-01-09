using Microsoft.Extensions.Configuration;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Communications
{
    internal class CommunicationsFiller
    {
        public delegate string CodeExtractor(CreativeSettings cs);

        protected IDictionary<string, ReusableValue> ReusableValueByKey;

        private readonly ICreativeSettingsFinder Finder;

        public CommunicationsFiller(ICreativeSettingsFinder finder, ICollection<ReusableValue> reusableValues)
        {
            Requires.NonNull(finder, nameof(finder));

            Finder = finder;
            ReusableValueByKey = (reusableValues ?? ReusableValue.None).ToDictionaryOnConflictKeepLast(z => z.Key, z => z);
        }

        private static readonly Regex DollarStringIndexerExpr = new Regex(@"\{\s*(?<IndexerName>\w+)\s*\[\s*((?<NumericKey>\d+?)|""(?<DoubleQuoteStringKey>[^""]*?)""|'(?<SingleQuoteStringKey>[^']*?)')\s*\]\s*}", RegexOptions.Compiled);
        private static readonly Regex DollarStringFunctionExpr = new Regex(@"\{\s*(?<FuncName>\w+)\s*\(\s*(?<ParamList>[^)]*?)\s*\)\s*}", RegexOptions.Compiled);
        private static readonly Regex DollarStringTokenExpr = new Regex(@"\{\s*([^}]+?)\s*}", RegexOptions.Compiled);
        private const string IntermediateOpenSquigleReplacement = "__#@!#@!OpEN_sQuIGGle__";

        private static Match FindFirstFunction(string s, string funcName)
        {
            var funcMatches = DollarStringFunctionExpr.Matches(s);
            if (funcMatches.Count > 0)
            {
                for (int z = 0; z < funcMatches.Count; ++z)
                {
                    var m = funcMatches[z];
                    if (StringHelpers.IsSameIgnoreCase(m.Groups["FuncName"].Value, funcName))
                    {
                        return m;
                    }
                }
            }
            return null;
        }

        public class CompiledContext
        {
            public readonly IConfiguration Config;

            public CompiledContext(IDictionary<string, object> contextObjectsByKey)
            {
                var builder = new ConfigurationBuilder();
                if (contextObjectsByKey != null)
                {
                    foreach (var kvp in contextObjectsByKey)
                    {
                        builder.AddObject(kvp.Value, kvp.Key);
                    }
                }
                Config = (IConfiguration)builder.Build();
            }
        }

        public void Flatten(CreativeSettings s)
        {
            s.EmailHtmlBody = Flatten(s.EmailHtmlBody, z => z.EmailHtmlBody);
            s.EmailSubject = Flatten(s.EmailSubject, z => z.EmailSubject);
            s.EmailTextBody = Flatten(s.EmailTextBody, z => z.EmailTextBody);
            s.TextMessageBody = Flatten(s.TextMessageBody, z => z.TextMessageBody);
        }

        private string Flatten(string s, CodeExtractor extractor)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            s = s.Replace("{{", IntermediateOpenSquigleReplacement);
            EvalFuncs:
            var funcMatches = DollarStringFunctionExpr.Matches(s);
            if (funcMatches.Count > 0)
            {
                for (int z = 0; z < funcMatches.Count; ++z)
                {
                    var m = funcMatches[z];
                    if (StringHelpers.IsSameIgnoreCase(m.Groups["FuncName"].Value, "Layout"))
                    {
                        var pl = m.Groups["ParamList"].Value;
                        int templateId = int.Parse(pl);
                        s = s.Replace(m);
                        var layoutCode = extractor(Finder.FindSettingsById(templateId));
                        m = FindFirstFunction(layoutCode, "RenderBody");
                        s = layoutCode.Replace(m, s);
                        goto EvalFuncs;
                    }
                }
            }
            s = s.Replace(IntermediateOpenSquigleReplacement, "{{");
            return s;
        }

        public string Evaluate(Creative creative, CodeExtractor extractor, CompiledContext context)
        {
            Requires.NonNull(creative, nameof(creative));
            Requires.NonNull(extractor, nameof(extractor));
            Requires.NonNull(context, nameof(context));

            var c = context.Config;

            Requires.Equals(creative.TemplateEngineType, TemplateEngineTypes.TraffkDollarString);

            var s = extractor(creative.CreativeSettings) ?? "";
            s = Flatten(s, extractor);
            s = s.Replace("{{", IntermediateOpenSquigleReplacement);

            s = DollarStringIndexerExpr.Replace(s, delegate (Match m)
            {
                var indexerName = m.Groups["IndexerName"].Value;
                var skey = Stuff.CoalesceStrings(m.Groups["SingleQuoteStringKey"]?.Value, m.Groups["DoubleQuoteStringKey"]?.Value);
                switch (indexerName)
                {
                    case "R":
                        var rv = ReusableValueByKey[skey];
                        return rv?.Value ?? "";
                }
                return "";
            });

            s = DollarStringTokenExpr.Replace(s, delegate (Match m)
            {
                var fieldKey = m.Groups[1].Value.Replace('.', ':');
                return c[fieldKey];
            });
            s = s.Replace(IntermediateOpenSquigleReplacement, "{");
            return s;
        }
    }
}
