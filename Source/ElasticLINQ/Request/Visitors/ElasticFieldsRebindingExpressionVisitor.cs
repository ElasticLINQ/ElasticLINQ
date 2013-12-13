// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Expression visitor that substitutes references to <see cref="ElasticFields"/>
    /// with the desired underlying special reserved name.
    /// </summary>
    internal class ElasticFieldsRebindingExpressionVisitor : RebindingExpressionVisitor
    {
        public ElasticFieldsRebindingExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
            : base(parameter, mapping)
        {
        }

        internal static Expression Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var visitor = new ElasticFieldsRebindingExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("selector", selector);
            return visitor.Visit(selector);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            return m.Member.DeclaringType == typeof(ElasticFields)
                ? VisitElasticField(m)
                : base.VisitMember(m);
        }

        protected virtual Expression VisitElasticField(MemberExpression m)
        {
            return Expression.Convert(Expression.PropertyOrField(Parameter, Mapping.GetFieldName(m.Member)), m.Type);
        }
    }
}