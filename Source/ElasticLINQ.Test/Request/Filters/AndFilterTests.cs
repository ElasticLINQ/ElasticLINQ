// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Request.Filters
{
    public class AndFilterTests
    {
        readonly IFilter sampleFilter1 = new TermFilter("first", "1st");
        readonly IFilter sampleFilter2 = new TermFilter("second", "2nd");

        [Fact]
        public void NamePropertyIsAnd()
        {
            var filter = new AndFilter();

            Assert.Equal("and", filter.Name);
        }

        [Fact]
        public void ConstructorSetsFilters()
        {
            var filter = new AndFilter(sampleFilter1, sampleFilter2);

            Assert.Contains(sampleFilter1, filter.Filters);
            Assert.Contains(sampleFilter2, filter.Filters);
            Assert.Equal(2, filter.Filters.Count);
        }

        [Fact]
        public void CombineThrowArgumentNullExceptionWhenFiltersIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => AndFilter.Combine(null));
        }

        [Fact]
        public void CombineWithEmptyListReturnsEmptyAnd()
        {
            var filter = AndFilter.Combine(new IFilter[] { });

            Assert.NotNull(filter);
            Assert.Empty(filter.Filters);
        }

        [Fact]
        public void CombineWithFiltersCombines()
        {
            var filter = AndFilter.Combine(sampleFilter1, sampleFilter2);

            Assert.Contains(sampleFilter1, filter.Filters);
            Assert.Contains(sampleFilter2, filter.Filters);
            Assert.Equal(2, filter.Filters.Count);
        }

        [Fact]
        public void CombineWithRangeFiltersCombinesRanges()
        {
            var lowerFirstRange = new RangeFilter("first", RangeComparison.GreaterThan, "lower");
            var upperFirstRange = new RangeFilter("first", RangeComparison.LessThanOrEqual, "upper");
            var secondRange = new RangeFilter("second", RangeComparison.GreaterThanOrEqual, "lower2");

            var andFilter = AndFilter.Combine(lowerFirstRange, secondRange, upperFirstRange);

            Assert.Equal(2, andFilter.Filters.Count);
            Assert.Contains(secondRange, andFilter.Filters);

            var combinedRange = andFilter.Filters.OfType<RangeFilter>().FirstOrDefault(r => r.Specifications.Count == 2);
            Assert.NotNull(combinedRange);
            Assert.Equal(lowerFirstRange.Field, combinedRange.Field);
            Assert.Single(combinedRange.Specifications, s => s.Comparison == lowerFirstRange.Specifications.First().Comparison);
            Assert.Single(combinedRange.Specifications, s => s.Comparison == upperFirstRange.Specifications.First().Comparison);
        }
    }
}