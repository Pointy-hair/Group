using Newtonsoft.Json;
using RevolutionaryStuff.Azure.DocumentDb;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Traffk.Bal.Data.Ddb.Crm;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Data.Query
{
    public class TestExpression
    {
        public CollectionNames Collection;
        public string PropertyAccessorName;
        public Operators Operator;
        public object Operand;

        public TestExpression() { }

        public TestExpression(CollectionNames collectionName, string propertyAccessorName, Operators op, object operand=null)
        {
            Collection = collectionName;
            PropertyAccessorName = propertyAccessorName;
            Operator = op;
            Operand = operand;
        }

        public static ICollection<string> MatchingContactIds(CrmDdbContext crm, IEnumerable<TestExpression> ands)
        {
            var m = ands.ToMultipleValueDictionary(a => a.Collection);
            var idLists = new List<ICollection<string>>();
            Parallel.ForEach(m.Keys, new ParallelOptions { MaxDegreeOfParallelism = 1 }, delegate (CollectionNames collection)
            {
                Type collectionType;
                string contactIdFieldName;
                switch (collection)
                {
                    case CollectionNames.Contacts:
                        contactIdFieldName = "id";
                        collectionType = typeof(Ddb.Crm.Zontact);
                        break;
                    case CollectionNames.Eligibility:
                        contactIdFieldName = "contactId";
                        collectionType = typeof(Eligibility);
                        break;
                    default:
                        throw new UnexpectedSwitchValueException(collection);
                }
                var databaseName = collectionType.GetCustomAttribute<DocumentCollectionAttribute>().DatabaseName;
                var collectionName = collectionType.GetCustomAttribute<DocumentCollectionAttribute>().CollectionName;
                var alias = "z";
                var expressionParts = new List<string>();
                foreach (var te in m[collection])
                {
                    var propertyAccessorPartNames = te.PropertyAccessorName.Split('.');
                    var fieldNameParts = new List<string>();
                    var baseType = collectionType;
                    foreach (var propertyAccessorPartName in propertyAccessorPartNames)
                    {
                        var p = baseType.GetProperty(propertyAccessorPartName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        fieldNameParts.Add(
                            p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ??
                            p.GetCustomAttribute<ColumnAttribute>()?.Name ??
                            p.Name);
                        baseType = p.PropertyType;
                    }
                    var fieldName = fieldNameParts.Join(".");
                    string format;
                    switch (te.Operator)
                    {
                        case Operators.Equals:
                            format = "{0}={1}";
                            break;
                        case Operators.GreaterThan:
                            format = "{0}>{1}";
                            break;
                        case Operators.GreaterThanOrEqual:
                            format = "{0}>={1}";
                            break;
                        case Operators.LessThan:
                            format = "{0}<{1}";
                            break;
                        case Operators.LessThanOrEqual:
                            format = "{0}<={1}";
                            break;
                        case Operators.NotEquals:
                            format = "{0}<>{1}";
                            break;
                        case Operators.AnyOf:
                            format = "{1}.indexOf({0})>=0";
                            break;
                        case Operators.NoneOf:
                            format = "{1}.indexOf({0})=-1";
                            break;
                        case Operators.IsTrue:
                            format = "{0}";
                            break;
                        case Operators.IsFalse:
                            format = "not({0})";
                            break;
                        case Operators.IsDefined:
                            format = "is_defined({0})";
                            break;
                        case Operators.IsNotDefined:
                            format = "not(is_defined({0}))";
                            break;
                        case Operators.IsNotNull:
                            format = "{0}<>null";
                            break;
                        default:
                            throw new UnexpectedSwitchValueException(te.Operator);
                    }
                    string comparator = JsonConvert.SerializeObject(te.Operand);
                    expressionParts.Add(string.Format(format, $"{alias}.{fieldName}", comparator));
                }
                var sql = $"select {alias}.{contactIdFieldName} {SimpleVal.StringValuePropertyName} from {collectionName} {alias} where {expressionParts.Join(" and ", "({0})")}";
                var idList = crm.Query<SimpleVal>(databaseName, collectionName, sql).ToList().ExecuteSynchronously().ConvertAll(s => s.StringValue).ToSet();
                idLists.Add(idList);
            });
            var ids = idLists.FirstOrDefault();
            foreach (var otherIds in idLists.Skip(1))
            {
                var toRemove = new List<string>();
                foreach (var id in ids)
                {
                    if (!otherIds.Contains(id))
                    {
                        toRemove.Add(id);
                    }
                }
                foreach (var id in toRemove)
                {
                    ids.Remove(id);
                }
            }
            return ids;
        }
    }
}
