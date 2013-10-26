// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    internal class ElasticFieldsProjectionExpressionVisitor : ExpressionVisitor
    {
        protected readonly ParameterExpression Parameter;
        protected readonly IElasticMapping Mapping;

        protected ElasticFieldsProjectionExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("parameter", parameter);
            Argument.EnsureNotNull("mapping", mapping);

            Parameter = parameter;
            Mapping = mapping;
        }

        internal static Expression Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var visitor = new ElasticFieldsProjectionExpressionVisitor(parameter, mapping);
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