// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    internal class Projection
    {
        internal readonly HashSet<string> FieldNames = new HashSet<string>();
        internal Expression MaterializationExpression;
    }

    /// <summary>
    /// Rewrites select projections to bind to JObject and captures the field names
    /// in order to only select from ElasticSearch those required.
    /// </summary>
    internal class ProjectionVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo getFieldMethod = typeof(JObject).GetMethod("GetValue", new[] { typeof(string) });

        private readonly ParameterExpression parameter;
        private readonly IElasticMapping mapping;
        private readonly Projection projection = new Projection();

        private ProjectionVisitor(ParameterExpression parameter, IElasticMapping mapping)
        {
            this.parameter = parameter;
            this.mapping = mapping;
        }

        internal static Projection ProjectColumns(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var projectionVisitor = new ProjectionVisitor(parameter, mapping);
            projectionVisitor.projection.MaterializationExpression = projectionVisitor.Visit(selector);
            return projectionVisitor.projection;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(ElasticFields))
                return VisitFieldSelection(m, true);

            if (m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
                return base.VisitMember(m);

            return VisitFieldSelection(m, false);
        }

        private Expression VisitFieldSelection(MemberExpression m, bool isOnHit)
        {
            var fieldName = mapping.GetFieldName(m.Member);
            projection.FieldNames.Add(fieldName);

            if (isOnHit)
                return Expression.Convert(Expression.PropertyOrField(parameter, fieldName), m.Type);

            return Expression.Convert(
                Expression.Call(Expression.PropertyOrField(parameter, "fields"), getFieldMethod, Expression.Constant(fieldName)), m.Type);
        }
    }
}