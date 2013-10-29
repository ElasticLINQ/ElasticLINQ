// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class QueryStringCriteriaTests
    {
        [Fact]
        public void ConstructorSetsPropertiesFromArguments()
        {
            const string expectedValue = "r2d2"; 

            var queryStringCriteria = new QueryStringCriteria(expectedValue);

            Assert.Equal(expectedValue, queryStringCriteria.Value);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryStringCriteria(null));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenValueIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new QueryStringCriteria(""));
        }
    }
}