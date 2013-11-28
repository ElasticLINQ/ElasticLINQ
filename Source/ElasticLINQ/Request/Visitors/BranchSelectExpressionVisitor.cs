// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Tests expressions to find interesting branches of the tree.
    /// Can be used for things like finding candidates for partial evaluation.
    /// </summary>
    internal class BranchSelectExpressionVisitor : ExpressionVisitor
    {
        private readonly HashSet<Expression> matches = new HashSet<Expression>();
        private readonly Func<Expression, bool> predicate;
        private bool decision;

        private BranchSelectExpressionVisitor(Func<Expression, bool> predicate)
        {
            this.predicate = predicate;
        }

        internal static HashSet<Expression> Select(Expression e, Func<Expression, bool> predicate)
        {
            var visitor = new BranchSelectExpressionVisitor(predicate);
            visitor.Visit(e);
            return visitor.matches;
        }

        public override Expression Visit(Expression e)
        {
            if (e == null)
                return null;

            var priorDecision = decision;
            decision = false;
            base.Visit(e);

            if (!decision)
            {
                if (predicate(e))
                    matches.Add(e);
                else
                    decision = true;
            }

            decision |= priorDecision;
            return e;
        }
    }
}