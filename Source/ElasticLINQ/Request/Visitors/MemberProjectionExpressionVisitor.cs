// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Rebinds select projection entity proprties to JObject and captures the
    /// specific field names for selection.
    /// </summary>
    internal class MemberProjectionExpressionVisitor : ElasticFieldExpressionVisitor
    {
        private static readonly MethodInfo getFieldMethod = typeof(MemberProjectionExpressionVisitor).GetMethod("GetFieldValue", BindingFlags.Static | BindingFlags.NonPublic);
        private readonly HashSet<string> fieldNames = new HashSet<string>();

        private MemberProjectionExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
            : base(parameter, mapping)
        {
        }

        internal static new Projection Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var visitor = new MemberProjectionExpressionVisitor(parameter, mapping);
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
            var getFieldExpression = Expression.Call(null, getFieldMethod, Expression.PropertyOrField(Parameter, "fields"), Expression.Constant(fieldName), Expression.Constant(m.Type));
            return Expression.Convert(getFieldExpression, m.Type);
        }

        internal static object GetFieldValue(IDictionary<string, JToken> dictionary, string key, Type expectedType)
        {
            JToken token;
            if (dictionary.TryGetValue(key, out token))
                return token.ToObject(expectedType);

            return expectedType.IsValueType
                ? Activator.CreateInstance(expectedType)
                : null;
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