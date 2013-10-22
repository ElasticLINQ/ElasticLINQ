// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors
{
    public class EvaluatingExpressionVisitorTests
    {
        [Fact]
        public void EvaluateReplacesExpressionsWithConstantExpressions()
        {
            Expression<Func<DateTime>> call = () => DateTime.Now.Subtract(TimeSpan.FromHours(5));
            var nodeToEvaluate = FlatteningExpressionVisitor.Flatten(call).OfType<MethodCallExpression>().Single(s => s.Method.Name == "FromHours");

            var evaluation = EvaluatingExpressionVisitor.Evaluate(call, new HashSet<Expression>(new[] { nodeToEvaluate }));

            var constantNodes = FlatteningExpressionVisitor.Flatten(evaluation).OfType<ConstantExpression>().ToArray();
            Assert.Equal(1, constantNodes.Length);
            Assert.Equal(TimeSpan.FromHours(5), constantNodes.Single().Value);
        }
    }
}