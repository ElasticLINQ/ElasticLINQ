// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Linq;
using ElasticLinq.Request.Filters;
using Xunit;

namespace ElasticLINQ.Test.Request.Filters
{
    public class TermFilterTests
    {
        [Fact]
        public void NamePropertyIsTermWhenOneTerm()
        {
            var filter = new TermFilter("field", 1);

            Assert.Equal("term", filter.Name);
            Assert.Equal(1, filter.Values.Count);
        }

        [Fact]
        public void NamePropertyIsTermsWhenMultipleTerms()
        {
            var filter = new TermFilter("field", 1, 2);

            Assert.Equal("terms", filter.Name);
            Assert.Equal(2, filter.Values.Count);
        }

        [Fact]
        public void NamePropertyIsTermsWhenListOfTerms()
        {
            var filter = TermFilter.FromIEnumerable("field", new List<int> { 1, 2, 3 }.OfType<object>());
            Assert.Equal("terms", filter.Name);
            Assert.Equal(3, filter.Values.Count);
        }

        [Fact]
        public void FilterValuesAreNormalized()
        {
            var filter = TermFilter.FromIEnumerable("field", new List<int> { 1, 2, 1, 1, 2, 9 }.OfType<object>());

            Assert.Equal(3, filter.Values.Count);
        }
    }
}