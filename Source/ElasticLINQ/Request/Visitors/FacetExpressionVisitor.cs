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
        private const string GroupKeyFacet = "GroupKey";
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

        private bool aggregateWithoutMember;
        private readonly HashSet<MemberInfo> aggregateMembers = new HashSet<MemberInfo>();
        private readonly Dictionary<string, ICriteria> aggregateCriteria = new Dictionary<string, ICriteria>();
        private readonly ParameterExpression bindingParameter = Expression.Parameter(typeof(AggregateRow), "r");

        private Expression groupBy;
        private int? size;
        private LambdaExpression selectProjection;

        private FacetExpressionVisitor(IElasticMapping mapping, string prefix)
            : base(mapping, prefix)
        {
        }

        internal static RebindCollectionResult<IFacet> Rebind(IElasticMapping mapping, string prefix, Expression expression)
        {
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("expression", expression);

            var visitor = new FacetExpressionVisitor(mapping, prefix);
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
                        var groupByField = Mapping.GetFieldName(Prefix, ((MemberExpression)groupBy).Member);
                        if (aggregateWithoutMember)
                            yield return new TermsFacet(GroupKeyFacet, null, size, groupByField);

                        foreach (var valueField in aggregateMembers.Select(member => Mapping.GetFieldName(Prefix, member)))
                            yield return new TermsStatsFacet(valueField, groupByField, valueField, size);

                        foreach (var criteria in aggregateCriteria)
                            yield return new TermsFacet(criteria.Key, criteria.Value, size, groupByField);

                        break;
                    }
                case ExpressionType.Constant:
                    {
                        if (aggregateWithoutMember)
                            yield return new FilterFacet(GroupKeyFacet, new MatchAllCriteria());

                        foreach (var valueField in aggregateMembers.Select(member => Mapping.GetFieldName(Prefix, member)))
                            yield return new StatisticalFacet(valueField, valueField);

                        foreach (var criteria in aggregateCriteria)
                            yield return new FilterFacet(criteria.Key, criteria.Value);

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

                if (m.Method.Name == "Take" && m.Arguments.Count == 2)
                    return VisitTake(m.Arguments[0], m.Arguments[1]);

                string operation;
                if (aggregateOperations.TryGetValue(m.Method.Name, out operation))
                    switch (m.Arguments.Count)
                    {
                        case 1:
                            return VisitAggregateGroupKeyOperation(operation, m.Method.ReturnType);
                        case 2:
                            return VisitAggregateGroupPredicateOperation(m.Arguments[1], operation, m.Method.ReturnType);
                    }

                if (aggregateMemberOperations.TryGetValue(m.Method.Name, out operation) && m.Arguments.Count == 2)
                    return VisitAggregateMemberOperation(m.Arguments[1], operation, m.Method.ReturnType);
            }
            
            return m; // Do not base.VisitMethodCall as we don't want to examine the whole tree
        }

        private Expression VisitTake(Expression source, Expression takeExpression)
        {
            var takeConstant = Visit(takeExpression) as ConstantExpression;
            if (takeConstant != null)
            {
                var takeValue = (int)takeConstant.Value;
                size = size.HasValue ? Math.Min(size.GetValueOrDefault(), takeValue) : takeValue;
            }
            return Visit(source);
        }

        private Expression VisitAggregateGroupPredicateOperation(Expression predicate, string operation, Type returnType)
        {
            var lambda = predicate.GetLambda();
            var body = BooleanMemberAccessBecomesEquals(lambda.Body);

            var criteriaExpression = body as CriteriaExpression;
            if (criteriaExpression == null)
                throw new NotSupportedException(string.Format("Unknown Aggregate predicate '{0}'", body));

            var facetName = String.Format(GroupKeyFacet + "." + aggregateCriteria.Count + 1);
            aggregateCriteria.Add(facetName, criteriaExpression.Criteria);
            return RebindValue(facetName, operation, returnType);
        }

        private Expression VisitGroupKeyAccess(Type returnType)
        {
            var getKeyExpression = Expression.Call(null, getKey, bindingParameter);
            return Expression.Convert(getKeyExpression, returnType);
        }

        private Expression VisitAggregateGroupKeyOperation(string operation, Type returnType)
        {
            aggregateWithoutMember = true;
            return RebindValue(GroupKeyFacet, operation, returnType);
        }

        private Expression VisitAggregateMemberOperation(Expression property, string operation, Type returnType)
        {
            var member = GetMemberInfoFromLambda(property);
            aggregateMembers.Add(member);
            return RebindValue(Mapping.GetFieldName(Prefix, member), operation, returnType);
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
                throw new NotSupportedException(String.Format("Aggregate required Lambda expression, {0} expression encountered.", expression.NodeType));

            var memberExpressionBody = lambda.Body as MemberExpression;
            if (memberExpressionBody == null)
                throw new NotSupportedException(String.Format("Aggregates must be specified against a member of the entity not {0}", lambda.Body.Type));

            return memberExpressionBody.Member;
        }
    }
}