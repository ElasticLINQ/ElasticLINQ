using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ElasticSpiking.BasicProvider
{
    public abstract class ProjectionRow
    {
        public abstract object GetValue(int index);
    }

    internal class ColumnProjection
    {
        internal string Columns;
        internal Expression Selector;
    }

    internal class ColumnProjector : ExpressionVisitor
    {
        private static MethodInfo miGetValue;
        private int iColumn;
        private ParameterExpression row;
        private StringBuilder sb;

        internal ColumnProjector()
        {
            if (miGetValue == null)
                miGetValue = typeof(ProjectionRow).GetMethod("GetValue");
        }

        internal ColumnProjection ProjectColumns(Expression expression, ParameterExpression row)
        {
            sb = new StringBuilder();
            this.row = row;
            var selector = Visit(expression);
            return new ColumnProjection { Columns = sb.ToString(), Selector = selector };
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.Append(m.Member.Name);

                return Expression.Convert(Expression.Call(row, miGetValue, Expression.Constant(iColumn++)), m.Type);
            }

            return base.VisitMemberAccess(m);
        }
    }
}