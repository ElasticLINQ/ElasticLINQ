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
    class FacetRebindCollectionResult : RebindCollectionResult<IFacet>
    {
        public FacetRebindCollectionResult(Expression expression, IEnumerable<IFacet> collected, ParameterExpression parameter, IElasticMaterializer materializer)
            : base(expression, collected, parameter)
        {
            Materializer = materializer;
        }

        public IElasticMaterializer Materializer { get; }
    }

    /// <summary>
    /// Gathers and rebinds aggregate operations into facets.
    /// </summary>
    class FacetExpressionVisitor : CriteriaExpressionVisitor
    {
        const string GroupKeyFacet = "GroupKey";
        static readonly MethodInfo getValue = typeof(AggregateRow).GetMethodInfo(m => m.IsStatic && m.Name == "GetValue");
        static readonly MethodInfo getKey = typeof(AggregateRow).GetMethodInfo(m => m.Name == "GetKey");

        static readonly Dictionary<string, string> aggregateMemberOperations = new Dictionary<string, string>
        {
            { "Min", "min" },
            { "Max", "max" },
            { "Sum", "total" },
            { "Average", "mean" },
        };

        static readonly Dictionary<string, string> aggregateOperations = new Dictionary<string, string>
        {
            { "Count", "count" },
            { "LongCount", "count" }
        };

        bool aggregateWithoutMember;
        readonly HashSet<MemberExpression> aggregateMembers = new HashSet<MemberExpression>();
        readonly Dictionary<string, ICriteria> aggregateCriteria = new Dictionary<string, ICriteria>();
        readonly ParameterExpression bindingParameter = Expression.Parameter(typeof(AggregateRow), "r");

        Expression groupBy;
        int? size;
        LambdaExpression selectProjection;

        FacetExpressionVisitor(IElasticMapping mapping, Type sourceType)
            : base(mapping, sourceType)
        {
        }

        internal static FacetRebindCollectionResult Rebind(IElasticMapping mapping, Type sourceType, Expression expression)
        {
            Argument.EnsureNotNull(nameof(mapping), mapping);
            Argument.EnsureNotNull(nameof(expression), expression);

            var visitor = new FacetExpressionVisitor(mapping, sourceType);
            var visitedExpression = visitor.Visit(expression);
            var facets = new HashSet<IFacet>(visitor.GetFacets());
            var materializer = GetFacetMaterializer(visitor.selectProjection, visitor.groupBy);

            return new FacetRebindCollectionResult(visitedExpression, facets, visitor.bindingParameter, materializer);
        }

        static IElasticMaterializer GetFacetMaterializer(LambdaExpression projection, Expression groupBy)
        {
            if (projection == null)
                return null;

            Func<AggregateRow, object> projector = r => projection.Compile().DynamicInvoke(r);

            // Top level sum/count etc. Single result.
            if (groupBy == null)
                return new TermlessFacetElasticMaterializer(projector, projection.ReturnType);

            // GroupBy on a constant. Single result in a list.
            if (groupBy is ConstantExpression)
                return new ListTermlessFacetsElasticMaterializer(projector, projection.ReturnType, ((ConstantExpression)groupBy).Value);

            // GroupBy on a member. Multiple results in a list.
            return new ListTermFacetsElasticMaterializer(projector, projection.ReturnType, groupBy.Type);
        }

        IEnumerable<IFacet> GetFacets()
        {
            if (groupBy == null || groupBy.NodeType == ExpressionType.Constant)
                return GetTermlessFacets();

            if (groupBy.NodeType == ExpressionType.MemberAccess)
                return GetTermFacets();

            throw new NotSupportedException($"GroupBy must be either a Member or a Constant not {groupBy.NodeType}");
        }

        IEnumerable<IFacet> GetTermlessFacets()
        {
            if (groupBy != null) // Top level counts and count predicates will be left to main translator
            {
                if (aggregateWithoutMember)
                    yield return new FilterFacet(GroupKeyFacet, MatchAllCriteria.Instance);

                foreach (var criteria in aggregateCriteria)
                    yield return new FilterFacet(criteria.Key, criteria.Value);
            }

            foreach (var valueField in aggregateMembers.Select(member => Mapping.GetFieldName(SourceType, member)).Distinct())
                yield return new StatisticalFacet(valueField, valueField);
        }

        IEnumerable<IFacet> GetTermFacets()
        {
            var groupByField = Mapping.GetFieldName(SourceType, (MemberExpression)groupBy);
            if (aggregateWithoutMember)
                yield return new TermsFacet(GroupKeyFacet, null, size, groupByField);

            foreach (var valueField in aggregateMembers.Select(member => Mapping.GetFieldName(SourceType, member)).Distinct())
                yield return new TermsStatsFacet(valueField, groupByField, valueField, size);

            foreach (var criteria in aggregateCriteria)
                yield return new TermsFacet(criteria.Key, criteria.Value, size, groupByField);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.Name == "Key" && node.Member.DeclaringType.IsGenericOf(typeof(IGrouping<,>)))
                return VisitGroupKeyAccess(node.Type);

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Enumerable) || node.Method.DeclaringType == typeof(Queryable))
            {
                var source = node.Arguments[0];

                if (node.Method.Name == "GroupBy" && node.Arguments.Count == 2)
                {
                    groupBy = node.Arguments[1].GetLambda().Body;
                    return Visit(source);
                }

                if (node.Method.Name == "Select" && node.Arguments.Count == 2)
                {
                    var y = Visit(node.Arguments[1]).GetLambda();
                    selectProjection = Expression.Lambda(y.Body, bindingParameter);
                    return Visit(source);
                }

                if (node.Method.Name == "Take" && node.Arguments.Count == 2)
                    return VisitTake(source, node.Arguments[1]);

                // Consider whether to take the groupby operation and rebind the semantics into the projection
                // and remove it from the expression tree so that processing can continue.
                var reboundExpression = RebindAggregateOperation(node);
                if (reboundExpression != null && !source.Type.IsGenericOf(typeof(IGrouping<,>)))
                {
                    selectProjection = Expression.Lambda(reboundExpression, bindingParameter);
                    return Visit(source);
                }

                // Rebinding an individual element within a Select
                if (source is ParameterExpression)
                {
                    if (reboundExpression != null)
                        return reboundExpression;
                }
            }

            return base.VisitMethodCall(node);
        }

        Expression RebindAggregateOperation(MethodCallExpression m)
        {
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

            return null;
        }

        Expression VisitTake(Expression source, Expression takeExpression)
        {
            var takeConstant = Visit(takeExpression) as ConstantExpression;
            if (takeConstant != null)
            {
                var takeValue = (int)takeConstant.Value;
                size = size.HasValue ? Math.Min(size.GetValueOrDefault(), takeValue) : takeValue;
            }
            return Visit(source);
        }

        Expression VisitAggregateGroupPredicateOperation(Expression predicate, string operation, Type returnType)
        {
            var lambda = predicate.GetLambda();
            var body = BooleanMemberAccessBecomesEquals(lambda.Body);

            var criteriaExpression = body as CriteriaExpression;
            if (criteriaExpression == null)
                throw new NotSupportedException($"Unknown Aggregate predicate '{body}'");

            var facetName = string.Format(GroupKeyFacet + "." + (aggregateCriteria.Count + 1));
            aggregateCriteria.Add(facetName, criteriaExpression.Criteria);
            return RebindValue(facetName, operation, returnType);
        }

        Expression VisitGroupKeyAccess(Type returnType)
        {
            var getKeyExpression = Expression.Call(null, getKey, bindingParameter);
            return Expression.Convert(getKeyExpression, returnType);
        }

        Expression VisitAggregateGroupKeyOperation(string operation, Type returnType)
        {
            aggregateWithoutMember = true;
            return RebindValue(GroupKeyFacet, operation, returnType);
        }

        Expression VisitAggregateMemberOperation(Expression property, string operation, Type returnType)
        {
            var memberExpression = GetMemberExpressionFromLambda(property);
            aggregateMembers.Add(memberExpression);
            return RebindValue(Mapping.GetFieldName(SourceType, memberExpression), operation, returnType);
        }

        Expression RebindValue(string valueField, string operation, Type returnType)
        {
            var getValueExpression = Expression.Call(null, getValue, bindingParameter,
                Expression.Constant(valueField), Expression.Constant(operation), Expression.Constant(returnType));
            return Expression.Convert(getValueExpression, returnType);
        }

        static MemberExpression GetMemberExpressionFromLambda(Expression expression)
        {
            var lambda = expression.StripQuotes() as LambdaExpression;
            if (lambda == null)
                throw new NotSupportedException($"Aggregate required Lambda expression, {expression.NodeType} expression encountered.");

            var memberExpressionBody = lambda.Body as MemberExpression;
            if (memberExpressionBody == null)
                throw new NotSupportedException($"Aggregates must be specified against a member of the entity not {lambda.Body.Type}");

            return memberExpressionBody;
        }
    }
}