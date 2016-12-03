using RevolutionaryStuff.Core;
using System.Linq;
using System;
using System.Linq.Expressions;
using RevolutionaryStuff.Core.Collections;
using System.Reflection;
using Newtonsoft.Json;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    public partial class DdbDocSet<TContext, TEntity> : IOrderedQueryable<TEntity> where TContext : DdbContext where TEntity : DdbEntity
    {
        /// <remarks>https://blogs.msdn.microsoft.com/mattwar/2007/07/31/linq-building-an-iqueryable-provider-part-ii/</remarks>
        private class DdbDocSetQueryProvider : QueryProvider
        {
            private readonly DdbDocSet<TContext, TEntity> DocSet;
            private int? TopN, SkipN, TakeN;
            private bool FirstOrDefault;
            private int WhereClauseCount;
            private string Selection;

            public DdbDocSetQueryProvider(DdbDocSet<TContext, TEntity> docSet)
            {
                DocSet = docSet;
            }

            public override object Execute(Expression expression)
            {
                var jsonQuery = new Internal.JsonQuery(Parse(expression))
                {
                    SkipN = SkipN,
                    TakeN = TakeN,
                };
                var e = DocSet.Query<TEntity>(jsonQuery);
                if (FirstOrDefault)
                {
                    var q = ((System.Collections.IEnumerable)e).GetEnumerator();
                    if (q.MoveNext())
                    {
                        return q.Current;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return e;
                }
            }

            private const string Alias = "z";
            private const string SelectionPlaceholder = "___SELECTION___";

            public override string GetQueryText(Expression expression) => Parse(expression);

            private string Parse(Expression e)
            {
                TopN = SkipN = TakeN = null;
                FirstOrDefault = false;
                WhereClauseCount = 0;
                var sql = Visit(e);
                sql = sql.Replace(SelectionPlaceholder, Selection??"*");
                return sql;
            }


            private string Visit(Expression e)
            {
                if (e is MethodCallExpression)
                {
                    return Visit((MethodCallExpression)e);
                }
                else if (e is UnaryExpression)
                {
                    return Visit((UnaryExpression)e);
                }
                else if (e is BinaryExpression)
                {
                    return Visit((BinaryExpression)e);
                }
                else if (e is ConstantExpression)
                {
                    return Visit((ConstantExpression)e);
                }
                else if (e is LambdaExpression)
                {
                    return Visit((LambdaExpression)e);
                }
                else if (e is MemberExpression)
                {
                    return Visit((MemberExpression)e);
                }
                return "???";
                //                throw new NotSupportedException();
            }

            private string Visit(ConstantExpression e)
            {
                IQueryable q = e.Value as IQueryable;

                string s;

                if (q != null)
                {
                    // assume constant nodes w/ IQueryables are table references
                    var top = TopN.HasValue ? $"top {TopN} " : "";
                    s = $"select {top}{SelectionPlaceholder} from {DocSet.CollectionName} {Alias}";
                }
                else if (e.Value == null)
                {
                    s = "null";
                }
                else
                {
                    s = JsonConvert.SerializeObject(e.Value);
                }

                return s;
            }

            private string Visit(BinaryExpression e)
            {
                string op;
                switch (e.NodeType)
                {
                    case ExpressionType.AndAlso:
                        op = "and";
                        break;
                    case ExpressionType.Or:
                        op = "or";
                        break;
                    case ExpressionType.Equal:
                        op = "=";
                        break;
                    case ExpressionType.NotEqual:
                        op = "!=";
                        break;
                    case ExpressionType.LessThan:
                        op = "<";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        op = "<=";
                        break;
                    case ExpressionType.GreaterThan:
                        op = ">";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        op = ">=";
                        break;
                    default:
                        throw new NotSupportedException($"operator type {e.NodeType} is not supported");
                }
                return $"({Visit(e.Left)} {op} {Visit(e.Right)})";
            }

            private string Visit(MemberExpression e)
            {
                if (!(e.Expression is ParameterExpression))
                {
                    try
                    {
                        object value = Expression.Lambda(e).Compile().DynamicInvoke();
                        return JsonConvert.SerializeObject(value);
                    }
                    catch (Exception) { }
                }
                var members = e.GetMembers();
                return Alias + "." + members.ConvertAll(mi => GetJsonName(mi)).Join(".");
            }

            private string Visit(LambdaExpression e)
            {
                if (e.Body is BinaryExpression)
                {
                    return Visit((BinaryExpression)e.Body);
                }
                else
                {
                    var members = e.GetMembers();
                    return Alias + "." + members.ConvertAll(mi => GetJsonName(mi)).Join(".");
                }
            }

            private string Visit(UnaryExpression e)
            {
                switch (e.NodeType)
                {
                    case ExpressionType.Quote:
                        return Visit(e.Operand);
                    default:
                        throw new NotSupportedException(e.NodeType.ToString());
                }
            }

            private string Visit(MethodCallExpression e)
            {
                var methodName = e.Method.Name;
                switch (methodName)
                {
                    case LinqHelpers.StandardMethodNames.FirstOrDefault:
                        {
                            TopN = 1;
                            FirstOrDefault = true;
                            var left = Visit(e.Arguments[0]);
                            if (e.Arguments.Count > 1)
                            {
                                var right = Visit(e.Arguments[1]);
                                return $"{left} where {right}";
                            }
                            else
                            {
                                return left;
                            }
                        }
                    case LinqHelpers.StandardMethodNames.Where:
                        {
                            var left = Visit(e.Arguments[0]);
                            var combiner = WhereClauseCount++ == 0 ? "where" : "and";
                            var right = Visit(e.Arguments[1]);
                            return $"{left} {combiner} {right}";
                        }
                    case LinqHelpers.StandardMethodNames.OrderBy:
                    case LinqHelpers.StandardMethodNames.OrderByDescending:
                        {
                            var left = Visit(e.Arguments[0]);
                            var right = Visit(e.Arguments[1]);
                            switch (methodName)
                            {
                                case LinqHelpers.StandardMethodNames.OrderBy:
                                    return $"{left} order by {right}";
                                case LinqHelpers.StandardMethodNames.OrderByDescending:
                                    return $"{left} order by {right} desc";
                                default:
                                    throw new UnexpectedSwitchValueException(methodName);
                            }
                        }
                    case LinqHelpers.StandardMethodNames.Select:
                        {
                            var left = Visit(e.Arguments[0]);
                            var right = Visit(e.Arguments[1]);
                            Selection = right;
                            return left;
                        }
                    case LinqHelpers.StandardMethodNames.Skip:
                        {
                            var left = Visit(e.Arguments[0]);
                            var right = Visit(e.Arguments[1]);
                            SkipN = int.Parse(right);
                            return left;
                        }
                    case LinqHelpers.StandardMethodNames.Take:
                        {
                            var left = Visit(e.Arguments[0]);
                            var right = Visit(e.Arguments[1]);
                            TakeN = int.Parse(right);
                            return left;
                        }
                    default:
                        throw new NotSupportedException();
                }
            }

            private string GetJsonName(MemberInfo mi) => mi.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? mi.Name;
        }
    }
}
