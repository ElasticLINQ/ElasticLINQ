// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Facets
{
    public class StatisticalFacetTests
    {
        private const string ExpectedName = "name";
        private readonly string[] expectedFields = { "field1", "field2", "field3" };
        private readonly ICriteria expectedFilter = new TermCriteria("field", "value1");

        [Fact]
        public void ConstructorSetsAllProperties()
        {
            var facet = new StatisticalFacet(ExpectedName, expectedFilter, expectedFields);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Same(expectedFilter, facet.Filter);
            Assert.Equal(expectedFields.Length, facet.Fields.Count);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionIfNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StatisticalFacet(null, expectedFilter, expectedFields));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionIfNameIsWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new StatisticalFacet(" ", expectedFilter, expectedFields));
        }

        [Fact]
        public void ConstructorAllowsNullFilter()
        {
            var facet = new StatisticalFacet(ExpectedName, null, expectedFields);

            Assert.Same(ExpectedName, facet.Name);
            Assert.Null(facet.Filter);
        }

        [Fact]
        public void ConstructorThrowsArgumentOutOfRangeIfNoExpectedFields()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StatisticalFacet(ExpectedName));
        }

        [Fact]
        public void TypePropertyIsAlwaysStatistical()
        {
            var facet = new StatisticalFacet(ExpectedName, expectedFields);

            Assert.Equal("statistical", facet.Type);
        }
    }
}