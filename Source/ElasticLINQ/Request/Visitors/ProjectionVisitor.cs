// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Mapping;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    internal class Projection
    {
        internal readonly HashSet<string> Fields = new HashSet<string>();
        internal Expression Selector;
    }

    /// <summary>
    /// Visitor that rewrites projections to bind to JObject.
    /// </summary>
    internal class ProjectionVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo getValueMethod = typeof(JObject).GetMethod("GetValue", new[] { typeof(string) });

        private readonly ParameterExpression parameter;
        private readonly IElasticMapping mapping;
        private readonly Projection projection;

        public ProjectionVisitor(ParameterExpression parameter, IElasticMapping mapping)
        {
            this.parameter = parameter;
            this.mapping = mapping;
            projection = new Projection();
        }

        internal Projection ProjectColumns(Expression selector)
        {
            projection.Selector = Visit(selector);
            return projection;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
                return base.VisitMember(m);

            var fieldName = mapping.GetFieldName(m.Member);
            projection.Fields.Add(fieldName);

            var methodCall = Expression.Call(parameter, getValueMethod, Expression.Constant(fieldName));
            return Expression.Convert(methodCall, m.Type);
        }
    }
}