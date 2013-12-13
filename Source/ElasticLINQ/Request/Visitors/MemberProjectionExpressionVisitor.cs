// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Rebinds select projection entity member accesses to JObject fields
    /// recording the specific field names required for selection.
    /// </summary>
    internal class MemberProjectionExpressionVisitor : ElasticFieldsRebindingExpressionVisitor
    {
        private readonly HashSet<string> fieldNames = new HashSet<string>();

        private MemberProjectionExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
            : base(parameter, mapping)
        {
        }

        internal static new MemberProjectionResult Rebind(ParameterExpression parameter, IElasticMapping mapping, Expression selector)
        {
            var visitor = new MemberProjectionExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("selector", selector);
            var materializer = visitor.Visit(selector);
            return new MemberProjectionResult(visitor.fieldNames, materializer);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                return VisitFieldSelection(m);

            return base.VisitMember(m);
        }

        protected override Expression VisitElasticField(MemberExpression m)
        {
            fieldNames.Add(Mapping.GetFieldName(m.Member));
            return base.VisitElasticField(m);
        }

        private Expression VisitFieldSelection(MemberExpression m)
        {
            var fieldName = Mapping.GetFieldName(m.Member);
            fieldNames.Add(fieldName);
            var getFieldExpression = Expression.Call(null, GetDictionaryValueMethod, Expression.PropertyOrField(Parameter, "fields"), Expression.Constant(fieldName), Expression.Constant(m.Type));
            return Expression.Convert(getFieldExpression, m.Type);
        }
    }

    internal class MemberProjectionResult
    {
        private readonly HashSet<string> fieldNames;
        private readonly Expression materializer;

        public MemberProjectionResult(HashSet<string> fieldNames, Expression materializer)
        {
            this.fieldNames = fieldNames;
            this.materializer = materializer;
        }

        public IEnumerable<string> FieldNames { get { return fieldNames.AsEnumerable(); } }
        
        public Expression Materializer { get { return materializer; } } 
    }
}