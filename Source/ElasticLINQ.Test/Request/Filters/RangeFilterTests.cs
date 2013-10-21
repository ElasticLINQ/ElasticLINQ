// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Request.Filters
{
    public class RangeFilterTests
    {
        [Fact]
        public void NamePropertyIsRange()
        {
            var filter = new RangeFilter("field", RangeComparison.GreaterThan, 1);

            Assert.Equal("range", filter.Name);
        }

        [Fact]
        public void SingleConstructorSetsSingleSpecification()
        {
            var filter = new RangeFilter("field", RangeComparison.GreaterThan, 1);

            Assert.Equal("field", filter.Field);            
            Assert.Equal(1, filter.Specifications.Count);

            var specification = filter.Specifications.Single();
            Assert.Equal("gt", specification.Name);
            Assert.Equal(1, specification.Value);
        }

        [Fact]
        public void EnumerableConstructorSetsMultipleSpecifications()
        {
            var greater = new RangeSpecificationFilter(RangeComparison.GreaterThanOrEqual, "D");
            var less = new RangeSpecificationFilter(RangeComparison.LessThan, "H");

            var filter = new RangeFilter("initials", new [] { greater, less });

            Assert.Equal("initials", filter.Field);
            Assert.Contains(greater, filter.Specifications);
            Assert.Contains(less, filter.Specifications);
            Assert.Equal(2, filter.Specifications.Count);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeFilter(null, RangeComparison.GreaterThan, 1));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new RangeFilter(" ", RangeComparison.GreaterThan, 1));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenComparisonsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeFilter("field", null));
        }

        [Fact]
        public void ConstructorThrowsArgumentOutOfRangeExceptionWhenRangeComparisonIsNotDefined()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RangeFilter(" ", (RangeComparison)99, 1));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RangeFilter("field", RangeComparison.GreaterThan, null));
        }

        [Fact]
        public void RangeComparisonsMapToExpectedNames()
        {
            var gt = new RangeSpecificationFilter(RangeComparison.GreaterThan, 1);
            var gte = new RangeSpecificationFilter(RangeComparison.GreaterThanOrEqual, 1);
            var lt = new RangeSpecificationFilter(RangeComparison.LessThan, 1);
            var lte = new RangeSpecificationFilter(RangeComparison.LessThanOrEqual, 1);

            Assert.Equal(gt.Name, "gt");
            Assert.Equal(gte.Name, "gte");
            Assert.Equal(lt.Name, "lt");
            Assert.Equal(lte.Name, "lte");
        }
    }
}