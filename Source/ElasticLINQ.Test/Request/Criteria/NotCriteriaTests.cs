// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class NotCriteriaTests
    {
        private readonly ICriteria sampleTerm = new TermCriteria("field", "value");

        [Fact]
        public void NamePropertyIsNot()
        {
            var criteria = NotCriteria.Create(sampleTerm);

            Assert.Equal("not", criteria.Name);
        }

        [Fact]
        public void CreateReturnsNotCriteriaWithChildCriteriaSet()
        {
            var criteria = NotCriteria.Create(sampleTerm);

            Assert.IsType<NotCriteria>(criteria);
            Assert.Equal(sampleTerm, ((NotCriteria)criteria).Criteria);
        }

        [Fact]
        public void CreateUnwrapsNestedNotCriteria()
        {
            var criteria = NotCriteria.Create(NotCriteria.Create(sampleTerm));

            Assert.IsType<TermCriteria>(criteria);
            Assert.Equal(sampleTerm, criteria);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => NotCriteria.Create(null));
        }
   }
}