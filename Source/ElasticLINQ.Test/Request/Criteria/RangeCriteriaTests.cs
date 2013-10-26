// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class RangeCriteriaTests
    {
        [Fact]
        public void NamePropertyIsRange()
        {
            var criteria = new RangeCriteria("field", RangeComparison.GreaterThan, 1);

            Assert.Equal("range", criteria.Name);
        }

        [Fact]
        public void SingleConstructorSetsSingleSpecification()
        {
            var criteria = new RangeCriteria("field", RangeComparison.GreaterThan, 1);

            Assert.Equal("field", criteria.Field);            
            Assert.Equal(1, criteria.Specifications.Count);

            var specification = criteria.Specifications.Single();
            Assert.Equal("gt", specification.Name);
            Assert.Equal(1, specification.Value);
        }

        [Fact]
        public void EnumerableConstructorSetsMultipleSpecifications()
        {
            var greater = new RangeSpecificationCriteria(RangeComparison.GreaterThanOrEqual, "D");
            var less = new RangeSpecificationCriteria(RangeComparison.LessThan, "H");

            var criteria = new RangeCriteria("initials", new [] { greater, less });

            Assert.Equal("initials", criteria.Field);
            Assert.Contains(greater, criteria.Specifications);
            Assert.Contains(less, criteria.Specifications);
            Assert.Equal(2, criteria.Specifications.Count);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria(null, RangeComparison.GreaterThan, 1));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new RangeCriteria(" ", RangeComparison.GreaterThan, 1));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenComparisonsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria("field", null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentOutOfRangeExceptionWhenRangeComparisonIsNotDefined()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RangeCriteria(" ", (RangeComparison)99, 1));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeCriteria("field", RangeComparison.GreaterThan, null));
        }

        [Fact]
        public void RangeComparisonsMapToExpectedNames()
        {
            var gt = new RangeSpecificationCriteria(RangeComparison.GreaterThan, 1);
            var gte = new RangeSpecificationCriteria(RangeComparison.GreaterThanOrEqual, 1);
            var lt = new RangeSpecificationCriteria(RangeComparison.LessThan, 1);
            var lte = new RangeSpecificationCriteria(RangeComparison.LessThanOrEqual, 1);

            Assert.Equal(gt.Name, "gt");
            Assert.Equal(gte.Name, "gte");
            Assert.Equal(lt.Name, "lt");
            Assert.Equal(lte.Name, "lte");
        }
    }
}