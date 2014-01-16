// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Facets;
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
    /// Rebinds aggregate method accesses to JObject facet fields.
    /// </summary>
    internal class AggregateExpressionVisitor : RebindingExpressionVisitor
    {
        private static readonly MethodInfo getFacetFromResponseMethod = typeof(AggregateExpressionVisitor)
            .GetMethod("GetFacetFromResponse", BindingFlags.Static | BindingFlags.NonPublic);

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

        internal static Expression Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression expression)
        {
            var visitor = new AggregateExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("expression", expression);
            return visitor.Visit(expression);
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

            if (m.Method.DeclaringType == typeof(Enumerable))
            {
                string slice;
                if (methodToFacetSlice.TryGetValue(m.Method.Name, out slice))
                {
                    if (m.Arguments.Count == 1)
                        return VisitAggregateTerm((ParameterExpression)m.Arguments[0], slice, m.Method.ReturnType);
                }
            }

            return base.VisitMethodCall(m);
        }

        private Expression VisitAggregateTerm(ParameterExpression parameter, string slice, Type returnType)
        {
            // Create the term facet
            var groupByMemberInfo = groupByMembers[parameter.Name];
            var keyField = Mapping.GetFieldName(groupByMemberInfo);
            facets[keyField] = new TermsFacet(keyField, keyField);

            // Rebind the property to the correct ElasticResponse node
            var getFacetExpression = Expression.Call(null, getFacetFromResponseMethod, BindingParameter,
                Expression.Constant(keyField), Expression.Constant(slice), Expression.Constant(returnType));

            return Expression.Convert(getFacetExpression, returnType);
        }

        internal static object GetFacetFromResponse(ElasticResponse r, string key, string slice, Type expectedType)
        {
            return 1;
            // TODO: Handle missing values
            return r.facets[key][slice].ToObject(expectedType);
        }
    }
}