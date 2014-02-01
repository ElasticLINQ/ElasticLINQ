// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Facets
{
    public class TermsStatsFacetTests
    {
        private const string ExpectedName = "name";
        private const string ExpectedKey = "key";
        private const string ExpectedValue = "value";
        private readonly ICriteria expectedFilter = new TermCriteria("field", "value1");
        private readonly int? expectedSize = 123;

        [Fact]
        public void ConstructorWithCriteriaSetsAllProperties()
        {
            var facet = new TermsStatsFacet(ExpectedName, expectedFilter, ExpectedKey, ExpectedValue);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Same(expectedFilter, facet.Filter);
            Assert.Same(ExpectedKey, facet.Key);
            Assert.Same(ExpectedValue, facet.Value);
            Assert.Null(facet.Size);
        }

        [Fact]
        public void ConstructorWithSizeSetsAllProperties()
        {
            var facet = new TermsStatsFacet(ExpectedName, ExpectedKey, ExpectedValue, expectedSize);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Same(ExpectedKey, facet.Key);
            Assert.Same(ExpectedValue, facet.Value);
            Assert.Equal(expectedSize, facet.Size);
            Assert.Null(facet.Filter);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionIfNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TermsStatsFacet(null, expectedFilter, ExpectedKey, ExpectedValue));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionIfNameIsWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new TermsStatsFacet(" ", expectedFilter, ExpectedKey, ExpectedValue));
        }

        [Fact]
        public void ConstructorAllowsNullFilter()
        {
            var facet = new TermsStatsFacet(ExpectedName, null, ExpectedKey, ExpectedValue);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Null(facet.Filter);
        }

        [Fact]
        public void ConstructorAllowsNullSize()
        {
            var facet = new TermsStatsFacet(ExpectedName, ExpectedKey, ExpectedValue, null);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Null(facet.Size);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullIfKeyNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TermsStatsFacet(ExpectedName, null, ExpectedValue, 1));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullIfValueNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TermsStatsFacet(ExpectedName, ExpectedKey, null, 1));
        }

        [Fact]
        public void TypePropertyIsAlwaysStatistical()
        {
            var facet = new TermsFacet(ExpectedName, ExpectedValue);

            Assert.Equal("terms", facet.Type);
        }
    }
}