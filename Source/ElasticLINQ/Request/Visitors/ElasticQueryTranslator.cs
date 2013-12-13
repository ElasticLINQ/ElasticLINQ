// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Response;
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
    /// Expression visitor to translate a LINQ query into a <see cref="ElasticTranslateResult"/>
    /// that captures remote and local semantics.
    /// </summary>
    internal class ElasticQueryTranslator : ExpressionVisitor
    {
        private readonly ElasticSearchRequest searchRequest = new ElasticSearchRequest();
        private readonly ParameterExpression hitParameter = Expression.Parameter(typeof(Hit), "h");
        private readonly IElasticMapping mapping;

        private Type type;
        private Func<Hit, Object> projector;
        private Func<IList, object> finalTransform;

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
            var evaluation = PartialEvaluator.Evaluate(e);
            var aggregate = AggregateExpressionVisitor.Rebind(hitParameter, mapping, evaluation);

            Visit(aggregate);

            searchRequest.Type = mapping.GetTypeName(type);
            if (searchRequest.Filter == null && searchRequest.Query == null)
                searchRequest.Filter = mapping.GetTypeSelectionCriteria(type);

            return new ElasticTranslateResult(searchRequest, projector ?? DefaultProjector, finalTransform);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable))
                return VisitQueryableMethodCall(m);

            if (m.Method.DeclaringType == typeof(Enumerable))
                return VisitEnumerableMethodCall(m);

            if (m.Method.DeclaringType == typeof(ElasticQueryExtensions))
                return VisitElasticQueryExtensionsMethodCall(m);

            if (m.Method.DeclaringType == typeof(ElasticMethods))
                return VisitElasticMethodsMethodCall(m);

            return VisitCommonMethodCall(m);
        }

        private Expression VisitElasticMethodsMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Regexp":
                    if (m.Arguments.Count == 2)
                        return VisitRegexp(m.Arguments[0], m.Arguments[1]);
                    break;

                case "Prefix":
                    if (m.Arguments.Count == 2)
                        return VisitPrefix(m.Arguments[0], m.Arguments[1]);
                    break;
            }

            throw new NotSupportedException(string.Format("The ElasticMethods method '{0}' is not supported", m.Method.Name));
        }

        private Expression VisitPrefix(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return new CriteriaExpression(new PrefixCriteria(mapping.GetFieldName(cm.MemberExpression.Member), cm.ConstantExpression.Value));

            throw new NotSupportedException("Prefix must be between a Member and a Constant");            
        }

        private Expression VisitRegexp(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return new CriteriaExpression(new RegexpCriteria(mapping.GetFieldName(cm.MemberExpression.Member), cm.ConstantExpression.Value.ToString()));

            throw new NotSupportedException("Regexp must be between a Member and a Constant");            
        }

        private Expression VisitCommonMethodCall(MethodCallExpression m)
        {
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
            var property = Visit(match);

            if (source is ConstantExpression && property is MemberExpression)
            {
                var field = mapping.GetFieldName(((MemberExpression)property).Member);
                var containsSource = ((IEnumerable)((ConstantExpression)source).Value).Cast<object>();
                var values = new List<object>(containsSource);
                return new CriteriaExpression(TermCriteria.FromIEnumerable(field, values.Distinct()));
            }

            throw new NotSupportedException(string.Format("Unknown source '{0}' for Contains operation", source));
        }

        internal Expression VisitElasticQueryExtensionsMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Query":
                    if (m.Arguments.Count == 2)
                        return VisitQuery(m.Arguments[0], m.Arguments[1]);
                    break;

                case "QueryString":
                    if (m.Arguments.Count == 2)
                        return VisitQueryString(m.Arguments[0], m.Arguments[1]);
                    break;

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

        private Expression VisitQueryString(Expression source, Expression queryExpression)
        {
            var constantQueryExpression = (ConstantExpression)queryExpression;
            var criteriaExpression = new CriteriaExpression(new QueryStringCriteria(constantQueryExpression.Value.ToString()));
            searchRequest.Query = ApplyCriteria(searchRequest.Query, criteriaExpression.Criteria);

            return Visit(source);
        }

        internal Expression VisitQueryableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Select":
                    if (m.Arguments.Count == 2)
                        return VisitSelect(m.Arguments[0], m.Arguments[1]);
                    break;

                case "First":
                case "FirstOrDefault":
                    if (m.Arguments.Count == 1)
                        return VisitFirst(m.Arguments[0], null, m.Method.Name);
                    if (m.Arguments.Count == 2)
                        return VisitFirst(m.Arguments[0], m.Arguments[1], m.Method.Name);
                    break;

                case "Single":
                case "SingleOrDefault":
                    if (m.Arguments.Count == 1)
                        return VisitSingle(m.Arguments[0], null, m.Method.Name);
                    if (m.Arguments.Count == 2)
                        return VisitSingle(m.Arguments[0], m.Arguments[1], m.Method.Name);
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

        private Expression VisitFirst(Expression source, Expression predicate, string methodName)
        {
            searchRequest.Size = 1;
            finalTransform = o => ElasticResponseMaterializer.First(o, methodName.EndsWith("Default"));

            return predicate != null
                ? VisitWhere(source, predicate)
                : Visit(source);
        }

        private Expression VisitSingle(Expression source, Expression predicate, string methodName)
        {
            searchRequest.Size = 2;
            finalTransform = o => ElasticResponseMaterializer.Single(o, methodName.EndsWith("Default"));

            return predicate != null
                ? VisitWhere(source, predicate)
                : Visit(source);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value is IQueryable)
                type = ((IQueryable)c.Value).ElementType;

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
                        var subExpression = Visit(node.Operand) as CriteriaExpression;
                        if (subExpression != null)
                            return new CriteriaExpression(NotCriteria.Create(subExpression.Criteria));
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

            throw new NotSupportedException(string.Format("The MemberInfo '{0}' is not supported", m.Member.Name));
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }

        private Expression VisitQuery(Expression source, Expression predicate)
        {
            var lambda = (LambdaExpression)StripQuotes(predicate);
            var body = BooleanMemberAccessBecomesEquals(Visit(lambda.Body));

            var criteriaExpression = body as CriteriaExpression;
            if (criteriaExpression == null)
                throw new NotSupportedException(string.Format("Unknown Query predicate '{0}'", body));

            searchRequest.Query = ApplyCriteria(searchRequest.Query, criteriaExpression.Criteria);

            return Visit(source);
        }

        private Expression VisitWhere(Expression source, Expression predicate)
        {
            var lambda = (LambdaExpression)StripQuotes(predicate);
            var body = BooleanMemberAccessBecomesEquals(Visit(lambda.Body));

            var criteriaExpression = body as CriteriaExpression;
            if (criteriaExpression == null)
                throw new NotSupportedException(String.Format("Unknown Where predicate '{0}'", body));

            searchRequest.Filter = ApplyCriteria(searchRequest.Filter, criteriaExpression.Criteria);

            return Visit(source);
        }

        private static ICriteria ApplyCriteria(ICriteria currentRoot, ICriteria newCriteria)
        {
            return currentRoot == null
                ? newCriteria
                : AndCriteria.Combine(currentRoot, newCriteria);
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
                    throw new NotSupportedException(string.Format("The binary expression '{0}' is not supported", b.NodeType));
            }
        }

        private Expression VisitAndAlso(BinaryExpression b)
        {
            var criteria = AssertExpressionsOfType<CriteriaExpression>(b.Left, b.Right).Select(f => f.Criteria).ToArray();
            return new CriteriaExpression(AndCriteria.Combine(criteria));
        }

        private Expression VisitOrElse(BinaryExpression b)
        {
            var criteria = AssertExpressionsOfType<CriteriaExpression>(b.Left, b.Right).Select(f => f.Criteria).ToArray();
            return new CriteriaExpression(OrCriteria.Combine(criteria));
        }

        private IEnumerable<T> AssertExpressionsOfType<T>(params Expression[] expressions) where T : Expression
        {
            foreach (var expression in expressions.Select(BooleanMemberAccessBecomesEquals))
            {
                var reducedExpression = expression is CriteriaExpression ? expression : Visit(expression);
                if ((reducedExpression as T) == null)
                    throw new NotSupportedException(string.Format("Unexpected binary expression '{0}'", reducedExpression));

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
                    throw new NotSupportedException(string.Format("The binary expression '{0}' is not supported", b.NodeType));
            }
        }

        private Expression CreateExists(ConstantMemberPair cm, bool positiveTest)
        {
            var fieldName = mapping.GetFieldName(UnwrapNullableMethodExpression(cm.MemberExpression));

            var value = cm.ConstantExpression.Value ?? false;

            if (value.Equals(positiveTest))
                return new CriteriaExpression(new ExistsCriteria(fieldName));

            if (value.Equals(!positiveTest))
                return new CriteriaExpression(new MissingCriteria(fieldName));

            throw new NotSupportedException("A null test Expression must have a member being compared to a bool or null");
        }

        private Expression VisitEquals(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return cm.IsNullTest
                    ? CreateExists(cm, true)
                    : new CriteriaExpression(new TermCriteria(mapping.GetFieldName(cm.MemberExpression.Member), cm.ConstantExpression.Value));

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
                    : new CriteriaExpression(NotCriteria.Create(new TermCriteria(mapping.GetFieldName(cm.MemberExpression.Member), cm.ConstantExpression.Value)));

            throw new NotSupportedException("A not-equal expression must consist of a constant and a member");
        }

        private Expression VisitRange(ExpressionType t, Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
            {
                var field = mapping.GetFieldName(cm.MemberExpression.Member);
                return new CriteriaExpression(new RangeCriteria(field, ExpressionTypeToRangeType(t), cm.ConstantExpression.Value));
            }

            throw new NotSupportedException("A range must consist of a constant and a member");
        }

        private static RangeComparison ExpressionTypeToRangeType(ExpressionType expressionType)
        {
            switch (expressionType)
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

            throw new ArgumentOutOfRangeException("expressionType");
        }

        private Expression VisitOrderBy(Expression source, Expression orderByExpression, bool ascending)
        {
            var lambda = (LambdaExpression)StripQuotes(orderByExpression);
            var final = Visit(lambda.Body) as MemberExpression;
            if (final != null)
            {
                var fieldName = mapping.GetFieldName(final.Member);
                var ignoreUnmapped = TypeHelper.IsNullableType(final.Type); // Consider a config switch?
                searchRequest.SortOptions.Insert(0, new SortOption(fieldName, ascending, ignoreUnmapped));
            }

            return Visit(source);
        }

        private Expression VisitOrderByScore(Expression source, bool ascending)
        {
            searchRequest.SortOptions.Insert(0, new SortOption("_score", ascending));
            return Visit(source);
        }

        private Expression VisitSelect(Expression source, Expression selectExpression)
        {
            var lambda = (LambdaExpression)StripQuotes(selectExpression);
            var selectBody = Visit(lambda.Body);

            if (selectBody is MemberExpression)
                return VisitSelectMember(source, (MemberExpression)selectBody);

            if (selectBody is NewExpression)
                return VisitSelectNew(source, (NewExpression)selectBody, lambda.Parameters);

            if (selectBody is MethodCallExpression)
                return VisitSelectMethodCall(source, (MethodCallExpression)selectBody, lambda.Parameters);

            return Visit(source);
        }

        private Expression VisitSelectMember(Expression source, MemberExpression selectExpression)
        {
            RebindPropertiesAndElasticFields(selectExpression);
            return Visit(source);
        }

        private Expression VisitSelectMethodCall(Expression source, MethodCallExpression selectExpression, IEnumerable<ParameterExpression> parameters)
        {
            var entityParameter = selectExpression.Arguments.SingleOrDefault(parameters.Contains) as ParameterExpression;
            if (entityParameter == null)
                RebindPropertiesAndElasticFields(selectExpression);
            else
                RebindElasticFieldsAndChainProjector(selectExpression, entityParameter);

            return Visit(source);
        }

        private Expression VisitSelectNew(Expression source, NewExpression selectExpression, IEnumerable<ParameterExpression> parameters)
        {
            var entityParameter = selectExpression.Arguments.SingleOrDefault(parameters.Contains) as ParameterExpression;
            if (entityParameter == null)
                RebindPropertiesAndElasticFields(selectExpression);
            else
                RebindElasticFieldsAndChainProjector(selectExpression, entityParameter);

            return Visit(source);
        }

        /// <summary>
        /// We are using the whole entity in a new select projection. Rebind any ElasticField references to JObject
        /// and ensure the entity parameter is a freshly materialized entity object from our default materializer.
        /// </summary>
        /// <param name="selectExpression">Select expression to rebind.</param>
        /// <param name="entityParameter">Parameter that references the whole entity.</param>
        private void RebindElasticFieldsAndChainProjector(Expression selectExpression, ParameterExpression entityParameter)
        {
            var projection = ElasticFieldsRebindingExpressionVisitor.Rebind(hitParameter, mapping, selectExpression);
            var compiled = Expression.Lambda(projection, entityParameter, hitParameter).Compile();
            projector = h => compiled.DynamicInvoke(DefaultProjector(h), h);
        }

        /// <summary>
        /// We are using just some properties of the entity. Rewrite the properties as JObject field lookups and
        /// record all the field names used to ensure we only select those.
        /// </summary>
        /// <param name="selectExpression">Select expression to rebind.</param>
        private void RebindPropertiesAndElasticFields(Expression selectExpression)
        {
            var projection = MemberProjectionExpressionVisitor.Rebind(hitParameter, mapping, selectExpression);
            var compiled = Expression.Lambda(projection.Materializer, hitParameter).Compile();
            projector = h => compiled.DynamicInvoke(h);
            searchRequest.Fields.AddRange(projection.FieldNames);
        }

        private Expression VisitSkip(Expression source, Expression skipExpression)
        {
            var skipConstant = Visit(skipExpression) as ConstantExpression;
            if (skipConstant != null)
                searchRequest.From = (int)skipConstant.Value;
            return Visit(source);
        }

        private Expression VisitTake(Expression source, Expression takeExpression)
        {
            var takeConstant = Visit(takeExpression) as ConstantExpression;
            if (takeConstant != null)
                searchRequest.Size = (int)takeConstant.Value;
            return Visit(source);
        }

        private Func<Hit, Object> DefaultProjector
        {
            get { return hit => mapping.GetObjectSource(type, hit).ToObject(type); }
        }
    }
}