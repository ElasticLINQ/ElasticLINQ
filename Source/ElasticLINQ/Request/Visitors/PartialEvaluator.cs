// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Decides which branches of the expression tree can be locally evaluated and then
    /// compiles and invokes them to get the results as constant values that can be
    /// then sent to the remote LINQ provider.
    /// </summary>
    public static class PartialEvaluator
    {
        public static Expression Evaluate(Expression expression, Func<Expression, bool> evaluationDecider)
        {
            var nominator = new Nominator(evaluationDecider).Nominate(expression);
            return new SubtreeEvaluator(nominator).Eval(expression);
        }

        public static Expression Evaluate(Expression expression)
        {
            return Evaluate(expression, e => e.NodeType != ExpressionType.Parameter);
        }

        /// <summary>
        /// Visits expressions replacing the candidates given with the results
        /// from their compilation and subsequent execution.
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression e)
            {
                return Visit(e);
            }

            public override Expression Visit(Expression e)
            {
                if (e == null)
                    return null;

                return candidates.Contains(e)
                    ? EvaluateInternal(e)
                    : base.Visit(e);
            }

            private static Expression EvaluateInternal(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                    return e;

                var compiled = Expression.Lambda(e).Compile();
                return Expression.Constant(compiled.DynamicInvoke(null), e.Type);
            }
        }

        /// <summary>
        /// Takes a decider and tests the expression tree to see
        /// which parts of the tree are candidates for evaluation.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> decider;
            private HashSet<Expression> candidates;
            bool decision;

            internal Nominator(Func<Expression, bool> decider)
            {
                this.decider = decider;
            }

            internal HashSet<Expression> Nominate(Expression e)
            {
                candidates = new HashSet<Expression>();
                Visit(e);
                return candidates;
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
                    if (decider(e))
                        candidates.Add(e);
                    else
                        decision = true;
                }

                decision |= priorDecision;
                return e;
            }
        }
    }
}