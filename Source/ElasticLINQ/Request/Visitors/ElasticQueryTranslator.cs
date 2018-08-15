// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;
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
    class ElasticQueryTranslator : CriteriaExpressionVisitor
    {
        readonly SearchRequest searchRequest = new SearchRequest();

        Type finalItemType;
        Func<Hit, object> itemProjector;
        IElasticMaterializer materializer;

        ElasticQueryTranslator(IElasticMapping mapping, Type sourceType)
            : base(mapping, sourceType)
        {
        }

        internal static ElasticTranslateResult Translate(IElasticMapping mapping, Expression e)
        {
            return new ElasticQueryTranslator(mapping, FindSourceType(e)).Translate(e);
        }

        static Type FindSourceType(Expression e)
        {
            var sourceQuery = QuerySourceExpressionVisitor.FindSource(e);
            if (sourceQuery == null)
                throw new NotSupportedException("Unable to identify an IQueryable source for this query.");
            return sourceQuery.ElementType;
        }

        ElasticTranslateResult Translate(Expression e)
        {
            var evaluated = PartialEvaluator.Evaluate(e);
            CompleteHitTranslation(evaluated);

            searchRequest.Query = ConstantCriteriaFilterReducer.Reduce(searchRequest.Query);
            ApplyTypeSelectionCriteria();

            return new ElasticTranslateResult(searchRequest, materializer);
        }

        void ApplyTypeSelectionCriteria()
        {
            var typeCriteria = Mapping.GetTypeSelectionCriteria(SourceType);

            searchRequest.Query = searchRequest.Query == null || searchRequest.Query == ConstantCriteria.True
                ? typeCriteria
                : AndCriteria.Combine(typeCriteria, searchRequest.Query);
        }

        void CompleteHitTranslation(Expression evaluated)
        {
            Visit(evaluated);
            searchRequest.DocumentType = Mapping.GetDocumentType(SourceType);

            if (materializer == null)
                materializer = new ListHitsElasticMaterializer(itemProjector ?? DefaultItemProjector, finalItemType ?? SourceType);
            else if (materializer is ChainMaterializer && ((ChainMaterializer)materializer).Next == null)
                ((ChainMaterializer)materializer).Next = new ListHitsElasticMaterializer(itemProjector ?? DefaultItemProjector, finalItemType ?? SourceType);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
                return VisitQueryableMethodCall(node);

            if (node.Method.DeclaringType == typeof(ElasticQueryExtensions))
                return VisitElasticQueryExtensionsMethodCall(node);

            if (node.Method.DeclaringType == typeof(ElasticMethods))
                return VisitElasticMethodsMethodCall(node);

            return base.VisitMethodCall(node);
        }

        internal Expression VisitElasticQueryExtensionsMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "QueryString":
                    if (m.Arguments.Count == 2)
                        return VisitQueryString(m.Arguments[0], m.Arguments[1]);
                    if (m.Arguments.Count == 3)
                        return VisitQueryString(m.Arguments[0], m.Arguments[1], m.Arguments[2]);
                    break;

                case "OrderByScore":
                case "OrderByScoreDescending":
                case "ThenByScore":
                case "ThenByScoreDescending":
                    if (m.Arguments.Count == 1)
                        return VisitOrderByScore(m.Arguments[0], !m.Method.Name.EndsWith("Descending", StringComparison.Ordinal));
                    break;

                case "MinScore":
                    if (m.Arguments.Count == 2)
                        return VisitMinimumScore(m.Arguments[0], m.Arguments[1]);
                    break;
                case "Highlight":
                    if (m.Arguments.Count == 3)
                        return VisitHighlight(m.Arguments[0], m.Arguments[1], m.Arguments[2]);
                    break;
            }

            throw new NotSupportedException($"ElasticQuery.{m.Method.Name} method is not supported");
        }

        Expression VisitHighlight(Expression source, Expression highlightExpression, Expression configExpression)
        {
            var unaryExpression = highlightExpression as UnaryExpression;
            if (unaryExpression == null) throw new NotSupportedException("Highlight expression specify only one property");

            var lambdaExpression = unaryExpression.Operand as LambdaExpression;
            if (lambdaExpression == null) throw new NotSupportedException("Highlight expression must be lambda");

            var bodyExpression = lambdaExpression.Body as MemberExpression;
            if (bodyExpression == null) throw new NotSupportedException("Highlight expression must select a member");

            // Highlighting is inserted into the materialization chain
            if (searchRequest.Highlight == null)
            {
                searchRequest.Highlight = (Highlight)((ConstantExpression)configExpression).Value;
                materializer = new HighlightElasticMaterializer(materializer);
            }

            searchRequest.Highlight.AddFields(Mapping.GetFieldName(SourceType, bodyExpression));

            return Visit(source);
        }

        Expression VisitMinimumScore(Expression source, Expression minScoreExpression)
        {
            if (minScoreExpression is ConstantExpression)
            {
                searchRequest.MinScore = Convert.ToDouble(((ConstantExpression)minScoreExpression).Value);
                return Visit(source);
            }

            throw new NotSupportedException($"Score must be a constant expression, not a {minScoreExpression.NodeType}.");
        }

        Expression VisitQueryString(Expression source, Expression queryExpression, Expression fieldsExpression = null)
        {
            var constantQueryExpression = (ConstantExpression)queryExpression;
            var constantFieldExpression = fieldsExpression as ConstantExpression;
            var constantFields = (string[])constantFieldExpression?.Value;
            var criteriaExpression = new CriteriaExpression(new QueryStringCriteria(constantQueryExpression.Value.ToString(), constantFields));
            searchRequest.Query = AndCriteria.Combine(searchRequest.Query, criteriaExpression.Criteria);

            return Visit(source);
        }

        internal Expression VisitQueryableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Select":
                    if (m.Arguments.Count == 2)
                        return VisitSelect(m.Arguments[0], m.Arguments[1]);
                    throw GetOverloadUnsupportedException(m.Method);

                case "First":
                case "FirstOrDefault":
                case "Single":
                case "SingleOrDefault":
                    if (m.Arguments.Count == 1)
                        return VisitFirstOrSingle(m.Arguments[0], null, m.Method.Name);
                    if (m.Arguments.Count == 2)
                        return VisitFirstOrSingle(m.Arguments[0], m.Arguments[1], m.Method.Name);
                    throw GetOverloadUnsupportedException(m.Method);

                case "Where":
                    if (m.Arguments.Count == 2)
                        return VisitWhere(m.Arguments[0], m.Arguments[1]);
                    throw GetOverloadUnsupportedException(m.Method);

                case "Skip":
                    if (m.Arguments.Count == 2)
                        return VisitSkip(m.Arguments[0], m.Arguments[1]);
                    throw GetOverloadUnsupportedException(m.Method);

                case "Take":
                    if (m.Arguments.Count == 2)
                        return VisitTake(m.Arguments[0], m.Arguments[1]);
                    throw GetOverloadUnsupportedException(m.Method);

                case "OrderBy":
                case "OrderByDescending":
                    if (m.Arguments.Count == 2)
                        return VisitOrderBy(m.Arguments[0], m.Arguments[1], m.Method.Name == "OrderBy");
                    throw GetOverloadUnsupportedException(m.Method);

                case "ThenBy":
                case "ThenByDescending":
                    if (m.Arguments.Count == 2)
                        return VisitOrderBy(m.Arguments[0], m.Arguments[1], m.Method.Name == "ThenBy");
                    throw GetOverloadUnsupportedException(m.Method);

                case "Count":
                case "LongCount":
                    if (m.Arguments.Count == 1)
                        return VisitCount(m.Arguments[0], null, m.Method.ReturnType);
                    if (m.Arguments.Count == 2)
                        return VisitCount(m.Arguments[0], m.Arguments[1], m.Method.ReturnType);
                    throw GetOverloadUnsupportedException(m.Method);

                case "Any":
                    if (m.Arguments.Count == 1)
                        return VisitAny(m.Arguments[0], null);
                    if (m.Arguments.Count == 2)
                        return VisitAny(m.Arguments[0], m.Arguments[1]);
                    throw GetOverloadUnsupportedException(m.Method);
            }

            throw new NotSupportedException($"Queryable.{m.Method.Name} method is not supported");
        }

        static NotSupportedException GetOverloadUnsupportedException(MethodInfo methodInfo)
        {
            return new NotSupportedException(
                $"Queryable.{methodInfo.GetSimpleSignature()} method overload is not supported");
        }

        Expression VisitAny(Expression source, Expression predicate)
        {
            materializer = new AnyElasticMaterializer();
            searchRequest.Size = 1;

            return predicate != null
                ? VisitWhere(source, predicate)
                : Visit(source);
        }

        Expression VisitCount(Expression source, Expression predicate, Type returnType)
        {
            materializer = new CountElasticMaterializer(returnType);
            searchRequest.Size = 0;
            return predicate != null
                ? VisitWhere(source, predicate)
                : Visit(source);
        }

        Expression VisitFirstOrSingle(Expression source, Expression predicate, string methodName)
        {
            var single = methodName.StartsWith("Single", StringComparison.Ordinal);
            var orDefault = methodName.EndsWith("OrDefault", StringComparison.Ordinal);

            searchRequest.Size = single ? 2 : 1;
            finalItemType = source.Type;
            materializer = new OneHitElasticMaterializer(itemProjector ?? DefaultItemProjector, finalItemType, single, orDefault);

            return predicate != null
                ? VisitWhere(source, predicate)
                : Visit(source);
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

        Expression VisitWhere(Expression source, Expression lambdaPredicate)
        {
            var lambda = lambdaPredicate.GetLambda();

            var criteriaExpression = lambda.Body as CriteriaExpression ?? BooleanMemberAccessBecomesEquals(lambda.Body) as CriteriaExpression;

            if (criteriaExpression == null)
                throw new NotSupportedException($"Where expression '{lambda.Body}' could not be translated");

            searchRequest.Query = AndCriteria.Combine(searchRequest.Query, criteriaExpression.Criteria);

            return Visit(source);
        }

        Expression VisitOrderBy(Expression source, Expression orderByExpression, bool ascending)
        {
            var lambda = orderByExpression.GetLambda();
            var final = Visit(lambda.Body) as MemberExpression;
            if (final != null)
            {
                var fieldName = Mapping.GetFieldName(SourceType, final);

                var sortFieldType = final.Type.IsGenericOf(typeof(Nullable<>))
                    ? final.Type.GetGenericArguments()[0]
                    : final.Type;

                var sortOption = new SortOption(fieldName, ascending,
                    final.Type.IsNullable() ? Mapping.GetElasticFieldType(sortFieldType) : null);

                searchRequest.SortOptions.Insert(0, sortOption);
            }

            return Visit(source);
        }

        Expression VisitOrderByScore(Expression source, bool ascending)
        {
            searchRequest.SortOptions.Insert(0, new SortOption("_score", ascending));
            return Visit(source);
        }

        Expression VisitSelect(Expression source, Expression selectExpression)
        {
            var lambda = selectExpression.GetLambda();

            if (lambda.Parameters.Count != 1)
                throw new NotSupportedException("Select method with T parameter is supported, additional parameters like index are not");

            var selectBody = lambda.Body;

            if (selectBody is MemberExpression)
                RebindPropertiesAndElasticFields(selectBody);

            if (selectBody is NewExpression)
                RebindSelectBody(selectBody, ((NewExpression)selectBody).Arguments, lambda.Parameters);

            if (selectBody is MethodCallExpression)
                RebindSelectBody(selectBody, ((MethodCallExpression)selectBody).Arguments, lambda.Parameters);

            if (selectBody is MemberInitExpression)
                RebindPropertiesAndElasticFields(selectBody);

            finalItemType = selectBody.Type;

            return Visit(source);
        }

        void RebindSelectBody(Expression selectExpression, IEnumerable<Expression> arguments, IEnumerable<ParameterExpression> parameters)
        {
            var entityParameter = arguments.SingleOrDefault(parameters.Contains) as ParameterExpression;
            if (entityParameter == null)
                RebindPropertiesAndElasticFields(selectExpression);
            else
                RebindElasticFieldsAndChainProjector(selectExpression, entityParameter);
        }

        /// <summary>
        /// We are using the whole entity in a new select projection. Re-bind any ElasticField references to JObject
        /// and ensure the entity parameter is a freshly materialized entity object from our default materializer.
        /// </summary>
        /// <param name="selectExpression">Select expression to re-bind.</param>
        /// <param name="entityParameter">Parameter that references the whole entity.</param>
        void RebindElasticFieldsAndChainProjector(Expression selectExpression, ParameterExpression entityParameter)
        {
            var projection = ElasticFieldsExpressionVisitor.Rebind(SourceType, Mapping, selectExpression);
            var compiled = Expression.Lambda(projection.Item1, entityParameter, projection.Item2).Compile();
            itemProjector = h => compiled.DynamicInvoke(DefaultItemProjector(h), h);
        }

        /// <summary>
        /// We are using just some properties of the entity. Rewrite the properties as JObject field lookups and
        /// record all the field names used to ensure we only select those.
        /// </summary>
        /// <param name="selectExpression">Select expression to re-bind.</param>
        void RebindPropertiesAndElasticFields(Expression selectExpression)
        {
            var projection = MemberProjectionExpressionVisitor.Rebind(SourceType, Mapping, selectExpression);
            var compiled = Expression.Lambda(projection.Expression, projection.Parameter).Compile();
            itemProjector = h => compiled.DynamicInvoke(h);
            searchRequest.Fields.AddRange(projection.Collected);
        }

        Expression VisitSkip(Expression source, Expression skipExpression)
        {
            var skipConstant = Visit(skipExpression) as ConstantExpression;
            if (skipConstant != null)
                searchRequest.From = (int)skipConstant.Value;
            return Visit(source);
        }

        Expression VisitTake(Expression source, Expression takeExpression)
        {
            var takeConstant = Visit(takeExpression) as ConstantExpression;
            if (takeConstant != null)
            {
                var takeValue = (int)takeConstant.Value;

                searchRequest.Size = searchRequest.Size.HasValue
                    ? Math.Min(searchRequest.Size.GetValueOrDefault(), takeValue)
                    : takeValue;
            }
            return Visit(source);
        }

        Func<Hit, object> DefaultItemProjector
        {
            get { return hit => Mapping.Materialize(hit._source, SourceType); }
        }
    }
}