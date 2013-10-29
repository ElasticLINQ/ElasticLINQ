// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class MissingCriteriaTests
    {
        [Fact]
        public void NamePropertyIsMissing()
        {
            var criteria = new MissingCriteria("something");

            Assert.Equal("missing", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            const string field = "myField";

            var criteria = new MissingCriteria(field);

            Assert.Equal(field, criteria.Field);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MissingCriteria(null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new MissingCriteria(" "));
        }

        [Fact]
        public void ToStringContainsFieldComparisonAndValue()
        {
            var missingCriteria = new MissingCriteria("thisIsAMissingField");
            var result = missingCriteria.ToString();

            Assert.Contains(missingCriteria.Field, result);
        }
   }
}