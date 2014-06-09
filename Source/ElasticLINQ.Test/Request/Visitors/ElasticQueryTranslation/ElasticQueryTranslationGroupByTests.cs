// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Materializers;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationGroupByTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void GroupByConstantSelectSumCreatesStatisticalFacet()
        {
            var query = Robots.GroupBy(r => 1).Select(g => g.Sum(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", query.Expression);

            var materializer = Assert.IsType<ElasticFacetsMaterializer>(translation.Materializer);
            Assert.Equal(typeof(decimal), materializer.ElementType);

            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("p.cost", facet.Fields[0]);
        }

        [Fact]
        public void GroupByConstantSelectCountPredicateCreatesFilterFacet()
        {
            const double expectedGreaterThanValue = 5;
            var query = Robots.GroupBy(r => 1).Select(g => g.Count(r => r.EnergyUse > expectedGreaterThanValue));

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", query.Expression);

            var materializer = Assert.IsType<ElasticFacetsMaterializer>(translation.Materializer);
            Assert.Equal(typeof(int), materializer.ElementType);

            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<FilterFacet>(translation.SearchRequest.Facets[0]);            
            var rangeCriteria = Assert.IsType<RangeCriteria>(facet.Filter);
            Assert.Equal("p.energyUse", rangeCriteria.Field);
            Assert.Equal(1, rangeCriteria.Specifications.Count);
            Assert.Equal(expectedGreaterThanValue, rangeCriteria.Specifications[0].Value);
            Assert.Equal(RangeComparison.GreaterThan, rangeCriteria.Specifications[0].Comparison);
        }

        [Fact]
        public void GroupByConstantWhereWithSelectLongCountPredicateCreatesTwoFilters()
        {
            const decimal expectedCost = 2.0m;
            const int expectedZone = 1;
            var grouped = Robots.Where(r => r.Zone == expectedZone).GroupBy(r => 1).Select(g => g.LongCount(r => r.Cost == expectedCost));

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", grouped.Expression);

            var materializer = Assert.IsType<ElasticFacetsMaterializer>(translation.Materializer);
            Assert.Equal(typeof(long), materializer.ElementType);

            var queryTerm = Assert.IsType<TermCriteria>(translation.SearchRequest.Filter);
            Assert.Equal("p.zone", queryTerm.Field);
            Assert.Equal(expectedZone, queryTerm.Value);

            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<FilterFacet>(translation.SearchRequest.Facets[0]);
            var facetTerm = Assert.IsType<TermCriteria>(facet.Filter);
            Assert.Equal("p.cost", facetTerm.Field);
            Assert.Equal(expectedCost, facetTerm.Value);
        }

        [Fact]
        public void GroupByFieldSelectSumCreatesTermsStatsFacet()
        {
            var query = Robots.GroupBy(r => r.Zone).Select(g => g.Sum(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", query.Expression);

            var materializer = Assert.IsType<ElasticFacetsMaterializer>(translation.Materializer);
            Assert.Equal(typeof(Decimal), materializer.ElementType);

            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<TermsStatsFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal("p.zone", facet.Key);
            Assert.Equal("p.cost", facet.Value);
        }

        [Fact]
        public void GroupByFieldSelectCoundPredicateCreatesTermsFacetWithFilter()
        {
            var query = Robots.GroupBy(r => r.Zone).Select(g => g.Count(r => r.Cost > 5m));

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", query.Expression);

            var materializer = Assert.IsType<ElasticFacetsMaterializer>(translation.Materializer);
            Assert.Equal(typeof(int), materializer.ElementType);

            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<TermsFacet>(translation.SearchRequest.Facets[0]);

            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("p.zone", facet.Fields[0]);

            var filter = Assert.IsType<RangeCriteria>(facet.Filter);
            Assert.Equal("p.cost", filter.Field);

            Assert.Equal(1, filter.Specifications.Count);
            var specification = filter.Specifications[0];
            Assert.Equal("gt", specification.Name);
            Assert.Equal(5m, specification.Value);
        }

        [Fact]
        public void GroupByFieldSelectAverageWithTakeCreatesTermsStatsFacetWithSize()
        {
            var query = Robots.GroupBy(r => r.Zone).Select(g => g.Average(r => r.EnergyUse)).Take(5);

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", query.Expression);

            var materializer = Assert.IsType<ElasticFacetsMaterializer>(translation.Materializer);
            Assert.Equal(typeof(Double), materializer.ElementType);

            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<TermsStatsFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal("p.zone", facet.Key);
            Assert.Equal("p.energyUse", facet.Value);
            Assert.Equal(5, facet.Size);
        }
    }
}
