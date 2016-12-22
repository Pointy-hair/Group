using Microsoft.Extensions.Configuration;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Templates
{
    public class TemplateManager
    {
        protected IDictionary<string, ReusableValue> ReusableValueByKey;

        public ITemplateFinder Finder { get; }

        public TemplateManager(ITemplateFinder finder, IList<ReusableValue> reusableValues)
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

        public string Evaluate(Template template, IDictionary<string, object> contextObjectsByKey)
        {
            Requires.NonNull(template, nameof(template));

            var builder = new ConfigurationBuilder();
            if (contextObjectsByKey != null)
            {
                foreach (var kvp in contextObjectsByKey)
                {
                    builder.AddObject(kvp.Value, kvp.Key);
                }
            }
            var c = (IConfiguration) builder.Build();

            Requires.Equals(template.TemplateEngineType, Template.TemplateEngineTypes.TraffkDollarString);

            var s = template.Code ?? "";
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
                        var layout = Finder.FindTemplateById(templateId);
                        m = FindFirstFunction(layout.Code, "RenderBody");
                        s = layout.Code.Replace(m, s);
                        goto EvalFuncs;
                    }
                }
            }

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
