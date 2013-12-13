// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Request.Visitors
{
    internal abstract class RebindingExpressionVisitor : ExpressionVisitor
    {
        protected readonly ParameterExpression Parameter;
        protected readonly IElasticMapping Mapping;

        protected static readonly MethodInfo GetDictionaryValueMethod = typeof(RebindingExpressionVisitor)
            .GetMethod("GetDictionaryValueOrDefault", BindingFlags.Static | BindingFlags.NonPublic);

        protected RebindingExpressionVisitor(ParameterExpression parameter, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("parameter", parameter);
            Argument.EnsureNotNull("mapping", mapping);

            Parameter = parameter;
            Mapping = mapping;
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

        protected static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
                e = ((UnaryExpression)e).Operand;
            return e;
        }
    }
}
