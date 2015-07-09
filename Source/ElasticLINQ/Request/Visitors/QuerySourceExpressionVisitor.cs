// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    class QuerySourceExpressionVisitor : ExpressionVisitor
    {
        IQueryable sourceQueryable;

        QuerySourceExpressionVisitor()
        {
        }

        public static IQueryable FindSource(Expression e)
        {
            var visitor = new QuerySourceExpressionVisitor();
            visitor.Visit(e);
            return visitor.sourceQueryable;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is IQueryable)
                sourceQueryable = ((IQueryable)node.Value);

            return node;
        }
    }
}