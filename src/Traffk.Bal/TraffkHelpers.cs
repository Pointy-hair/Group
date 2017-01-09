using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Crypto;
using RevolutionaryStuff.Core.EncoderDecoders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Traffk.Bal.Data;
using H = cloudscribe.HtmlAgilityPack;

namespace Traffk.Bal
{
    public static class TraffkHelpers
    {
        public static readonly Assembly ThisAssembly;

        public const string TraffkUrn = "urn:traffk.com";

        public static class CustomDataTypes
        {
            public const string HtmlColor = "HtmlColor";
        }

        public static readonly Regex HumanSeparatedEntrySplitter = new Regex(@"[\s;,]", RegexOptions.Singleline | RegexOptions.Compiled);

        public static string ToHumanSeparatedEntryString(this IEnumerable<string> vals, string separator = " ")
        {
            if (vals == null) return "";
            return vals.Join(separator);
        }

        public static string[] ToArrayFromHumanDelineatedString(this string s, bool uniqueify=false)
        {
            if (s == null) return Empty.StringArray;
            var e = s.Split(HumanSeparatedEntrySplitter).ConvertAll(p => p.Trim()).Where(p => p.Length > 0);
            if (uniqueify)
            {
                e = e.Distinct();
            }
            return e.ToArray();
        }

        public static H.HtmlNodeCollection SelectNodesOrEmpty(this H.HtmlNode node, string xpath)
        {
            Requires.NonNull(node, nameof(node));
            var nodeCollection = node.SelectNodes(xpath);
            return nodeCollection ?? new H.HtmlNodeCollection(node);
        }

        public static string ToHtmlString(this H.HtmlDocument doc)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                doc.Save(sw);
            }
            return sb.ToString();
        }

        public static TItem JsonConvertDeserializeObjectOrFallback<TItem>(string json, TItem fallback = default(TItem))
        {
            if (string.IsNullOrEmpty(json)) return fallback;
            try
            {
                return JsonConvert.DeserializeObject<TItem>(json);
            }
            catch (Exception)
            {
                return fallback;
            }         
        }

        public static string AppendIfHasData(this string s, string append) => string.IsNullOrEmpty(s) ? s : s + append;

        public static string GetJsonPropertyName(this MemberInfo mi) => mi.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? mi.Name;

        public static string GetColumnPropertyName(this MemberInfo mi) => mi.GetCustomAttribute<ColumnAttribute>()?.Name ?? mi.Name;

        public static string GetSerializedPropertyName(this MemberInfo mi) => mi.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? mi.GetCustomAttribute<ColumnAttribute>()?.Name ?? mi.Name;

        public static bool IsCountable(this MemberInfo mi, bool includePhi)
        {
            var pi = mi as PropertyInfo;
            if (pi != null && pi.CanRead)
            {
                var phi = pi.GetCustomAttribute<ProtectedHealthInformationAttribute>();
                if (phi != null && !includePhi) return false;
                if (pi.GetCustomAttribute<FreeFormDataAttribute>() != null) return false;
                if (pi.GetCustomAttribute<ConstrainedDataAttribute>() == null) return false;
                return true;
            }
            return false;
        }

        static TraffkHelpers()
        {
            ThisAssembly = typeof(TraffkHelpers).GetTypeInfo().Assembly;
        }
    }
}
