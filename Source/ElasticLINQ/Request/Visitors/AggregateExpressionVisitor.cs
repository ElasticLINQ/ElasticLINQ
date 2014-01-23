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
    /// <summary>
    /// Rebinds aggregate method accesses to JObject facet fields.
    /// </summary>
    internal class AggregateExpressionVisitor : ExpressionVisitor
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

        private readonly HashSet<MemberInfo> aggregateMembers = new HashSet<MemberInfo>();
        private readonly ParameterExpression bindingParameter;
        private readonly IElasticMapping mapping;

        private MemberInfo groupByMember;

        private AggregateExpressionVisitor(ParameterExpression bindingParameter, IElasticMapping mapping)
        {
            this.bindingParameter = bindingParameter;
            this.mapping = mapping;
        }

        internal static RebindCollectionResult<IFacet> Rebind(IElasticMapping mapping, Expression expression)
        {
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("expression", expression);

            var parameter = Expression.Parameter(typeof(AggregateRow), "r");
            var visitor = new AggregateExpressionVisitor(parameter, mapping);

            return new RebindCollectionResult<IFacet>(visitor.Visit(expression), new HashSet<IFacet>(visitor.GetFacets()), parameter, visitor.selectProjection);
        }

        private IEnumerable<IFacet> GetFacets()
        {
            if (groupByMember != null)
            {
                var groupByField = mapping.GetFieldName(groupByMember);
                foreach (var member in aggregateMembers)
                {
                    var valueField = mapping.GetFieldName(member);
                    yield return new TermsStatsFacet(valueField, groupByField, valueField);
                }
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.Name == "Key" && m.Member.DeclaringType.IsGenericOf(typeof(IGrouping<,>)))
                return VisitAggregateKey(m.Type);

            return base.VisitMember(m);
        }

        private LambdaExpression selectProjection;

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Enumerable) || m.Method.DeclaringType == typeof(Queryable))
            {
                if (m.Method.Name == "GroupBy" && m.Arguments.Count == 2)
                    groupByMember = GetMemberInfoFromLambda(m.Arguments[1]);

                if (m.Method.Name == "Select" && m.Arguments.Count == 2)
                {
                    var y = (LambdaExpression)StripQuotes(Visit(m.Arguments[1]));
                    selectProjection = Expression.Lambda(y.Body, bindingParameter);

                    return Visit(m.Arguments[0]);
                }

                string slice;
                if (methodToFacetSlice.TryGetValue(m.Method.Name, out slice) && m.Arguments.Count == 2)
                    return VisitAggregateTerm(m.Arguments[1], slice, m.Method.ReturnType);
            }

            return base.VisitMethodCall(m);
        }

        private static MemberInfo GetMemberInfoFromLambda(Expression expression)
        {
            var lambda = StripQuotes(expression) as LambdaExpression;
            if (lambda == null)
                throw new NotSupportedException(String.Format("Require a lambda with member access not {0}", expression));

            var memberExpressionBody = lambda.Body as MemberExpression;
            if (memberExpressionBody == null)
                throw new NotSupportedException("GroupBy must be specified against a member of the entity");

            return memberExpressionBody.Member;
        }

        private Expression VisitAggregateKey(Type returnType)
        {
            var getFacetExpression = Expression.Call(null, getKeyFromRow, bindingParameter);
            return Expression.Convert(getFacetExpression, returnType);
        }

        private Expression VisitAggregateTerm(Expression property, string operation, Type returnType)
        {
            var member = GetMemberInfoFromLambda(property);
            var valueField = mapping.GetFieldName(member);
            aggregateMembers.Add(member);

            // Rebind the property to the correct ElasticResponse node
            var getFacetExpression = Expression.Call(null, getValueFromRow, bindingParameter,
                Expression.Constant(valueField), Expression.Constant(operation), Expression.Constant(returnType));

            return Expression.Convert(getFacetExpression, returnType);
        }

        protected static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }
    }
}