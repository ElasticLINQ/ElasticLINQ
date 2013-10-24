// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Visitors;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors
{
    public class FlatteningExpressionVisitorTests
    {
        [Fact]
        public void FlattenReturnsListOfAllExpressionNodes()
        {
            Expression<Func<int, bool>> sampleExpression = a => a > 2;

            var flattened = FlatteningExpressionVisitor.Flatten(sampleExpression);

            Assert.Equal(ExpressionType.Lambda, flattened[0].NodeType);
            Assert.Equal(ExpressionType.GreaterThan, flattened[1].NodeType);
            Assert.Equal(ExpressionType.Parameter, flattened[2].NodeType);
            Assert.Equal(ExpressionType.Constant, flattened[3].NodeType);
            Assert.Equal(ExpressionType.Parameter, flattened[4].NodeType);

            Assert.Equal(5, flattened.Count);
        }
    }
}