// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticLinq.Test.TestSupport
{
    /// <summary>
    /// Flattens an expression tree into a list of expressions for
    /// debugging and testing.
    /// </summary>
    class FlatteningExpressionVisitor : ExpressionVisitor
    {
        readonly List<Expression> flattened = new List<Expression>();

        FlatteningExpressionVisitor()
        {            
        }

        public static List<Expression> Flatten(Expression e)
        {
            var visitor = new FlatteningExpressionVisitor();
            visitor.Visit(e);
            return visitor.flattened;
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;

            if (node.NodeType != ExpressionType.Quote)
                flattened.Add(node);

            return base.Visit(node);
        }
    }
}