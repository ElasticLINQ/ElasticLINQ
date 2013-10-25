// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Filters;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Expression visitor to translate a LINQ query into ElasticSearch request.
    /// </summary>
    internal class ElasticQueryTranslator : ExpressionVisitor
    {
        private readonly IElasticMapping mapping;
        private readonly ParameterExpression projectionParameter = Expression.Parameter(typeof(Hit), "h");
        private readonly List<string> fields = new List<string>();
        private readonly List<SortOption> sortOptions = new List<SortOption>();

        private Func<Hit, Object> projector;
        private Type type;
        private int skip;
        private int? take;
        private IFilter topFilter;

        private ElasticQueryTranslator(IElasticMapping mapping)
        {
            this.mapping = new ElasticFieldsMappingWrapper(mapping);
        }

        internal static ElasticTranslateResult Translate(IElasticMapping mapping, Expression e)
        {
            return new ElasticQueryTranslator(mapping).Translate(e);
        }

        private ElasticTranslateResult Translate(Expression e)
        {
            var evaluated = PartialEvaluator.Evaluate(e);
            Visit(evaluated);
            var searchRequest = new ElasticSearchRequest(mapping.GetTypeName(type), skip, take, fields, sortOptions, topFilter);
            return new ElasticTranslateResult(searchRequest, projector ?? DefaultProjector);
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
                return new FilterExpression(TermFilter.FromIEnumerable(field, values.Distinct()));
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
                case "ThenByScoreDescending":
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
                            return new FilterExpression(NotFilter.Create(subExpression.Filter));
                        break;
                    }
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(ElasticFields))
                return m;

            switch (m.Expression.NodeType)
            {
                case ExpressionType.Parameter:
                    return m;

                case ExpressionType.MemberAccess:
                    if (m.Member.Name == "HasValue" && TypeHelper.IsNullableType(m.Member.DeclaringType))
                        return m;
                    break;
            }

            throw new NotSupportedException(String.Format("The MemberInfo '{0}' is not supported", m.Member.Name));
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
            var body = BooleanMemberAccessBecomesEquals(Visit(lambda.Body));

            if (body is FilterExpression)
                topFilter = AddFilter(((FilterExpression)body).Filter);
            else
                throw new NotSupportedException(String.Format("Unknown where predicate '{0}'", body));

            return Visit(source);
        }

        private IFilter AddFilter(IFilter thisFilter)
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
            foreach (var expression in expressions.Select(BooleanMemberAccessBecomesEquals))
            {
                var reducedExpression = expression is FilterExpression ? expression : Visit(expression);
                if ((reducedExpression as T) == null)
                    throw new NotImplementedException(string.Format("Unknown binary expression {0}", reducedExpression));
                
                yield return (T)reducedExpression;
            }
        }

        private Expression BooleanMemberAccessBecomesEquals(Expression e)
        {
            var wasNegative = e.NodeType == ExpressionType.Not;

            if (e is UnaryExpression)
                e = Visit(((UnaryExpression)e).Operand);

            if (e is MemberExpression && e.Type == typeof(bool))
                return Visit(Expression.Equal(e, Expression.Constant(!wasNegative)));

            if (wasNegative && e is FilterExpression)
                return new FilterExpression(NotFilter.Create(((FilterExpression)e).Filter));

            return e;
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

        private Expression CreateExists(ConstantMemberPair cm, bool positiveTest)
        {
            var fieldName = mapping.GetFieldName(UnwrapNullableMethodExpression(cm.MemberExpression));

            var value = cm.ConstantExpression.Value ?? false;

            if (value.Equals(positiveTest))
                return new FilterExpression(new ExistsFilter(fieldName));

            if (value.Equals(!positiveTest))
                return new FilterExpression(new MissingFilter(fieldName));

            throw new NotSupportedException("A null test Expression must consist a member and be compared to a bool or null");
        }

        private Expression VisitEquals(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return cm.IsNullTest
                    ? CreateExists(cm, true)
                    : new FilterExpression(new TermFilter(mapping.GetFieldName(cm.MemberExpression.Member), cm.ConstantExpression.Value));

            throw new NotSupportedException("Equality must be between a Member and a Constant");
        }

        private static MemberInfo UnwrapNullableMethodExpression(MemberExpression m)
        {
            if (m.Expression is MemberExpression)
                return ((MemberExpression)(m.Expression)).Member;

            return m.Member;
        }

        private Expression VisitNotEqual(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return cm.IsNullTest
                    ? CreateExists(cm, false)
                    : new FilterExpression(NotFilter.Create(new TermFilter(mapping.GetFieldName(cm.MemberExpression.Member), cm.ConstantExpression.Value)));

            throw new NotSupportedException("A NotEqual Expression must consist of a constant and a member");
        }

        private Expression VisitRange(ExpressionType t, Expression left, Expression right)
        {
            var o = ConstantMemberPair.Create(left, right);

            if (o != null)
            {
                var field = mapping.GetFieldName(o.MemberExpression.Member);
                return new FilterExpression(new RangeFilter(field, ExpressionTypeToRangeType(t), o.ConstantExpression.Value));
            }

            throw new NotSupportedException("A range must consist of a constant and a member");
        }

        private static RangeComparison ExpressionTypeToRangeType(ExpressionType t)
        {
            switch (t)
            {
                case ExpressionType.GreaterThan:
                    return RangeComparison.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return RangeComparison.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return RangeComparison.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return RangeComparison.LessThanOrEqual;
            }

            throw new ArgumentOutOfRangeException("t");
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
            {
                var projection = ProjectionExpressionVisitor.ProjectColumns(projectionParameter, mapping, selectBody);
                fields.AddRange(projection.FieldNames);
                var compiled = Expression.Lambda(projection.Materialization, projectionParameter).Compile();
                projector = h => compiled.DynamicInvoke(h);
            }

            return Visit(source);
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

        private Func<Hit, Object> DefaultProjector
        {
            get { return hit => mapping.GetObjectSource(type, hit).ToObject(type); }
        }

        private void SetType(Type elementType)
        {
            type = elementType;
        }
    }
}