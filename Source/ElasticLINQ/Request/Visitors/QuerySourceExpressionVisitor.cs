// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    internal class QuerySourceExpressionVisitor : ExpressionVisitor
    {
        private IQueryable sourceQueryable;

        private QuerySourceExpressionVisitor()
        {
        }

        public static IQueryable FindSource(Expression e)
        {
            var visitor = new QuerySourceExpressionVisitor();
            visitor.Visit(e);
            return visitor.sourceQueryable;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value is IQueryable)
                sourceQueryable = ((IQueryable)c.Value);

            return c;
        }
    }
}