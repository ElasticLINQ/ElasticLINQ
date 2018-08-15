// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Rebinds select projection entity member accesses to JObject fields
    /// recording the specific field names required for selection.
    /// </summary>
    class MemberProjectionExpressionVisitor : ElasticFieldsExpressionVisitor
    {
        protected static readonly MethodInfo GetKeyedValueMethod = typeof(MemberProjectionExpressionVisitor).GetMethodInfo(m => m.Name == "GetKeyedValueOrDefault");

        readonly HashSet<string> fieldNames = new HashSet<string>();

        MemberProjectionExpressionVisitor(Type sourceType, ParameterExpression bindingParameter, IElasticMapping mapping)
            : base(sourceType, bindingParameter, mapping)
        {
        }

        internal new static RebindCollectionResult<string> Rebind(Type sourceType, IElasticMapping mapping, Expression selector)
        {
            var parameter = Expression.Parameter(typeof(Hit), "h");
            var visitor = new MemberProjectionExpressionVisitor(sourceType, parameter, mapping);
            Argument.EnsureNotNull(nameof(selector), selector);
            var materializer = visitor.Visit(selector);
            return new RebindCollectionResult<string>(materializer, visitor.fieldNames, parameter);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
            {
                switch (node.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        return VisitFieldSelection(node);
                    case ExpressionType.MemberAccess:
                        if (!IsElasticField(node)) return VisitFieldSelection(node);
                        break;
                }
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitElasticField(MemberExpression m)
        {
            var member = base.VisitElasticField(m);
            fieldNames.Add("_" +  m.Member.Name.ToLowerInvariant());
            return member;
        }

        Expression VisitFieldSelection(MemberExpression m)
        {
            var fieldName = Mapping.GetFieldName(SourceType, m);
            fieldNames.Add(fieldName);
            var getFieldExpression = Expression.Call(null, GetKeyedValueMethod, Expression.PropertyOrField(BindingParameter, "_source"), Expression.Constant(fieldName), Expression.Constant(m.Type));
            return Expression.Convert(getFieldExpression, m.Type);
        }

        internal static object GetKeyedValueOrDefault(JObject hit, string key, Type expectedType)
        {
            if (hit != null)
            {
                var token = hit[key];
                if (token == null)
                    return TypeHelper.CreateDefault(expectedType);

                return token.ToObject(expectedType);
            }

            return Activator.CreateInstance(expectedType);
        }
    }
}