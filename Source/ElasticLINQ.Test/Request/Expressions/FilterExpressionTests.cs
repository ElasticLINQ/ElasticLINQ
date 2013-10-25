// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Filters;
using Xunit;

namespace ElasticLinq.Test.Request.Expressions
{
    public class FilterExpressionTests
    {
        [Fact]
        public void ConstructorSetsFilter()
        {
            var filter = new TermFilter("field", "value");

            var expression = new FilterExpression(filter);

            Assert.Same(filter, expression.Filter);
        }

        [Fact]
        public void ExpressionsTypeIsBool()
        {
            var filter = new TermFilter("field", "value");

            var expression = new FilterExpression(filter);

            Assert.Equal(typeof(bool), expression.Type);
        }
    }
}