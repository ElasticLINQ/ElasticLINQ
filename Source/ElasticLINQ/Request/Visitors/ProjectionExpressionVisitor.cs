// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Rewrites select projections to bind to JObject and captures the field names
    /// in order to only select those required.
    /// </summary>
    internal class ProjectionExpressionVisitor : ElasticFieldsProjectionExpressionVisitor
    {
        private static readonly MethodInfo getFieldMethod = typeof(JObject).GetMethod("GetValue", new[] { typeof(string) });
        private readonly HashSet<string> fieldNames = new HashSet<string>();

        private ProjectionExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
            : base(parameter, mapping)
        {
        }

        internal static new Projection Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var visitor = new ProjectionExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("selector", selector);
            var materialization = visitor.Visit(selector);
            return new Projection(visitor.fieldNames, materialization);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                return VisitFieldSelection(m);

            return base.VisitMember(m);            
        }

        private Expression VisitFieldSelection(MemberExpression m)
        {
            var fieldName = Mapping.GetFieldName(m.Member);
            fieldNames.Add(fieldName);
            return Expression.Convert(Expression.Call(Expression.PropertyOrField(Parameter, "fields"), getFieldMethod, Expression.Constant(fieldName)), m.Type);
        }

        protected override Expression VisitElasticField(MemberExpression m)
        {
            fieldNames.Add(Mapping.GetFieldName(m.Member));
            return base.VisitElasticField(m);
        }
    }

    internal class Projection
    {
        private readonly HashSet<string> fieldNames;
        private readonly Expression materialization;

        public Projection(HashSet<string> fieldNames, Expression materialization)
        {
            this.fieldNames = fieldNames;
            this.materialization = materialization;
        }

        public IEnumerable<string> FieldNames { get { return fieldNames.AsEnumerable(); } }

        public Expression Materialization { get { return materialization; } }
    }
}