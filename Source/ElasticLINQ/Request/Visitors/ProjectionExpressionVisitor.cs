// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Linq;
using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    internal class Projection
    {
        private readonly HashSet<string> fieldNames;
        private readonly Expression materialization;

        public Projection(HashSet<string> fieldNames, Expression materialization)
        {
            this.fieldNames = fieldNames;
            this.materialization = materialization;
        }

        public IEnumerable<string> FieldNames { get { return fieldNames.AsEnumerable(); }} 
        
        public Expression Materialization {get { return materialization; }}
    }

    /// <summary>
    /// Rewrites select projections to bind to JObject and captures the field names
    /// in order to only select from ElasticSearch those required.
    /// </summary>
    internal class ProjectionExpressionVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo getFieldMethod = typeof(JObject).GetMethod("GetValue", new[] { typeof(string) });

        private readonly HashSet<string> fieldNames = new HashSet<string>();
        private readonly ParameterExpression parameter;
        private readonly IElasticMapping mapping;

        private ProjectionExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
        {
            this.parameter = parameter;
            this.mapping = mapping;
        }

        internal static Projection ProjectColumns(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            Argument.EnsureNotNull("parameter", parameter);
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("selector", selector);

            var visitor = new ProjectionExpressionVisitor(parameter, mapping);

            var materialization = visitor.Visit(selector);
            return new Projection(visitor.fieldNames, materialization);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(ElasticFields))
                return VisitFieldSelection(m, false);

            if (m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter)
                return base.VisitMember(m);

            return VisitFieldSelection(m, true);
        }

        private Expression VisitFieldSelection(MemberExpression m, bool inFieldsProperty)
        {
            var fieldName = mapping.GetFieldName(m.Member);
            fieldNames.Add(fieldName);

            return Expression.Convert(GetFieldExpression(inFieldsProperty, fieldName), m.Type);
        }

        private Expression GetFieldExpression(bool inFieldsProperty, string fieldName)
        {
            return !inFieldsProperty
                ? (Expression) Expression.PropertyOrField(parameter, fieldName)
                : Expression.Call(Expression.PropertyOrField(parameter, "fields"), getFieldMethod, Expression.Constant(fieldName));
        }
    }
}