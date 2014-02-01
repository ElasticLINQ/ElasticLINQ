// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Facets
{
    public class TermsFacetTests
    {
        private const string ExpectedName = "name";
        private readonly string[] expectedFields = { "field1", "field2", "field3" };
        private readonly ICriteria expectedFilter = new TermCriteria("field", "value1");
        private readonly int? expectedSize = 123;

        [Fact]
        public void ConstructorSetsAllProperties()
        {
            var facet = new TermsFacet(ExpectedName, expectedFilter, expectedSize, expectedFields);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Same(expectedFilter, facet.Filter);
            Assert.Equal(expectedSize, facet.Size);
            Assert.Equal(expectedFields.Length, facet.Fields.Count);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionIfNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TermsFacet(null, expectedFilter, expectedSize, expectedFields));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionIfNameIsWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new TermsFacet(" ", expectedFilter, expectedSize, expectedFields));
        }

        [Fact]
        public void ConstructorAllowsNullFilter()
        {
            var facet = new TermsFacet(ExpectedName, null,  expectedSize, expectedFields);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Null(facet.Filter);
        }

        [Fact]
        public void ConstructorAllowsNullSize()
        {
            var facet = new TermsFacet(ExpectedName, expectedFilter, null, expectedFields);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Null(facet.Size);
        }

        [Fact]
        public void ConstructorThrowsArgumentOutOfRangeIfNoExpectedFields()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TermsFacet(ExpectedName));
        }

        [Fact]
        public void TypePropertyIsAlwaysStatistical()
        {
            var facet = new TermsFacet(ExpectedName, expectedFields);

            Assert.Equal("terms", facet.Type);
        }
    }
}