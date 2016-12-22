using Newtonsoft.Json;
using RevolutionaryStuff.Azure.DocumentDb;
using RevolutionaryStuff.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public partial class CrmDdbContext
    {
        internal Task<GetCountsResult> GetCountsAsync<TDdbEntity>(params Expression<Func<TDdbEntity, object>>[] fieldNameExpressions)
            where TDdbEntity : DdbEntity
        {
            var fieldNames = new string[fieldNameExpressions.Length];
            for (int z = 0; z < fieldNames.Length; ++z)
            {
                var e = fieldNameExpressions[z];
                var mis = e.GetMembers();
                string name = mis.ConvertAll(mi => mi.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? mi.Name).Join(".");
                fieldNames[z] = name;
            }
            return GetCountsAsync<TDdbEntity>(fieldNames);
        }

        internal Task<GetCountsResult> GetCountsAsync<TDdbEntity>(bool includePhiFields) where TDdbEntity : DdbEntity => GetCountsAsync<TDdbEntity>(GetCountableFields(typeof(TDdbEntity), includePhiFields));

        private class GetCountableFieldsContext
        {
            public string Prefix;
            public GetCountableFieldsContext(GetCountableFieldsContext parent = null, MemberInfo mi = null)
            {
                Prefix = (parent?.Prefix ?? "").AppendIfHasData(".");
                Prefix += (mi == null ? "" : mi.GetJsonPropertyName());
            }
        }

        private static IList<string> GetCountableFields(Type baseType, bool includePhiFields)
        {
            var fieldNames = new List<string>();
            baseType.MemberWalk<GetCountableFieldsContext>(
                BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public,
                (parent, t, mi) => new GetCountableFieldsContext(parent, mi),
                (ctx, t, mi) => 
                {
                    if (!mi.IsCountable(includePhiFields)) return;
                    var fieldName = ctx.Prefix.AppendIfHasData(".") + mi.GetJsonPropertyName();
                    fieldNames.Add(fieldName);
                },
                (ctx, t, mi) =>
                {
                    if (t.IsA(typeof(IEnumerable))) return false;
                    if (!t.Namespace.StartsWith("Traffk.")) return false;
                    return true;
                } 
                );
            return fieldNames;
        }

        private async Task<GetCountsResult> GetCountsAsync<TDdbEntity>(IList<string> fieldNames)
            where TDdbEntity : DdbEntity
        {
            if (fieldNames == null || fieldNames.Count == 0) return new GetCountsResult();
            var tenantId = await TenantFinder.GetTenantIdAsync();
            var dc = typeof(TDdbEntity).GetCustomAttribute<DocumentCollectionAttribute>();
            return await CallSprocAsync<GetCountsResult>(dc.DatabaseName, dc.CollectionName, "getCounts", tenantId, fieldNames);
        }
    }
}
