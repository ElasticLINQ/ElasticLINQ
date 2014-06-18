// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Test.TestSupport;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Formatters
{
    public class PostBodyRequestFormatterFacetTests
    {
        private static readonly ElasticConnection defaultConnection = new ElasticConnection(new Uri("http://a.b.com:9000/"));
        private readonly IElasticMapping mapping = Substitute.For<IElasticMapping>();

        [Fact]
        public void BodyContainsStatisticalFacet()
        {
            var expectedFacet = new StatisticalFacet("TotalSales", "OrderTotal");
            var searchRequest = new ElasticSearchRequest { Facets = new List<IFacet>(new [] { expectedFacet }) };

            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("facets", expectedFacet.Name, expectedFacet.Type, "field");
            Assert.Equal(expectedFacet.Fields[0], result.ToString());
        }

        [Fact]
        public void BodyContainsFilterFacet()
        {
            var expectedFilter = new ExistsCriteria("IsLocal");
            var expectedFacet = new FilterFacet("LocalSales", expectedFilter);
            var searchRequest = new ElasticSearchRequest { Facets = new List<IFacet>(new[] { expectedFacet }) };

            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("facets", expectedFacet.Name, expectedFacet.Type, expectedFilter.Name, "field");
            Assert.Equal(expectedFilter.Field, result.ToString());
        }

        [Fact]
        public void BodyContainsTermsFacet()
        {
            var expectedFacet = new TermsFacet("Totals", "OrderTotal", "OrderCost");
            var searchRequest = new ElasticSearchRequest { Facets = new List<IFacet>(new[] { expectedFacet }) };

            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var actualFields = body.TraverseWithAssert("facets", expectedFacet.Name, expectedFacet.Type, "fields").ToArray();

            foreach(var expectedField in expectedFacet.Fields)
                Assert.Contains(expectedField, actualFields);
        }

        [Fact]
        public void BodyContainsTermsStatsFacet()
        {
            const int expectedSize = 101;
            var expectedFacet = new TermsStatsFacet("Name", "Key", "Value", expectedSize);
            var searchRequest = new ElasticSearchRequest { Facets = new List<IFacet>(new[] { expectedFacet }) };

            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("facets", expectedFacet.Name, expectedFacet.Type);
            Assert.Equal(expectedFacet.Key, result.TraverseWithAssert("key_field").ToString());
            Assert.Equal(expectedFacet.Value, result.TraverseWithAssert("value_field").ToString());
            Assert.Equal(expectedSize.ToString(CultureInfo.InvariantCulture), result.TraverseWithAssert("size").ToString());
        }

        [Fact]
        public void BodyContainsMultipleFacets()
        {
            var expectedFacets = new List<IFacet>
            {
                new FilterFacet("LocalSales", new ExistsCriteria("IsLocal")),
                new StatisticalFacet("TotalSales", "OrderTotal")
            };

            var searchRequest = new ElasticSearchRequest { Facets = expectedFacets };

            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var facetResults = body.TraverseWithAssert("facets");
            foreach (var expectedFacet in expectedFacets)
                facetResults.TraverseWithAssert(expectedFacet.Name, expectedFacet.Type);
        }

        [Fact]
        public void BodyContainsFilterFacetAndedWithRequestFilter()
        {
            var expectedFacet = new FilterFacet("LocalSales", new ExistsCriteria("IsLocal"));
            var searchRequest = new ElasticSearchRequest
            {
                Filter = new MissingCriteria("Country"),
                Query = new PrefixCriteria("Field", "Prefix"),
                Facets = new List<IFacet>(new[] { expectedFacet })
            };

            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var andFilter = body.TraverseWithAssert("facets", expectedFacet.Name, expectedFacet.Type, "and");
            Assert.Equal(2, andFilter.Count());
        }

        [Fact]
        public void ParseThrowsInvalidOperationForUnknownCriteriaTypes()
        {
            var facets = new List<IFacet> { new FakeFacet() };
            var formatter = new PostBodyRequestFormatter(defaultConnection, mapping, new ElasticSearchRequest { DocumentType = "type1", Facets = facets });
            Assert.Throws<InvalidOperationException>(() => JObject.Parse(formatter.Body));
        }

        class FakeFacet : IFacet
        {
            public string Name { get; private set; }
            public string Type { get; private set; }
            public ICriteria Filter { get; private set; }
        }
    }
}