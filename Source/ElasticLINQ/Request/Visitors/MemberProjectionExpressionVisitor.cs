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
    internal class MemberProjectionExpressionVisitor : ElasticFieldsExpressionVisitor
    {
        protected static readonly MethodInfo GetDictionaryValueMethod = typeof(MemberProjectionExpressionVisitor)
            .GetMethod("GetDictionaryValueOrDefault", BindingFlags.Static | BindingFlags.NonPublic);

        private readonly HashSet<string> fieldNames = new HashSet<string>();

        private MemberProjectionExpressionVisitor(ParameterExpression bindingParameter, IElasticMapping mapping)
            : base(bindingParameter, mapping)
        {
        }

        internal static new RebindCollectionResult<string> Rebind(IElasticMapping mapping, Expression selector)
        {
            var parameter = Expression.Parameter(typeof(Hit), "h");
            var visitor = new MemberProjectionExpressionVisitor(parameter, mapping);
            Argument.EnsureNotNull("selector", selector);
            var materializer = visitor.Visit(selector);
            return new RebindCollectionResult<string>(materializer, visitor.fieldNames, parameter);
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
            var getFieldExpression = Expression.Call(null, GetDictionaryValueMethod, Expression.PropertyOrField(BindingParameter, "fields"), Expression.Constant(fieldName), Expression.Constant(m.Type));
            return Expression.Convert(getFieldExpression, m.Type);
        }

        internal static object GetDictionaryValueOrDefault(IDictionary<string, JToken> dictionary, string key, Type expectedType)
        {
            JToken token;
            if (dictionary.TryGetValue(key, out token))
                return token.ToObject(expectedType);

            return expectedType.IsValueType
                ? Activator.CreateInstance(expectedType)
                : null;
        }
    }
}