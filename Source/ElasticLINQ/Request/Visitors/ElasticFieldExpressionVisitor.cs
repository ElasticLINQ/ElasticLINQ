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
    internal class ElasticFieldExpressionVisitor : ExpressionVisitor
    {
        protected readonly ParameterExpression Parameter;
        protected readonly IElasticMapping Mapping;

        protected ElasticFieldExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("parameter", parameter);
            Argument.EnsureNotNull("mapping", mapping);

            Parameter = parameter;
            Mapping = mapping;
        }

        internal static Expression Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var visitor = new ElasticFieldExpressionVisitor(parameter, mapping);
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