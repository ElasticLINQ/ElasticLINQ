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
        static readonly MethodInfo getDictionaryValueMethod = typeof(MemberProjectionExpressionVisitor).GetMethodInfo(m => m.Name == "GetDictionaryValueOrDefault");

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
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
                return VisitFieldSelection(node);

            return base.VisitMember(node);
        }

        protected override Expression VisitElasticField(MemberExpression m)
        {
            var member = base.VisitElasticField(m);
            fieldNames.Add("_" + m.Member.Name.ToLowerInvariant());
            return member;
        }

        Expression VisitFieldSelection(MemberExpression m)
        {
            var fieldName = Mapping.GetFieldName(SourceType, m);
            fieldNames.Add(fieldName);
            var getFieldExpression = Expression.Call(null, getDictionaryValueMethod, Expression.PropertyOrField(BindingParameter, "fields"), Expression.Constant(fieldName), Expression.Constant(m.Type));
            return Expression.Convert(getFieldExpression, m.Type);
        }

        internal static object GetDictionaryValueOrDefault(IDictionary<string, JToken> dictionary, string key, Type expectedType)
        {
            JToken token;
            if (!dictionary.TryGetValue(key, out token))
                return TypeHelper.CreateDefault(expectedType);

            // Elasticsearch 1.0+ puts fields in an array, unwrap it if necessary
            var jArray = token as JArray;
            if (jArray != null && jArray.Count == 1 && !expectedType.IsArray)
                token = jArray[0];

            return token.ToObject(expectedType);
        }
    }
}