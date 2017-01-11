#if false

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Data.Query;
using RevolutionaryStuff.Core;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using Traffk.Bal;
using Traffk.Bal.Data;
using Traffk.Bal.Data.Rdb;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TraffkPortal.Models
{
    public class ContactQueryModel
    {
        public enum QueryFieldTypes
        {
            Boolean,
            Numeric,
            Date,
            Text,
        }

        public abstract class QueryBase
        {
            public QuerySection ParentSection { get; set; }
            public string Title { get; set; }
        }

        public class QueryField : QueryBase
        {
            public string PropertyPath { get; set; }

            public string JPath { get; set; }

            public QueryFieldTypes QueryFieldType { get; set; }
            public IList<SelectListItem> ValuesList { get; set; }
            public Operators Operator { get; set; }

            public bool BoolVal { get; set; }
            public double NumericVal { get; set; }
            public DateTime DateVal { get; set; }
            public string TextVal { get; set; }

            public QueryField() { }

            public QueryField(QuerySection section, PropertyInfo pi, GetCountsResult counts)
            {
                ParentSection = section;
                Title = pi.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? pi.Name;
                var pt = pi.PropertyType;
                if (pt.IsNumber())
                {
                    QueryFieldType = QueryFieldTypes.Numeric;
                }
                else if (pt == typeof(bool))
                {
                    QueryFieldType = QueryFieldTypes.Boolean;
                }
                else if (pt == typeof(DateTime))
                {
                    QueryFieldType = QueryFieldTypes.Date;
                }
                else
                {
                    QueryFieldType = QueryFieldTypes.Text;
                }
                PropertyPath = (ParentSection.PropertyPath.AppendIfHasData(".")??"") + pi.Name;
                JPath = (ParentSection.JPath.AppendIfHasData(".")??"") + pi.GetSerializedPropertyName();
                if (counts != null)
                {
                    var cntByVal = counts.CntByValByField.FindOrDefault(JPath);
                    if (cntByVal != null && cntByVal.Count > 0)
                    {
                        ValuesList = cntByVal.ConvertAll(kvp => new SelectListItem { Value = kvp.Key, Text = $"{kvp.Key} ({kvp.Value})" });
                    }
                }
            }
        }

        public class QuerySection : QueryBase
        {
            public static bool ShouldIgnore(PropertyInfo pi)
            {
                if (pi.GetCustomAttribute<HideInPortalAttribute>() != null) return true;
                if (pi.GetCustomAttribute<NotMappedAttribute>() != null) return true;
                if (pi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null) return true;
                if (pi.GetCustomAttribute<JsonIgnoreAttribute>() != null) return true;
                if (pi.GetCustomAttribute<InversePropertyAttribute>() != null) return true;
                if (UserDefinedDataAttribute.IsUserDefined(pi)) return true;
                return false;
            }

            private CollectionNames Collection { get; }

            public string PropertyPath { get; set; }

            public string JPath { get; set; }

            public IList<QueryBase> Children { get; set; } = new List<QueryBase>();

            public QuerySection() { }

            public QuerySection(CollectionNames collection, Type baseType, MemberInfo baseMember, bool includePhi, GetCountsResult counts, QuerySection parentSection = null)
            {
                Collection = collection;
                ParentSection = parentSection;
                Title = baseType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? baseType.Name;
                if (baseMember != null)
                {
                    PropertyPath = parentSection.PropertyPath + "." + baseMember.Name;
                    JPath = parentSection.JPath + "." + baseMember.GetSerializedPropertyName();
                }
                baseType.MemberWalk<QuerySection>(
                    BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                    (parent, t, mi) => this,
                    (ctx, t, mi) =>
                    {
                        var pi = (PropertyInfo)mi;
                        if (ShouldIgnore(pi)) return;
                        if (t.GetTypeInfo().IsClass && t.Namespace.StartsWith("Traffk."))
                        {
                            var s = new QuerySection(collection, t, mi, includePhi, counts, this);
                            if (s.Children.Count > 0)
                            {
                                Children.Add(s);
                            }
                        }
                        else 
                        {
                            var qf = new QueryField(this, pi, counts);
                            Children.Add(qf);
                        }
                    },
                    (ctx, t, mi) =>
                    {
                        if (t.IsA(typeof(IEnumerable))) return false;
                        if (!t.Namespace.StartsWith("Traffk.")) return false;
                        if (ShouldIgnore((PropertyInfo)mi)) return false;
                        return true;
                    }
                );
            }
        }

        public IList<QuerySection> Sections { get; set; } = new List<QuerySection>();

        public ContactQueryModel()
        { }

        public async Task Populate(TraffkRdbContext rdb, CrmDdbContext crm, bool includePhi, params CollectionNames[] supportedCollections)
        {
            foreach (var collection in supportedCollections)
            {
                Type baseType;
                GetCountsResult counts = null;
                switch (collection)
                {
                    case CollectionNames.Contacts:
                        counts = await rdb.GetContrainedFieldCountsAsync<Contact>(true);
                        baseType = typeof(Contact);
                        break;
                    case CollectionNames.Eligibility:
                        counts = await rdb.GetContrainedFieldCountsAsync<Eligibility>(true);
                        baseType = typeof(Eligibility);
                        break;
                    case CollectionNames.Scores:
                        counts = await rdb.GetContrainedFieldCountsAsync<Score>(true);
                        baseType = typeof(Score);
                        break;
                    case CollectionNames.Demographics:
                        counts = await rdb.GetContrainedFieldCountsAsync<Demographic>(true);
                        baseType = typeof(Demographic);
                        break;
                    case CollectionNames.Pcp:
                        counts = await rdb.GetContrainedFieldCountsAsync<MemberPCP>(true);
                        baseType = typeof(MemberPCP);
                        break;
                    case CollectionNames.HighCostDiagnosis:
                        counts = await rdb.GetContrainedFieldCountsAsync<HighCostDiagnosi>(true);
                        baseType = typeof(HighCostDiagnosi);
                        break;
                    default:
                        throw new UnexpectedSwitchValueException(collection);
                }
                var section = new QuerySection(collection, baseType, null, includePhi, counts);
                Sections.Add(section);
            }
        }

        private static IList<SelectListItem> CreateSelectListItems(SelectListItem firstItem, params Operators[] operators)
        {
            var items = new List<SelectListItem>();
            if (firstItem != null)
            {
                items.Add(firstItem);
            }
            foreach (var op in operators)
            {
                var item = new SelectListItem { Text = AspHelpers.GetDisplayName(op), Value = op.ToString() };
                items.Add(item);
            }
            return items;
        }

        public static void PopulateViewBagWithOperatorSelectListItems(dynamic viewBag)
        {
            viewBag.QueryFieldTypeBooleanOperators = CreateSelectListItems(
                AspHelpers.CreateNoneSelectedSelectListItem(),
                Operators.IsTrue,
                Operators.IsFalse
                );
            viewBag.QueryFieldTypeNumericOperators = CreateSelectListItems(
                AspHelpers.CreateNoneSelectedSelectListItem(),
                Operators.LessThan,
                Operators.LessThanOrEqual,
                Operators.Equals,
                Operators.NotEquals,
                Operators.GreaterThan,
                Operators.GreaterThanOrEqual
                );
            viewBag.QueryFieldTypeDateOperators = CreateSelectListItems(
                AspHelpers.CreateNoneSelectedSelectListItem(),
                Operators.LessThan,
                Operators.LessThanOrEqual,
                Operators.Equals,
                Operators.NotEquals,
                Operators.GreaterThan,
                Operators.GreaterThanOrEqual
                );
            viewBag.QueryFieldTypeTextOperators = CreateSelectListItems(
                AspHelpers.CreateNoneSelectedSelectListItem(),
                Operators.Equals,
                Operators.NotEquals
                );
            viewBag.OneOfOperators = CreateSelectListItems(
                null,
                Operators.AnyOf
                );
        }
    }
}


#endif