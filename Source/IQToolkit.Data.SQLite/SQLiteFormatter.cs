using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace IQToolkit.Data.SQLite
{
    using IQToolkit.Data.Common;

    public class SQLiteFormatter : SqlFormatter
    {
        private SQLiteFormatter(QueryLanguage language)
            : base(language)
        {
        }

        public static new string Format(Expression expression)
        {
            return Format(expression, new SQLiteLanguage());
        }

        public static string Format(Expression expression, QueryLanguage language)
        {
            SQLiteFormatter formatter = new SQLiteFormatter(language);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            this.AddAliases(select.From);
            this.Write("SELECT ");
            if (select.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            this.WriteColumns(select.Columns);
            if (select.From != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("FROM ");
                this.VisitSource(select.From);
            }
            if (select.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.VisitPredicate(select.Where);
            }
            if (select.GroupBy != null && select.GroupBy.Count > 0)
            {
                this.WriteLine(Indentation.Same);
                this.Write("GROUP BY ");
                for (int i = 0, n = select.GroupBy.Count; i < n; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(select.GroupBy[i]);
                }
            }
            if (select.OrderBy != null && select.OrderBy.Count > 0)
            {
                this.WriteLine(Indentation.Same);
                this.Write("ORDER BY ");
                for (int i = 0, n = select.OrderBy.Count; i < n; i++)
                {
                    OrderExpression exp = select.OrderBy[i];
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(exp.Expression);
                    if (exp.OrderType != OrderType.Ascending)
                    {
                        this.Write(" DESC");
                    }
                }
            }
            if (select.Take != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("LIMIT ");
                if (select.Skip == null)
                {
                    this.Write("0");
                }
                else
                {
                    this.Write(select.Skip);
                }
                this.Write(", ");
                this.Visit(select.Take);
            }
            return select;
        }

        protected override void WriteColumns(System.Collections.ObjectModel.ReadOnlyCollection<ColumnDeclaration> columns)
        {
            if (columns.Count == 0)
            {
                this.Write("0");
                if (this.IsNested)
                {
                    this.Write(" AS ");
                    this.WriteColumnName("tmp");
                    this.Write(" ");
                }
            }
            else
            {
                base.WriteColumns(columns);
            }
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(string))
            {
                switch (m.Member.Name)
                {
                    case "Length":
                        this.Write("LENGTH(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                }
            }
            else if (m.Member.DeclaringType == typeof(DateTime) || m.Member.DeclaringType == typeof(DateTimeOffset))
            {
                switch (m.Member.Name)
                {
                    case "Day":
                        this.Write("STRFTIME('%d', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("STRFTIME('%m', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("STRFTIME('%Y', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("STRFTIME('%H', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("STRFTIME('%M', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("STRFTIME('%S', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Millisecond":
                        this.Write("STRFTIME('%f', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "DayOfWeek":
                        this.Write("STRFTIME('%w', ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "DayOfYear":
                        this.Write("(STRFTIME('%j', ");
                        this.Visit(m.Expression);
                        this.Write(") - 1)");
                        return m;
                }
            }
            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(string))
            {
                switch (m.Method.Name)
                {
                    case "StartsWith":
                        this.Write("Like(");
                        this.Visit(m.Arguments[0]);
                        this.Write(" || '%', ");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "EndsWith":
                        this.Write("Like('%' || ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Contains":
                        this.Write("Like('%' || ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" || '%', ");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Concat":
                        IList<Expression> args = m.Arguments;
                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                        {
                            args = ((NewArrayExpression)args[0]).Expressions;
                        }
                        for (int i = 0, n = args.Count; i < n; i++)
                        {
                            if (i > 0) this.Write(" || ");
                            this.Visit(args[i]);
                        }
                        return m;
                    case "IsNullOrEmpty":
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(" IS NULL OR ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" = '')");
                        return m;
                    case "ToUpper":
                        this.Write("UPPER(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "ToLower":
                        this.Write("LOWER(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Replace":
                        this.Write("REPLACE(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Substring":
                        this.Write("SUBSTR(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + 1, ");
                        if (m.Arguments.Count == 2)
                        {
                            this.Visit(m.Arguments[1]);
                        }
                        else
                        {
                            this.Write("8000");
                        }
                        this.Write(")");
                        return m;
                    case "Remove":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", 1, ");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                        }
                        else
                        {
                            this.Write("SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", 1, ");
                            this.Visit(m.Arguments[0]);
                            this.Write(") + SUBSTR(");
                            this.Visit(m.Object);
                            this.Write(", ");
                            this.Visit(m.Arguments[0]);
                            this.Write(" + ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                        }
                        return m;
                    case "Trim":
                        this.Write("TRIM(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                }
            }
            else if (m.Method.DeclaringType == typeof(DateTime))
            {
                switch (m.Method.Name)
                {
                    case "op_Subtract":
                        if (m.Arguments[1].Type == typeof(DateTime))
                        {
                            this.Write("DATEDIFF(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                }
            }
            else if (m.Method.DeclaringType == typeof(Decimal))
            {
                switch (m.Method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        this.Write("(");
                        this.VisitValue(m.Arguments[0]);
                        this.Write(" ");
                        this.Write(GetOperator(m.Method.Name));
                        this.Write(" ");
                        this.VisitValue(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Negate":
                        this.Write("-");
                        this.Visit(m.Arguments[0]);
                        this.Write("");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", 0)");
                            return m;
                        }
                        else if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                }
            }
            else if (m.Method.DeclaringType == typeof(Math))
            {
                switch (m.Method.Name)
                {
                    case "Abs":
                    case "Acos":
                    case "Asin":
                    case "Atan":
                    case "Cos":
                    case "Exp":
                    case "Log10":
                    case "Sin":
                    case "Tan":
                    case "Sqrt":
                    case "Sign":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Atan2":
                        this.Write("ATN2(");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Log":
                        if (m.Arguments.Count == 1)
                        {
                            goto case "Log10";
                        }
                        break;
                    case "Pow":
                        this.Write("POWER(");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", 0)");
                            return m;
                        }
                        else if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            this.Write(", ");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                }
            }
            if (m.Method.Name == "ToString")
            {
                // no-op
                this.Visit(m.Object);
                return m;
            }
            else if (!m.Method.IsStatic && m.Method.Name == "CompareTo" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 1)
            {
                this.Write("(CASE WHEN ");
                this.Visit(m.Object);
                this.Write(" = ");
                this.Visit(m.Arguments[0]);
                this.Write(" THEN 0 WHEN ");
                this.Visit(m.Object);
                this.Write(" < ");
                this.Visit(m.Arguments[0]);
                this.Write(" THEN -1 ELSE 1 END)");
                return m;
            }
            else if (m.Method.IsStatic && m.Method.Name == "Compare" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 2)
            {
                this.Write("(CASE WHEN ");
                this.Visit(m.Arguments[0]);
                this.Write(" = ");
                this.Visit(m.Arguments[1]);
                this.Write(" THEN 0 WHEN ");
                this.Visit(m.Arguments[0]);
                this.Write(" < ");
                this.Visit(m.Arguments[1]);
                this.Write(" THEN -1 ELSE 1 END)");
                return m;
            }
            return base.VisitMethodCall(m);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Power)
            {
                this.Write("POWER(");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Coalesce)
            {
                this.Write("COALESCE(");
                this.VisitValue(b.Left);
                this.Write(", ");
                Expression right = b.Right;
                while (right.NodeType == ExpressionType.Coalesce)
                {
                    BinaryExpression rb = (BinaryExpression)right;
                    this.VisitValue(rb.Left);
                    this.Write(", ");
                    right = rb.Right;
                }
                this.VisitValue(right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.ExclusiveOr)
            {
                // SQLite does not have XOR (^).. Use translation:  ((A & ~B) | (~A & B))
                this.Write("((");
                this.VisitValue(b.Left);
                this.Write(" & ~");
                this.VisitValue(b.Right);
                this.Write(") | (~");
                this.VisitValue(b.Left);
                this.Write(" & ");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            return base.VisitBinary(b);
        }

        protected override string GetOperator(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Add && b.Type == typeof(string))
            {
                return "||";
            }
            else
            {
                return base.GetOperator(b);
            }
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Constructor.DeclaringType == typeof(DateTime))
            {
                if (nex.Arguments.Count == 3)
                {
                    this.Write("(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(" || '-' || (CASE WHEN ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" < 10 THEN '0' || ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" ELSE ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" END)");
                    this.Write(" || '-' || (CASE WHEN ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(" < 10 THEN '0' || ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(" ELSE ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(" END)");                                        
                    this.Write(")");
                    return nex;
                }
                else if (nex.Arguments.Count == 6)
                {
                    this.Write("(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(" || '-' || ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" || '-' || ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(" || ' ' || ");
                    this.Visit(nex.Arguments[3]);
                    this.Write(" || ':' || ");
                    this.Visit(nex.Arguments[4]);
                    this.Write(" || ':' || ");
                    this.Visit(nex.Arguments[5]);
                    this.Write(")");
                    return nex;
                }
            }
            return base.VisitNew(nex);
        }

        protected override Expression VisitValue(Expression expr)
        {
            if (IsPredicate(expr))
            {
                this.Write("CASE WHEN (");
                this.Visit(expr);
                this.Write(") THEN 1 ELSE 0 END");
                return expr;
            }
            return base.VisitValue(expr);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if (this.IsPredicate(c.Test))
            {
                this.Write("(CASE WHEN ");
                this.VisitPredicate(c.Test);
                this.Write(" THEN ");
                this.VisitValue(c.IfTrue);
                Expression ifFalse = c.IfFalse;
                while (ifFalse != null && ifFalse.NodeType == ExpressionType.Conditional)
                {
                    ConditionalExpression fc = (ConditionalExpression)ifFalse;
                    this.Write(" WHEN ");
                    this.VisitPredicate(fc.Test);
                    this.Write(" THEN ");
                    this.VisitValue(fc.IfTrue);
                    ifFalse = fc.IfFalse;
                }
                if (ifFalse != null)
                {
                    this.Write(" ELSE ");
                    this.VisitValue(ifFalse);
                }
                this.Write(" END)");
            }
            else
            {
                this.Write("(CASE ");
                this.VisitValue(c.Test);
                this.Write(" WHEN 0 THEN ");
                this.VisitValue(c.IfFalse);
                this.Write(" ELSE ");
                this.VisitValue(c.IfTrue);
                this.Write(" END)");
            }
            return c;
        }
    }
}
