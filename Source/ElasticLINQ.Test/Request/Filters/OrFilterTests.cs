// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Filters
{
    public class OrFilterTests
    {
        private readonly TermFilter salutationFilterMr = new TermFilter("salutation", "Mr");
        private readonly TermFilter salutationFilterMrs = new TermFilter("salutation", "Mrs");
        private readonly TermFilter salutationFilterMs = new TermFilter("salutation", "Miss", "Ms");
        private readonly TermFilter areaFilter408 = new TermFilter("area", "408");

        [Fact]
        public void NamePropertyIsOr()
        {
            var filter = new OrFilter();

            Assert.Equal("or", filter.Name);
        }

        [Fact]
        public void ConstructorSetsFilters()
        {
            var filter = new OrFilter(salutationFilterMr, areaFilter408);

            Assert.Contains(salutationFilterMr, filter.Filters);
            Assert.Contains(areaFilter408, filter.Filters);
            Assert.Equal(2, filter.Filters.Count);
        }

        [Fact]
        public void CombineWithEmptyListReturnsEmptyOr()
        {
            var filter = OrFilter.Combine(new IFilter[] { });

            Assert.IsType<OrFilter>(filter);
            Assert.Empty(((OrFilter)filter).Filters);
        }

        [Fact]
        public void CombineWithAllTermFieldsSameCombinesIntoSingleTerm()
        {
            var filter = OrFilter.Combine(salutationFilterMr, salutationFilterMrs, salutationFilterMs);

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal(termFilter.Field, salutationFilterMr.Field);

            var allValues = salutationFilterMr.Values.Concat(salutationFilterMrs.Values).Concat(salutationFilterMs.Values).Distinct().ToArray();
            foreach (var value in allValues)
                Assert.Contains(value, termFilter.Values);

            Assert.Equal(allValues.Length, termFilter.Values.Count);
        }

        [Fact]
        public void CombineWithDifferTermFiltersDoesNotCombines()
        {
            var filter = OrFilter.Combine(salutationFilterMr, salutationFilterMrs, areaFilter408);

            Assert.IsType<OrFilter>(filter);
            var orFilter = (OrFilter)filter;
            Assert.Contains(salutationFilterMr, orFilter.Filters);
            Assert.Contains(salutationFilterMrs, orFilter.Filters);
            Assert.Contains(areaFilter408, orFilter.Filters);
            Assert.Equal(3, orFilter.Filters.Count);
        }
    }
}