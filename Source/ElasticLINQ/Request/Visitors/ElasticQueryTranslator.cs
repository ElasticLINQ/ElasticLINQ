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

    /// <summary>
    /// Expression visitor to translate a LINQ query into ElasticSearch request.
    /// </summary>
    internal class ElasticQueryTranslator : ExpressionVisitor
    {
        private readonly IElasticMapping mapping;
        private Projection projection;
        private ParameterExpression projectParameter;

        private string type;
        private int skip;
        private int? take;
        private readonly List<string> fields = new List<string>();
        private readonly List<SortOption> sortOptions = new List<SortOption>();
        private readonly Dictionary<string, IReadOnlyList<object>> termCriteria = new Dictionary<string, IReadOnlyList<object>>();

        public ElasticQueryTranslator(IElasticMapping mapping)
        {
            this.mapping = mapping;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable))
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
                    case "from":
                        if (m.Arguments.Count == 2)
                            return VisitSkip(m.Arguments[0], m.Arguments[1]);
                        break;
                    case "size":
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
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
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

        internal ElasticTranslateResult Translate(Expression e)
        {
            projectParameter = Expression.Parameter(typeof(JObject), "row");

            Visit(e);

            if (projection != null)
                fields.AddRange(projection.Fields);

            return new ElasticTranslateResult
            {
                SearchRequest = new ElasticSearchRequest(type, skip, take, fields, sortOptions, termCriteria),
                Projector = projection != null ? Expression.Lambda(projection.Selector, projectParameter) : null
            };
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
            inWhereCondition = true;

            var lambda = (LambdaExpression)StripQuotes(predicate);
            Visit(lambda);

            inWhereCondition = false;

            return Visit(source);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Visit(b.Left);
            var operation = VisitComparisonOperation(b);
            Visit(b.Right);

            if (whereMemberInfos.Any() && whereConstants.Any())
                AddTermCriterion(whereMemberInfos.Pop(), operation, whereConstants.Pop());

            return b;
        }

        private void AddTermCriterion(MemberInfo memberInfo, string operation, ConstantExpression value)
        {
            var fieldName = mapping.GetFieldName(memberInfo);

            IReadOnlyList<object> existingCriteria;
            termCriteria.TryGetValue(fieldName, out existingCriteria);

            termCriteria[fieldName] = new List<object>(existingCriteria ?? Enumerable.Empty<object>())
                { value.Value }
                .AsReadOnly();
        }

        private static string VisitComparisonOperation(BinaryExpression b)
        {
            // TODO: Make these filter or query specific
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Equal:
                    return "";
                case ExpressionType.NotEqual:
                    return "NOT";
                case ExpressionType.LessThan:
                    return "lt";
                case ExpressionType.LessThanOrEqual:
                    return "lte";
                case ExpressionType.GreaterThan:
                    return "gt";
                case ExpressionType.GreaterThanOrEqual:
                    return "gte";
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
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
            projection = new ProjectionVisitor(projectParameter, mapping).ProjectColumns(selectBody);
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