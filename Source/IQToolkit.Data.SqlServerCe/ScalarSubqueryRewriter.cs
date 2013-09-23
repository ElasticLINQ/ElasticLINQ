// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQToolkit.Data.SqlServerCe
{
    using IQToolkit.Data.Common;

    /// <summary>
    /// SQLCE doesn't understand scalar subqueries (???) but it does understand cross/outer apply.
    /// Convert scalar subqueries into OUTER APPLY
    /// </summary>
    public class ScalarSubqueryRewriter : DbExpressionVisitor
    {
        QueryLanguage language;
        Expression currentFrom;

        public ScalarSubqueryRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new ScalarSubqueryRewriter(language).Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            var saveFrom = this.currentFrom;

            var from = this.VisitSource(select.From);
            this.currentFrom = from;

            var where = this.Visit(select.Where);
            var orderBy = this.VisitOrderBy(select.OrderBy);
            var groupBy = this.VisitExpressionList(select.GroupBy);
            var skip = this.Visit(select.Skip);
            var take = this.Visit(select.Take);
            var columns = this.VisitColumnDeclarations(select.Columns);

            from = this.currentFrom;
            this.currentFrom = saveFrom;

            return this.UpdateSelect(select, from, where, orderBy, groupBy, skip, take, select.IsDistinct, select.IsReverse, columns);
        }

        protected override Expression VisitScalar(ScalarExpression scalar)
        {
            var select = scalar.Select;
            var colType = this.language.TypeSystem.GetColumnType(scalar.Type);
            if (string.IsNullOrEmpty(select.Columns[0].Name))
            {
                var name = select.Columns.GetAvailableColumnName("scalar");
                select = select.SetColumns(new[] { new ColumnDeclaration(name, select.Columns[0].Expression, colType) });
            }
            this.currentFrom = new JoinExpression(JoinType.OuterApply, this.currentFrom, select, null);
            return new ColumnExpression(scalar.Type, colType, scalar.Select.Alias, select.Columns[0].Name);
        }
    }
}