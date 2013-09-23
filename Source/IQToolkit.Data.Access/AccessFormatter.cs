// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IQToolkit.Data.Access
{
    using IQToolkit.Data.Common;

    /// <summary>
    /// Formats a query expression into MS Access query syntax
    /// </summary>
    public class AccessFormatter : SqlFormatter
    {
        private AccessFormatter(QueryLanguage language)
            : base(language)
        {
        }

        public static new string Format(Expression expression)
        {
            var formatter = new AccessFormatter(new AccessLanguage());
            formatter.FormatWithParameters(expression);
            return formatter.ToString();
        }

        protected virtual void FormatWithParameters(Expression expression)
        {
            var names = NamedValueGatherer.Gather(expression);
            if (names.Count > 0)
            {
                this.Write("PARAMETERS ");
                for (int i = 0, n = names.Count; i < n; i++)
                {
                    if (i > 0)
                        this.Write(", ");
                    this.WriteParameterName(names[i].Name);
                    this.Write(" ");
                    this.Write(this.Language.TypeSystem.GetVariableDeclaration(names[i].QueryType, true));
                }
                this.Write(";");
                this.WriteLine(Indentation.Same);
            }
            this.Visit(expression);
        }

        protected override void WriteParameterName(string name)
        {
            this.Write(name);        
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            if (select.Skip != null)
            {
                if (select.OrderBy == null && select.OrderBy.Count == 0)
                {
                    throw new NotSupportedException("Access cannot support the 'skip' operation without explicit ordering");
                }
                else if (select.Take == null)
                {
                    throw new NotSupportedException("Access cannot support the 'skip' operation without the 'take' operation");
                }
                else
                {
                    throw new NotSupportedException("Access cannot support the 'skip' operation in this query");
                }
            }
            return base.VisitSelect(select);
        }

        protected override void WriteTopClause(Expression expression)
        {
            this.Write("TOP ");
            this.Write(expression);
            this.Write(" ");
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            if (join.Join == JoinType.CrossJoin)
            {
                this.VisitJoinLeft(join.Left);
                this.Write(", ");
                this.VisitJoinRight(join.Right);
                return join;
            }
            return base.VisitJoin(join);
        }

        protected override Expression VisitJoinLeft(Expression source)
        {
            if (source is JoinExpression)
            {
                this.Write("(");
                this.VisitSource(source);
                this.Write(")");
            }
            else
            {
                this.VisitSource(source);
            }
            return source;
        }

        protected override Expression VisitDeclaration(DeclarationCommand decl)
        {
            if (decl.Source != null)
            {
                this.Visit(decl.Source);
                return decl;
            }
            return base.VisitDeclaration(decl);
        }

        protected override void WriteColumns(System.Collections.ObjectModel.ReadOnlyCollection<ColumnDeclaration> columns)
        {
            if (columns.Count == 0)
            {
                this.Write("0");
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
                        this.Write("Len(");
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
                        this.Write("Day(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Month":
                        this.Write("Month(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Year":
                        this.Write("Year(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Hour":
                        this.Write("Hour( ");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Minute":
                        this.Write("Minute(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "Second":
                        this.Write("Second(");
                        this.Visit(m.Expression);
                        this.Write(")");
                        return m;
                    case "DayOfWeek":
                        this.Write("(Weekday(");
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
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + '%')");
                        return m;
                    case "EndsWith":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Contains":
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Write(" + '%')");
                        return m;
                    case "Concat":
                        IList<Expression> args = m.Arguments;
                        if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
                        {
                            args = ((NewArrayExpression)args[0]).Expressions;
                        }
                        for (int i = 0, n = args.Count; i < n; i++)
                        {
                            if (i > 0) this.Write(" + ");
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
                        this.Write("UCase(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "ToLower":
                        this.Write("LCase(");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "Substring":
                        this.Write("Mid(");
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
                    case "Replace":
                        this.Write("Replace(");
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(", ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "IndexOf":
                        this.Write("(InStr(");
                        if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                        {
                            this.Visit(m.Arguments[1]);
                            this.Write(" + 1, ");
                        }
                        this.Visit(m.Object);
                        this.Write(", ");
                        this.Visit(m.Arguments[0]);
                        this.Write(") - 1)");
                        return m;
                    case "Trim":
                        this.Write("Trim(");
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
                            this.Write("DateDiff(\"d\",");
                            this.Visit(m.Arguments[0]);
                            this.Write(",");
                            this.Visit(m.Arguments[1]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "AddYears":
                        this.Write("DateAdd(\"yyyy\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddMonths":
                        this.Write("DateAdd(\"m\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddDays":
                        this.Write("DateAdd(\"d\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddHours":
                        this.Write("DateAdd(\"h\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddMinutes":
                        this.Write("DateAdd(\"n\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
                    case "AddSeconds":
                        this.Write("DateAdd(\"s\",");
                        this.Visit(m.Arguments[0]);
                        this.Write(",");
                        this.Visit(m.Object);
                        this.Write(")");
                        return m;
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
                    case "Truncate":
                        this.Write("Fix");
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("Round(");
                            this.Visit(m.Arguments[0]);
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
                    case "Cos":
                    case "Exp":
                    case "Sin":
                    case "Tan":
                        this.Write(m.Method.Name.ToUpper());
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Sqrt":
                        this.Write("Sqr(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Sign":
                        this.Write("Sgn(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Atan":
                        this.Write("Atn(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    case "Log":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("Log(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "Pow":
                        this.Visit(m.Arguments[0]);
                        this.Write("^");
                        this.Visit(m.Arguments[1]);
                        return m;
                    case "Round":
                        if (m.Arguments.Count == 1)
                        {
                            this.Write("Round(");
                            this.Visit(m.Arguments[0]);
                            this.Write(")");
                            return m;
                        }
                        break;
                    case "Truncate":
                        this.Write("Fix(");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                }
            }
            if (m.Method.Name == "ToString")
            {
                if (m.Object.Type != typeof(string))
                {
                    this.Write("CStr(");
                    this.Visit(m.Object);
                    this.Write(")");
                }
                else
                {
                    this.Visit(m.Object);
                }
                return m;
            }
            else if (!m.Method.IsStatic && m.Method.Name == "CompareTo" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 1)
            {
                this.Write("IIF(");
                this.Visit(m.Object);
                this.Write(" = ");
                this.Visit(m.Arguments[0]);
                this.Write(", 0, IIF(");
                this.Visit(m.Object);
                this.Write(" < ");
                this.Visit(m.Arguments[0]);
                this.Write(", -1, 1))");
                return m;
            }
            else if (m.Method.IsStatic && m.Method.Name == "Compare" && m.Method.ReturnType == typeof(int) && m.Arguments.Count == 2)
            {
                this.Write("IIF(");
                this.Visit(m.Arguments[0]);
                this.Write(" = ");
                this.Visit(m.Arguments[1]);
                this.Write(", 0, IIF(");
                this.Visit(m.Arguments[0]);
                this.Write(" < ");
                this.Visit(m.Arguments[1]);
                this.Write(", -1, 1))");
                return m;
            }
            return base.VisitMethodCall(m);
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Constructor.DeclaringType == typeof(DateTime))
            {
                if (nex.Arguments.Count == 3)
                {
                    this.Write("CDate(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(")");
                    return nex;
                }
                else if (nex.Arguments.Count == 6)
                {
                    this.Write("CDate(");
                    this.Visit(nex.Arguments[0]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[1]);
                    this.Write(" & '/' & ");
                    this.Visit(nex.Arguments[2]);
                    this.Write(" & ' ' & ");
                    this.Visit(nex.Arguments[3]);
                    this.Write(" & ':' & ");
                    this.Visit(nex.Arguments[4]);
                    this.Write(" & + ':' & ");
                    this.Visit(nex.Arguments[5]);
                    this.Write(")");
                    return nex;
                }
            }
            return base.VisitNew(nex);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Power)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write("^");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.Coalesce)
            {
                this.Write("IIF(");
                this.VisitValue(b.Left);
                this.Write(" IS NOT NULL, ");
                this.VisitValue(b.Left);
                this.Write(", ");
                this.VisitValue(b.Right);
                this.Write(")");
                return b;
            }
            else if (b.NodeType == ExpressionType.LeftShift)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write(" * (2^");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            else if (b.NodeType == ExpressionType.RightShift)
            {
                this.Write("(");
                this.VisitValue(b.Left);
                this.Write(@" \ (2^");
                this.VisitValue(b.Right);
                this.Write("))");
                return b;
            }
            return base.VisitBinary(b);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            this.Write("IIF(");
            this.VisitPredicate(c.Test);
            this.Write(", ");
            this.VisitValue(c.IfTrue);
            this.Write(", ");
            this.VisitValue(c.IfFalse);
            this.Write(")");
            return c;
        }

        protected override string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    if (b.Type == typeof(bool) || b.Type == typeof(bool?))
                        return "AND";
                    return "BAND";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Or:
                    if (b.Type == typeof(bool) || b.Type == typeof(bool?))
                        return "OR";
                    return "BOR";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.ExclusiveOr:
                    return "XOR";
                case ExpressionType.Divide:
                    if (this.IsInteger(b.Type))
                        return "\\"; // integer divide
                    goto default;
                default:
                    return base.GetOperator(b);
            }
        }

        protected override string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    return "NOT";
                default:
                    return base.GetOperator(u);
            }
        }

        protected override string GetOperator(string methodName)
        {
            if (methodName == "Remainder")
            {
                return "MOD";
            }
            else
            {
                return base.GetOperator(methodName);
            }
        }

        protected override void WriteValue(object value)
        {
            if (value != null && value.GetType() == typeof(bool))
            {
                this.Write(((bool)value) ? -1 : 0);
            }
            else 
            {
                base.WriteValue(value);
            }
        }
    }
}
