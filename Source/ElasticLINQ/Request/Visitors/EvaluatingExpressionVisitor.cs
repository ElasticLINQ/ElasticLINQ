// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Visits the expression tree and for any node in its list
    /// it replaces that expression with a constant expression
    /// that resulted from it's compilation and invocation.
    /// </summary>
    internal class EvaluatingExpressionVisitor : ExpressionVisitor
    {
        private readonly HashSet<Expression> chosenForEvaluation;

        private EvaluatingExpressionVisitor(HashSet<Expression> chosenForEvaluation)
        {
            this.chosenForEvaluation = chosenForEvaluation;
        }

        internal static Expression Evaluate(Expression e, HashSet<Expression> chosenForEvaluation)
        {
            return new EvaluatingExpressionVisitor(chosenForEvaluation).Visit(e);
        }

        public override Expression Visit(Expression e)
        {
            if (e == null || e.NodeType == ExpressionType.Constant)
                return e;

            return chosenForEvaluation.Contains(e)
                ? Expression.Constant(Expression.Lambda(e).Compile().DynamicInvoke(null), e.Type)
                : base.Visit(e);
        }
    }
}