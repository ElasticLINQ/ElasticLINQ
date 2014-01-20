// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
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
    internal class AggregateRebindResult
    {
        private readonly Expression expression;
        private readonly List<IFacet> facets;

        public AggregateRebindResult(Expression expression, List<IFacet> facets)
        {
            this.expression = expression;
            this.facets = facets;
        }

        public Expression Expression { get { return expression; } }

        public IReadOnlyList<IFacet> Facets { get { return facets.AsReadOnly(); } }
    }

    /// <summary>
    /// Rebinds aggregate method accesses to JObject facet fields.
    /// </summary>
    internal class AggregateExpressionVisitor : RebindingExpressionVisitor
    {
        private static readonly MethodInfo getValueFromRow = typeof(AggregateRow).GetMethod("GetValue", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo getKeyFromRow = typeof(AggregateRow).GetMethod("GetKey", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly Dictionary<string, string> methodToFacetSlice = new Dictionary<string, string>
        {
            { "Min", "min" },
            { "Max", "max" },
            { "Sum", "total" },
            { "Average", "mean" },
            { "Count", "count" },
            { "LongCount", "count" }
        };

        private readonly Dictionary<string, IFacet> facets = new Dictionary<string, IFacet>();
        private readonly Dictionary<string, MemberInfo> groupByMembers = new Dictionary<string, MemberInfo>();

        public AggregateExpressionVisitor(ParameterExpression bindingParameter, IElasticMapping mapping)
            : base(bindingParameter, mapping)
        {
        }

        internal static AggregateRebindResult Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression expression)
        {
            var visitor = new AggregateExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("expression", expression);
            return new AggregateRebindResult(visitor.Visit(expression), visitor.facets.Values.ToList());
        }

        private void StoreGroupByMemberInfo(MethodCallExpression m)
        {
            var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
            var parameter = lambda.Parameters[0];

            if (!groupByMembers.ContainsKey(parameter.Name))
                groupByMembers[parameter.Name] = ((MemberExpression)lambda.Body).Member;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            // Create the GroupBy before we process the args so we have something to reference
            var sourceMethodCall = m.Arguments[0] as MethodCallExpression;
            if (sourceMethodCall != null && sourceMethodCall.Method.Name == "GroupBy")
                StoreGroupByMemberInfo(sourceMethodCall);

            if (m.Method.DeclaringType == typeof(Enumerable) || m.Method.DeclaringType == typeof(Queryable))
            {
                if (m.Method.Name == "Key")
                    return VisitAggregateKey(m.Method.ReturnType);

                string slice;
                if (methodToFacetSlice.TryGetValue(m.Method.Name, out slice))
                {
                    if (m.Arguments.Count == 2)
                        return VisitAggregateTerm((ParameterExpression)m.Arguments[0], (LambdaExpression) m.Arguments[1], slice, m.Method.ReturnType);
                }
            }

            return base.VisitMethodCall(m);
        }

        private Expression VisitAggregateKey(Type returnType)
        {
            var getFacetExpression = Expression.Call(null, getKeyFromRow, BindingParameter);
            return Expression.Convert(getFacetExpression, returnType);
        }

        private Expression VisitAggregateTerm(ParameterExpression parameter, LambdaExpression property, string operation, Type returnType)
        {
            var termsStatsFacet = CreateTermsStatsFacet(parameter, property);
            facets[termsStatsFacet.Name] = termsStatsFacet;

            // Rebind the property to the correct ElasticResponse node
            var getFacetExpression = Expression.Call(null, getValueFromRow, BindingParameter, Expression.Constant(termsStatsFacet.Name), Expression.Constant(operation));

            return Expression.Convert(getFacetExpression, returnType);
        }

        private TermsStatsFacet CreateTermsStatsFacet(ParameterExpression parameter, LambdaExpression property)
        {
            var keyField = Mapping.GetFieldName(groupByMembers[parameter.Name]);
            var valueExpression = (MemberExpression)Visit(property.Body);
            var valueField = Mapping.GetFieldName(valueExpression.Member);
            return new TermsStatsFacet(valueField, keyField, valueField);
        }
    }
}