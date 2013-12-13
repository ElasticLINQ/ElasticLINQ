// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Facets;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Rebinds aggregate method accesses to JObject facet fields.
    /// </summary>
    internal class AggregateExpressionVisitor : RebindingExpressionVisitor
    {
        private readonly List<IFacet> facets = new List<IFacet>();

        public AggregateExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
            : base(parameter, mapping)
        {
        }

        internal static Expression Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression expression)
        {
            var visitor = new AggregateExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("expression", expression);
            return visitor.Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Min":
                    case "Max":
                    case "Sum":
                    case "Average":
                    case "Count":
                    case "LongCount":
                        if (m.Arguments.Count == 2)
                            return VisitAggregateMethodCall(m.Arguments[0], m.Arguments[1], m.Method.Name);
                        break;
                }
            }

            if (m.Method.DeclaringType == typeof(Queryable))
            {
                switch (m.Method.Name)
                {
                    case "Sum":
                        if (m.Arguments.Count == 2)
                            return VisitAggregateMethodCall(m.Arguments[0], m.Arguments[1], m.Method.Name);
                        break;
                }
            }

            return base.VisitMethodCall(m);
        }

        private Expression VisitAggregateMethodCall(Expression source, Expression match, string operation)
        {
            var lambda = (LambdaExpression)StripQuotes(match);
            var property = Visit(lambda.Body);

            if (property is MemberExpression)
            {
                var field = Mapping.GetFieldName(((MemberExpression)property).Member);

                // TODO: If source is a group, we need to term_stats not statistical
                facets.Add(new StatisticalFacet("stats", field));

                var getFieldExpression = Expression.Call(null, GetDictionaryValueMethod, Expression.PropertyOrField(Parameter, "facets"), Expression.Constant("stats"), Expression.Constant(match.Type));

                return Expression.Convert(getFieldExpression, match.Type);
                //searchRequest.SearchType = "count"; // We don't need documents
            }

            throw new NotImplementedException("Unknown aggregate property");
        }
    }
}