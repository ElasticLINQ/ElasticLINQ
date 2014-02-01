// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Facets
{
    public class FilterFacetTests
    {
        private const string ExpectedName = "name";
        private readonly ICriteria expectedFilter = new TermCriteria("field", "value1");

        [Fact]
        public void ConstructorSetsAllProperties()
        {
            var facet = new FilterFacet(ExpectedName, expectedFilter);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Same(expectedFilter, facet.Filter);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionIfNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FilterFacet(null, expectedFilter));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionIfNameIsWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new FilterFacet(" ", expectedFilter));
        }

        [Fact]
        public void ConstructorAllowsNullFilter()
        {
            var facet = new FilterFacet(ExpectedName, null);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Null(facet.Filter);
        }

        [Fact]
        public void TypePropertyIsAlwaysFilter()
        {
            var facet = new FilterFacet(ExpectedName, null);

            Assert.Equal("filter", facet.Type);
        }
    }
}