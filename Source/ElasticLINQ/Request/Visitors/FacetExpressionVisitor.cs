// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Facets;
using ElasticLinq.Response.Materializers;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Gathers and rebinds aggregate operations into ElasticSearch facets.
    /// </summary>
    internal class FacetExpressionVisitor : CriteriaExpressionVisitor
    {
        private const string GroupKeyFacetFormat = "GroupKey.{0}";
        private static readonly MethodInfo getValue = typeof(AggregateRow).GetMethod("GetValue", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo getKey = typeof(AggregateRow).GetMethod("GetKey", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly Dictionary<string, string> aggregateMemberOperations = new Dictionary<string, string>
        {
            { "Min", "min" },
            { "Max", "max" },
            { "Sum", "total" },
            { "Average", "mean" },
        };

        private static readonly Dictionary<string, string> aggregateOperations = new Dictionary<string, string>
        {
            { "Count", "count" },
            { "LongCount", "count" }
        };

        private readonly HashSet<MemberInfo> aggregateMembers = new HashSet<MemberInfo>();
        private readonly Dictionary<string, ICriteria> aggregateCriteria = new Dictionary<string, ICriteria>(); 
        private readonly ParameterExpression bindingParameter = Expression.Parameter(typeof(AggregateRow), "r");

        private Expression groupBy;
        private LambdaExpression selectProjection;

        private FacetExpressionVisitor(IElasticMapping mapping)
            : base(mapping)
        {
        }

        internal static RebindCollectionResult<IFacet> Rebind(IElasticMapping mapping, Expression expression)
        {
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("expression", expression);

            var visitor = new FacetExpressionVisitor(mapping);
            var visitedExpression = visitor.Visit(expression);
            var facets = new HashSet<IFacet>(visitor.GetFacets());

            return new RebindCollectionResult<IFacet>(visitedExpression, facets, visitor.bindingParameter, visitor.selectProjection);
        }

        private IEnumerable<IFacet> GetFacets()
        {
            if (groupBy == null)
                yield break;

            switch (groupBy.NodeType)
            {
                case ExpressionType.MemberAccess:
                    {
                        var groupByField = Mapping.GetFieldName(((MemberExpression)groupBy).Member);
                        foreach (var valueField in aggregateMembers.Select(member => Mapping.GetFieldName(member)))
                            yield return new TermsStatsFacet(valueField, groupByField, valueField);

                        foreach (var criteria in aggregateCriteria)
                            yield return new TermsFacet(criteria.Key, groupByField) { Filter = criteria.Value };

                        break;
                    }
                case ExpressionType.Constant:
                    {
                        foreach (var valueField in aggregateMembers.Select(member => Mapping.GetFieldName(member)))
                            yield return new StatisticalFacet(valueField, valueField);

                        foreach (var criteria in aggregateCriteria)
                            yield return new FilterFacet(criteria.Key) { Filter = criteria.Value };

                        break;
                    }

                default:
                    throw new NotSupportedException(String.Format("GroupBy must be either a Member or a Constant not {0}", groupBy.NodeType));
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.Name == "Key" && m.Member.DeclaringType.IsGenericOf(typeof(IGrouping<,>)))
                return VisitGroupKeyAccess(m.Type);

            return base.VisitMember(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Enumerable) || m.Method.DeclaringType == typeof(Queryable))
            {
                var source = m.Arguments[0];

                if (m.Method.Name == "GroupBy" && m.Arguments.Count == 2)
                {
                    groupBy = m.Arguments[1].GetLambda().Body;
                    return Visit(source);
                }

                if (m.Method.Name == "Select" && m.Arguments.Count == 2)
                {
                    var y = Visit(m.Arguments[1]).GetLambda();
                    selectProjection = Expression.Lambda(y.Body, bindingParameter);
                    return Visit(source);
                }

                string operation;
                if (aggregateOperations.TryGetValue(m.Method.Name, out operation))
                    switch (m.Arguments.Count)
                    {
                        case 1:
                            return VisitAggregateGroupOperation(operation, m.Method.ReturnType);
                        case 2:
                            return VisitAggregateGroupPredicateOperation(m.Arguments[1], operation, m.Method.ReturnType);
                    }

                if (aggregateMemberOperations.TryGetValue(m.Method.Name, out operation) && m.Arguments.Count == 2)
                    return VisitAggregateMemberOperation(m.Arguments[1], operation, m.Method.ReturnType);
            }

            return base.VisitMethodCall(m);
        }

        private Expression VisitAggregateGroupPredicateOperation(Expression predicate, string operation, Type returnType)
        {
            var lambda = predicate.GetLambda();
            var body = BooleanMemberAccessBecomesEquals(lambda.Body);

            var criteriaExpression = body as CriteriaExpression;
            if (criteriaExpression == null)
                throw new NotSupportedException(string.Format("Unknown Aggregate predicate '{0}'", body));

            var facetName = String.Format(GroupKeyFacetFormat, aggregateCriteria.Count + 1);
            aggregateCriteria.Add(facetName, criteriaExpression.Criteria);
            return RebindValue(facetName, operation, returnType);
        }

        private Expression VisitGroupKeyAccess(Type returnType)
        {
            var getKeyExpression = Expression.Call(null, getKey, bindingParameter);
            return Expression.Convert(getKeyExpression, returnType);
        }

        private Expression VisitAggregateGroupOperation(string operation, Type returnType)
        {
            return RebindValue(GroupKeyFacetFormat, operation, returnType);
        }

        private Expression VisitAggregateMemberOperation(Expression property, string operation, Type returnType)
        {
            var member = GetMemberInfoFromLambda(property);
            aggregateMembers.Add(member);
            return RebindValue(Mapping.GetFieldName(member), operation, returnType);
        }

        private Expression RebindValue(string valueField, string operation, Type returnType)
        {
            var getValueExpression = Expression.Call(null, getValue, bindingParameter,
                Expression.Constant(valueField), Expression.Constant(operation), Expression.Constant(returnType));
            return Expression.Convert(getValueExpression, returnType);
        }

        private static MemberInfo GetMemberInfoFromLambda(Expression expression)
        {
            var lambda = expression.StripQuotes() as LambdaExpression;
            if (lambda == null)
                throw new NotSupportedException(String.Format("Aggregate requrired Lambda expression, {0} expression encountered.", expression.NodeType));

            var memberExpressionBody = lambda.Body as MemberExpression;
            if (memberExpressionBody == null)
                throw new NotSupportedException(String.Format("Aggregates must be specified against a member of the entity not {0}", lambda.Body.Type));

            return memberExpressionBody.Member;
        }
    }
}