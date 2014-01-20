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
    internal class RebindingResult<T>
    {
        private readonly Expression expression;
        private readonly HashSet<T> collected;
        private readonly ParameterExpression parameter;

        public RebindingResult(Expression expression, HashSet<T> collected, ParameterExpression parameter)
        {
            this.expression = expression;
            this.collected = collected;
            this.parameter = parameter;
        }

        public Expression Expression { get { return expression; } }
        public ParameterExpression Parameter { get { return parameter; } }
        public IReadOnlyList<T> Collected { get { return collected.ToList().AsReadOnly(); } }
    }

    internal abstract class RebindingExpressionVisitor : ExpressionVisitor
    {
        protected readonly ParameterExpression BindingParameter;
        protected readonly IElasticMapping Mapping;

        protected RebindingExpressionVisitor(ParameterExpression bindingParameter, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("bindingParameter", bindingParameter);
            Argument.EnsureNotNull("mapping", mapping);

            BindingParameter = bindingParameter;
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
