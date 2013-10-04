// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Mapping;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    internal class ElasticTranslateResult
    {
        public ElasticSearchRequest SearchRequest;
        public LambdaExpression Projector;
    }

    internal class FilterExpression : Expression
    {
        private readonly Filter filter;

        public FilterExpression(Filter filter)
        {
            this.filter = filter;
        }

        public Filter Filter { get { return filter; } }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)10000; }
        }

        public override Type Type
        {
            get { return typeof (bool); }
        }
    }

    /// <summary>
    /// Expression visitor to translate a LINQ query into ElasticSearch request.
    /// </summary>
    internal class ElasticQueryTranslator : ExpressionVisitor
    {
        private readonly IElasticMapping mapping;
        private Projection projection;
        private ParameterExpression projectParameter;

        private readonly List<string> fields = new List<string>();
        private readonly List<SortOption> sortOptions = new List<SortOption>();
        private string type;
        private int skip;
        private int? take;
        private FilterExpression filterExpression;

        public ElasticQueryTranslator(IElasticMapping mapping)
        {
            this.mapping = mapping;
        }

        internal ElasticTranslateResult Translate(Expression e)
        {
            projectParameter = Expression.Parameter(typeof(JObject), "row");

            Visit(e);

            if (projection != null)
                fields.AddRange(projection.Fields);

            return new ElasticTranslateResult
            {
                SearchRequest = new ElasticSearchRequest(type, skip, take, fields, sortOptions, filterExpression.Filter),
                Projector = projection != null ? Expression.Lambda(projection.Selector, projectParameter) : null
            };
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable))
                return VisitQueryableMethodCall(m);

            if (m.Method.DeclaringType == typeof(ElasticQueryExtensions))
                return VisitElasticMethodCall(m);

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
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

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
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

            throw new NotSupportedException(string.Format("The method '{0}' of Queryable is not supported", m.Method.Name));
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value is IQueryable)
                SetType(((IQueryable)c.Value).ElementType);

            if (inWhereCondition)
                whereConstants.Push(c);

            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (inWhereCondition)
                whereMemberInfos.Push(m.Member);

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

        private bool inWhereCondition;
        private readonly Stack<MemberInfo> whereMemberInfos = new Stack<MemberInfo>();
        private readonly Stack<ConstantExpression> whereConstants = new Stack<ConstantExpression>();

        private Expression VisitWhere(Expression source, Expression predicate)
        {
            inWhereCondition = true; // TODO: Replace with context-sensitive stack

            var lambda = (LambdaExpression)StripQuotes(predicate);
            Visit(lambda);

            inWhereCondition = false;

            return Visit(source);
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
                    return VisitComparisonBinary(b);

                default:
                    throw new NotImplementedException(String.Format("Don't yet know {0}", b.NodeType));
            }
        }

        private Expression VisitAndAlso(BinaryExpression b)
        {
            var left = Visit(b.Left) as FilterExpression;
            var right = Visit(b.Right) as FilterExpression;

            if (left == null || right == null)
                throw new NotImplementedException("Unknown binary expressions");

            filterExpression = new FilterExpression(new AndFilter(left.Filter, right.Filter));
            return filterExpression;
        }

        private Expression VisitOrElse(BinaryExpression b)
        {
            var left = Visit(b.Left) as FilterExpression;
            var right = Visit(b.Right) as FilterExpression;

            if (left == null || right == null)
                throw new NotImplementedException("Unknown binary expressions");

            filterExpression = new FilterExpression(OrFilter.Combine(left.Filter, right.Filter));
            return filterExpression;
        }

        private Expression VisitComparisonBinary(BinaryExpression b)
        {
            Visit(b.Left);
            Visit(b.Right);

            var haveMemberAndConstant = whereMemberInfos.Any() && whereConstants.Any();

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    {
                        if (haveMemberAndConstant)
                        {
                            var field = mapping.GetFieldName(whereMemberInfos.Pop());
                            var expression = new FilterExpression(new TermFilter(field, whereConstants.Pop().Value));
                            if (filterExpression == null)
                                filterExpression = expression;
                            return expression;
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException(String.Format("Don't yet know {0}", b.NodeType));
            }

            return b;
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

            if (selectBody is NewExpression || selectBody is MemberExpression)
                VisitSelectNew(selectBody);

            return Visit(source);
        }

        private void VisitSelectNew(Expression selectBody)
        {
            projection = ProjectionVisitor.ProjectColumns(projectParameter, mapping, selectBody);
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