// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Expressions;
using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Expressions
{
    public class CriteriaExpressionTests
    {
        [Fact]
        public void ConstructorSetsCriteria()
        {
            var criteria = new TermCriteria("field", "value");

            var expression = new CriteriaExpression(criteria);

            Assert.Same(criteria, expression.Criteria);
        }

        [Fact]
        public void ExpressionsTypeIsBool()
        {
            var criteria = new TermCriteria("field", "value");

            var expression = new CriteriaExpression(criteria);

            Assert.Equal(typeof(bool), expression.Type);
        }
    }
}