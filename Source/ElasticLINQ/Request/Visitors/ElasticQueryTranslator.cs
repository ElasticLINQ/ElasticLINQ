// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Filters;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    internal class ElasticTranslateResult
    {
        public ElasticSearchRequest SearchRequest;
        public LambdaExpression Projector;
    }

    /// <summary>
    /// Expression visitor to translate a LINQ query into ElasticSearch request.
    /// </summary>
    internal class ElasticQueryTranslator : ExpressionVisitor
    {
        private readonly IElasticMapping mapping;
        private Projection projection;
        private ParameterExpression projectionParameter;

        private readonly List<string> fields = new List<string>();
        private readonly List<SortOption> sortOptions = new List<SortOption>();
        private string type;
        private int skip;
        private int? take;
        private IFilter topFilter;

        public ElasticQueryTranslator(IElasticMapping mapping)
        {
            this.mapping = mapping;
        }

        internal ElasticTranslateResult Translate(Expression e)
        {
            projectionParameter = Expression.Parameter(typeof(JObject), "r");

            Visit(e);

            var result = new ElasticTranslateResult();
            if (projection != null)
            {
                result.Projector = Expression.Lambda(projection.Selector, projectionParameter);
                fields.AddRange(projection.Fields);
            }

            result.SearchRequest = new ElasticSearchRequest(type, skip, take, fields, sortOptions, topFilter);
            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable))
                return VisitQueryableMethodCall(m);

            if (m.Method.DeclaringType == typeof(Enumerable))
                return VisitEnumerableMethodCall(m);

            if (m.Method.DeclaringType == typeof(ElasticQueryExtensions))
                return VisitElasticMethodCall(m);

            switch (m.Method.Name)
            {
                case "Equals":
                    if (m.Arguments.Count == 1)
                        return VisitEquals(Visit(m.Object), Visit(m.Arguments[0]));
                    if (m.Arguments.Count == 2)
                        return VisitEquals(Visit(m.Arguments[0]), Visit(m.Arguments[1]));

                    break;

                case "Contains":
                    if (TypeHelper.FindIEnumerable(m.Method.DeclaringType) != null)
                        return VisitEnumerableContainsMethodCall(m.Object, m.Arguments[0]);
                    break;

                case "Create":
                    return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        private Expression VisitEnumerableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    if (m.Arguments.Count == 2)
                        return VisitEnumerableContainsMethodCall(m.Arguments[0], m.Arguments[1]);
                    break;
            }

            throw new NotSupportedException(string.Format("The Enumerable method '{0}' is not supported", m.Method.Name));
        }

        private Expression VisitEnumerableContainsMethodCall(Expression source, Expression match)
        {
            var matched = Visit(match);

            if (source is ConstantExpression && matched is MemberExpression)
            {
                var field = mapping.GetFieldName(((MemberExpression)matched).Member);
                var containsSource = ((IEnumerable)((ConstantExpression)source).Value).Cast<object>();
                var values = new List<object>(containsSource);
                return new FilterExpression(new TermFilter(field, values.Distinct().ToArray()));
            }

            throw new NotImplementedException("Unknown source for Contains");
        }

        internal Expression VisitElasticMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "OrderByScore":
                case "OrderByScoreDescending":
                case "ThenByScore":
                case "ThenByScoreDecending":
                    if (m.Arguments.Count == 1)
                        return VisitOrderByScore(m.Arguments[0], !m.Method.Name.EndsWith("Descending"));
                    break;
            }

            throw new NotSupportedException(string.Format("The ElasticQuery method '{0}' is not supported", m.Method.Name));
        }

        internal Expression VisitQueryableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Select":
                    if (m.Arguments.Count == 2)
                        return VisitSelect(m.Arguments[0], m.Arguments[1]);
                    break;
                case "Where":
                    if (m.Arguments.Count == 2)
                        return VisitWhere(m.Arguments[0], m.Arguments[1]);
                    break;
                case "Skip":
                    if (m.Arguments.Count == 2)
                        return VisitSkip(m.Arguments[0], m.Arguments[1]);
                    break;
                case "Take":
                    if (m.Arguments.Count == 2)
                        return VisitTake(m.Arguments[0], m.Arguments[1]);
                    break;
                case "OrderBy":
                case "OrderByDescending":
                    if (m.Arguments.Count == 2)
                        return VisitOrderBy(m.Arguments[0], m.Arguments[1], m.Method.Name == "OrderBy");
                    break;
                case "ThenBy":
                case "ThenByDescending":
                    if (m.Arguments.Count == 2)
                        return VisitOrderBy(m.Arguments[0], m.Arguments[1], m.Method.Name == "ThenBy");
                    break;
            }

            throw new NotSupportedException(string.Format("The Queryable method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value is IQueryable)
                SetType(((IQueryable)c.Value).ElementType);

            return c;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    return node.Operand;

                case ExpressionType.Not:
                {
                    var subExpression = Visit(node.Operand) as FilterExpression;
                    if (subExpression != null)
                        return new FilterExpression(new NotFilter(subExpression.Filter));
                    break;
                }
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException(string.Format("The memberInfo '{0}' is not supported", m.Member.Name));

            return m;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }

        private Expression VisitWhere(Expression source, Expression predicate)
        {
            var lambda = (LambdaExpression)StripQuotes(predicate);
            var criteriaExpression = Visit(lambda.Body);

            if (criteriaExpression is FilterExpression)
                topFilter = CombineFilter(((FilterExpression)criteriaExpression).Filter);
            else
                throw new NotSupportedException(String.Format("Unknown where predicate {0}", criteriaExpression));

            return Visit(source);
        }

        private IFilter CombineFilter(IFilter thisFilter)
        {
            if (topFilter == null)
                return thisFilter;

            if (topFilter is AndFilter)
                return AndFilter.Combine(((AndFilter)topFilter).Filters.Concat(new[] { thisFilter }).ToArray());

            return AndFilter.Combine(topFilter, thisFilter);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.OrElse:
                    return VisitOrElse(b);

                case ExpressionType.AndAlso:
                    return VisitAndAlso(b);

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return VisitComparisonBinary(b);

                default:
                    throw new NotImplementedException(String.Format("Don't yet know {0}", b.NodeType));
            }
        }

        private Expression VisitAndAlso(BinaryExpression b)
        {
            var filters = AssertExpressionsOfType<FilterExpression>(b.Left, b.Right).Select(f => f.Filter).ToArray();
            return new FilterExpression(AndFilter.Combine(filters));
        }

        private Expression VisitOrElse(BinaryExpression b)
        {
            var filters = AssertExpressionsOfType<FilterExpression>(b.Left, b.Right).Select(f => f.Filter).ToArray();
            return new FilterExpression(OrFilter.Combine(filters));
        }

        private IEnumerable<T> AssertExpressionsOfType<T>(params Expression[] expressions) where T : Expression
        {
            foreach (var expression in expressions.Select(Visit))
                if ((expression as T) == null)
                    throw new NotImplementedException(string.Format("Unknown binary expression {0}", expression));
                else
                    yield return (T)expression;
        }

        private Expression VisitComparisonBinary(BinaryExpression b)
        {
            var left = Visit(b.Left);
            var right = Visit(b.Right);

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    return VisitEquals(left, right);

                case ExpressionType.NotEqual:
                    return VisitNotEqual(left, right);

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return VisitRange(b.NodeType, left, right);

                default:
                    throw new NotImplementedException(String.Format("Don't yet know {0}", b.NodeType));
            }
        }

        private Expression VisitNotEqual(Expression left, Expression right)
        {
            var o = OrganizeConstantAndMember(left, right);

            if (o != null) 
                return new FilterExpression(new NotFilter(new TermFilter(mapping.GetFieldName(o.Item2.Member), o.Item1.Value)));

            throw new NotSupportedException("A NotEqual Expression must consist of a constant and a member");
        }

        private Expression VisitRange(ExpressionType t, Expression left, Expression right)
        {
            var o = OrganizeConstantAndMember(left, right);

            if (o != null)
            {
                var field = mapping.GetFieldName(o.Item2.Member);
                var specification = new RangeSpecificationFilter(ExpressionTypeToRangeType(t), o.Item1.Value);
                return new FilterExpression(new RangeFilter(field, specification));
            }

            throw new NotSupportedException("A range must consist of a constant and a member");
        }

        private static Tuple<ConstantExpression, MemberExpression> OrganizeConstantAndMember(Expression a, Expression b)
        {
            if (a is ConstantExpression && b is MemberExpression)
                return Tuple.Create((ConstantExpression)a, (MemberExpression)b);

            if (b is ConstantExpression && a is MemberExpression)
                return Tuple.Create((ConstantExpression)b, (MemberExpression)a);

            return null;
        }

        private static string ExpressionTypeToRangeType(ExpressionType t)
        {
            switch (t)
            {
                case ExpressionType.GreaterThan:
                    return "gt";
                case ExpressionType.GreaterThanOrEqual:
                    return "gte";
                case ExpressionType.LessThan:
                    return "lt";
                case ExpressionType.LessThanOrEqual:
                    return "lte";
            }

            throw new ArgumentOutOfRangeException("t");
        }

        private Expression VisitEquals(Expression left, Expression right)
        {
            var o = OrganizeConstantAndMember(left, right);
            if (o != null)
                return new FilterExpression(new TermFilter(mapping.GetFieldName(o.Item2.Member), o.Item1.Value));

            throw new NotSupportedException("Equality must be between a Member and a Constant");
        }

        private Expression VisitOrderBy(Expression source, Expression orderByExpression, bool ascending)
        {
            var lambda = (LambdaExpression)StripQuotes(orderByExpression);
            var final = Visit(lambda.Body) as MemberExpression;
            if (final != null)
            {
                var fieldName = mapping.GetFieldName(final.Member);
                sortOptions.Insert(0, new SortOption(fieldName, ascending));
            }

            return Visit(source);
        }

        private Expression VisitOrderByScore(Expression source, bool ascending)
        {
            sortOptions.Insert(0, new SortOption("_score", ascending));
            return Visit(source);
        }

        private Expression VisitSelect(Expression source, Expression selectExpression)
        {
            var lambda = (LambdaExpression)StripQuotes(selectExpression);
            var selectBody = Visit(lambda.Body);

            if (selectBody is NewExpression || selectBody is MemberExpression || selectBody is MethodCallExpression)
                VisitSelectNew(selectBody);

            return Visit(source);
        }

        private void VisitSelectNew(Expression selectBody)
        {
            projection = ProjectionVisitor.ProjectColumns(projectionParameter, mapping, selectBody);
        }

        private Expression VisitSkip(Expression source, Expression skipExpression)
        {
            var skipConstant = Visit(skipExpression) as ConstantExpression;
            if (skipConstant != null)
                skip = (int)skipConstant.Value;
            return Visit(source);
        }

        private Expression VisitTake(Expression source, Expression takeExpression)
        {
            var takeConstant = Visit(takeExpression) as ConstantExpression;
            if (takeConstant != null)
                take = (int)takeConstant.Value;
            return Visit(source);
        }

        private void SetType(Type elementType)
        {
            type = elementType == typeof(object) ? null : mapping.GetTypeName(elementType);
        }
    }
}