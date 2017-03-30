// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Expression visitor that substitutes references to <see cref="ElasticFields"/>
    /// with the desired underlying special reserved name.
    /// </summary>
    class ElasticFieldsExpressionVisitor : ExpressionVisitor
    {
        protected readonly ParameterExpression BindingParameter;
        protected readonly IElasticMapping Mapping;
        protected readonly Type SourceType;

        public ElasticFieldsExpressionVisitor(Type sourcetype, ParameterExpression bindingParameter, IElasticMapping mapping)
        {
            Argument.EnsureNotNull(nameof(bindingParameter), bindingParameter);
            Argument.EnsureNotNull(nameof(mapping), mapping);

            SourceType = sourcetype;
            BindingParameter = bindingParameter;
            Mapping = mapping;
        }

        internal static Tuple<Expression, ParameterExpression> Rebind(Type sourceType, IElasticMapping mapping, Expression selector)
        {
            var parameter = Expression.Parameter(typeof(Hit), "h");
            var visitor = new ElasticFieldsExpressionVisitor(sourceType, parameter, mapping);
            Argument.EnsureNotNull(nameof(selector), selector);
            return Tuple.Create(visitor.Visit(selector), parameter);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return IsElasticField(node) ? VisitElasticField(node) : base.VisitMember(node);
        }

        protected bool IsElasticField (MemberExpression node)
        {
            return node.Member.DeclaringType == typeof(ElasticFields);
        }

        protected virtual Expression VisitElasticField(MemberExpression m)
        {
            return Expression.Convert(Expression.PropertyOrField(BindingParameter, "_" + m.Member.Name.ToLowerInvariant()), m.Type);
        }
    }
}