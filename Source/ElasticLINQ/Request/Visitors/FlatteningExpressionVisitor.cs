// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Flattens an expression tree into a list of expressions for debugging
    /// and testing senarios.
    /// </summary>
    internal class FlatteningExpressionVisitor : ExpressionVisitor
    {
        private readonly List<Expression> flattened = new List<Expression>();

        private FlatteningExpressionVisitor()
        {            
        }

        public static List<Expression> Flatten(Expression e)
        {
            var visitor = new FlatteningExpressionVisitor();
            visitor.Visit(e);
            return visitor.flattened;
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;

            if (node.NodeType != ExpressionType.Quote)
                flattened.Add(node);
            return base.Visit(node);
        }
    }
}