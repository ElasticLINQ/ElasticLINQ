// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQToolkit.Data.Common
{
    /// <summary>
    /// Attempt to rewrite cross joins as inner joins
    /// </summary>
    public class CrossJoinRewriter : DbExpressionVisitor
    {
        private Expression currentWhere;

        public static Expression Rewrite(Expression expression)
        {
            return new CrossJoinRewriter().Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            var saveWhere = this.currentWhere;
            try
            {
                this.currentWhere = select.Where;
                var result = (SelectExpression)base.VisitSelect(select);
                if (this.currentWhere != result.Where)
                {
                    return result.SetWhere(this.currentWhere);
                }
                return result;
            }
            finally
            {
                this.currentWhere = saveWhere;
            }
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            join = (JoinExpression)base.VisitJoin(join);
            if (join.Join == JoinType.CrossJoin && this.currentWhere != null)
            {
                // try to figure out which parts of the current where expression can be used for a join condition
                var declaredLeft = DeclaredAliasGatherer.Gather(join.Left);
                var declaredRight = DeclaredAliasGatherer.Gather(join.Right);
                var declared = new HashSet<TableAlias>(declaredLeft.Union(declaredRight));
                var exprs = this.currentWhere.Split(ExpressionType.And, ExpressionType.AndAlso);
                var good = exprs.Where(e => CanBeJoinCondition(e, declaredLeft, declaredRight, declared)).ToList();
                if (good.Count > 0)
                {
                    var condition = good.Join(ExpressionType.And);
                    join = this.UpdateJoin(join, JoinType.InnerJoin, join.Left, join.Right, condition);
                    var newWhere = exprs.Where(e => !good.Contains(e)).Join(ExpressionType.And);
                    this.currentWhere = newWhere;
                }
            }
            return join;
        }

        private bool CanBeJoinCondition(Expression expression, HashSet<TableAlias> left, HashSet<TableAlias> right, HashSet<TableAlias> all)
        {
            // an expression is good if it has at least one reference to an alias from both left & right sets and does
            // not have any additional references that are not in both left & right sets
            var referenced = ReferencedAliasGatherer.Gather(expression);
            var leftOkay = referenced.Intersect(left).Any();
            var rightOkay = referenced.Intersect(right).Any();
            var subset = referenced.IsSubsetOf(all);
            return leftOkay && rightOkay && subset;
        }
    }
}