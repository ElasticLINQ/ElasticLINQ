// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq.Expressions;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Extension methods for Expression objects.
    /// </summary>
    static class ExpressionExtensions
    {
        /// <summary>
        /// Remove all the outer quotes from an expression.
        /// </summary>
        /// <param name="expression">Expression that might contain outer quotes.</param>
        /// <returns>Expression that no longer contains outer quotes.</returns>
        public static Expression StripQuotes(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }

        /// <summary>
        /// Get the lambda for an expression stripping any necessary outer quotes.
        /// </summary>
        /// <param name="expression">Expression that should be a lamba possibly wrapped
        /// in outer quotes.</param>
        /// <returns>LambdaExpression no longer wrapped in quotes.</returns>
        public static LambdaExpression GetLambda(this Expression expression)
        {
            return (LambdaExpression)expression.StripQuotes();
        }
    }
}