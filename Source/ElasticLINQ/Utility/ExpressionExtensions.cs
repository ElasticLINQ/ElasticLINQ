// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq.Expressions;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Extension methods for Expression objects.
    /// </summary>
    internal static class ExpressionExtensions
    {
        public static Expression StripQuotes(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }

        public static LambdaExpression GetLambda(this Expression expression)
        {
            return (LambdaExpression)expression.StripQuotes();
        }
    }
}