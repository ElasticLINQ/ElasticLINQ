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
    /// Converts aggregate expressions into ElasticSearch facets.
    /// </summary>
    internal class AggregateExpressionVisitor : ExpressionVisitor
    {
        private const string GroupKeyTermsName = "GroupKey";

        private static readonly MethodInfo getValueFromRow = typeof(AggregateRow).GetMethod("GetValue", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo getKeyFromRow = typeof(AggregateRow).GetMethod("GetKey", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly Dictionary<string, string> aggregateMemberOperations = new Dictionary<string, string>
        {
            { "Min", "min" },
            { "Max", "max" },
            { "Sum", "total" },
            { "Average", "mean" },
        };

        private static readonly Dictionary<string, string> aggregateOperations = new Dictionary<string, string>
        {
            { "Count", "count" },
            { "LongCount", "count" }
        };

        private readonly HashSet<MemberInfo> aggregateMembers = new HashSet<MemberInfo>();
        private readonly ParameterExpression bindingParameter = Expression.Parameter(typeof(AggregateRow), "r");
        private readonly IElasticMapping mapping;

        private MemberInfo groupByMember;
        private LambdaExpression selectProjection;
        private bool includeGroupKeyTerms;

        private AggregateExpressionVisitor(IElasticMapping mapping)
        {
            this.mapping = mapping;
        }

        internal static RebindCollectionResult<IFacet> Rebind(IElasticMapping mapping, Expression expression)
        {
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("expression", expression);

            var visitor = new AggregateExpressionVisitor(mapping);

            return new RebindCollectionResult<IFacet>(visitor.Visit(expression), new HashSet<IFacet>(visitor.GetFacets()), visitor.bindingParameter, visitor.selectProjection);
        }

        private IEnumerable<IFacet> GetFacets()
        {
            if (groupByMember == null)
            {
                foreach (var valueField in aggregateMembers.Select(member => mapping.GetFieldName(member)))
                    yield return new StatisticalFacet(valueField, valueField);                   
            }
            else
            {
                var groupByField = mapping.GetFieldName(groupByMember);
                foreach (var valueField in aggregateMembers.Select(member => mapping.GetFieldName(member)))
                    yield return new TermsStatsFacet(valueField, groupByField, valueField);

                if (includeGroupKeyTerms)
                    yield return new TermsFacet(GroupKeyTermsName, groupByField);
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.Name == "Key" && m.Member.DeclaringType.IsGenericOf(typeof(IGrouping<,>)))
                return VisitGroupKey(m.Type);

            return base.VisitMember(m);
        }

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

                string operation;
                if (aggregateOperations.TryGetValue(m.Method.Name, out operation) && m.Arguments.Count == 1)
                    return VisitAggregateOperation(operation, m.Method.ReturnType);

                if (aggregateMemberOperations.TryGetValue(m.Method.Name, out operation) && m.Arguments.Count == 2)
                    return VisitAggregateMemberOperations(m.Arguments[1], operation, m.Method.ReturnType);
            }

            return base.VisitMethodCall(m);
        }

        private Expression VisitGroupKey(Type returnType)
        {
            var getKeyExpression = Expression.Call(null, getKeyFromRow, bindingParameter);
            return Expression.Convert(getKeyExpression, returnType);
        }

        private Expression VisitAggregateOperation(string operation, Type returnType)
        {
            includeGroupKeyTerms = true;

            var getValueExpression = Expression.Call(null, getValueFromRow, bindingParameter,
                Expression.Constant(GroupKeyTermsName), Expression.Constant(operation), Expression.Constant(returnType));

            return Expression.Convert(getValueExpression, returnType);
        }

        private Expression VisitAggregateMemberOperations(Expression property, string operation, Type returnType)
        {
            var member = GetMemberInfoFromLambda(property);
            var valueField = mapping.GetFieldName(member);
            aggregateMembers.Add(member);

            var getValueExpression = Expression.Call(null, getValueFromRow, bindingParameter,
                Expression.Constant(valueField), Expression.Constant(operation), Expression.Constant(returnType));

            return Expression.Convert(getValueExpression, returnType);
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

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }
    }
}