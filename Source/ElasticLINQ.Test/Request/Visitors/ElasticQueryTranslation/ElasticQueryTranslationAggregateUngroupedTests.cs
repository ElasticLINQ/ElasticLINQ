// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Materializers;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationAggregateUngroupedTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SumCreatesStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots, x => x.Sum(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(decimal), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            Assert.Null(translation.SearchRequest.Filter);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("cost", facet.Fields[0]);
        }

        [Fact]
        public void SumWithWhereCreatesFilterWithStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots.Where(r => r.Id > 10), x => x.Sum( r => r.EnergyUse));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(double), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var rangeFilter = Assert.IsType<RangeCriteria>(translation.SearchRequest.Filter);
            Assert.Equal("id", rangeFilter.Field);
            var rangeSpec = Assert.Single(rangeFilter.Specifications);
            Assert.Equal(RangeComparison.GreaterThan, rangeSpec.Comparison);
            Assert.Equal(10, rangeSpec.Value);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("energyUse", facet.Fields[0]);
        }

        [Fact]
        public void MinCreatesStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots, x => x.Min(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(decimal), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            Assert.Null(translation.SearchRequest.Filter);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("cost", facet.Fields[0]);
        }

        [Fact]
        public void MinWithWhereCreatesFilterWithStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots.Where(r => r.Id > 10), x => x.Min(r => r.EnergyUse));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(double), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var rangeFilter = Assert.IsType<RangeCriteria>(translation.SearchRequest.Filter);
            Assert.Equal("id", rangeFilter.Field);
            var rangeSpec = Assert.Single(rangeFilter.Specifications);
            Assert.Equal(RangeComparison.GreaterThan, rangeSpec.Comparison);
            Assert.Equal(10, rangeSpec.Value);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("energyUse", facet.Fields[0]);
        }

        public void MaxCreatesStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots, x => x.Max(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(decimal), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            Assert.Null(translation.SearchRequest.Filter);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("cost", facet.Fields[0]);
        }

        [Fact]
        public void MaxWithWhereCreatesFilterWithStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots.Where(r => r.Id > 10), x => x.Max(r => r.EnergyUse));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(double), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var rangeFilter = Assert.IsType<RangeCriteria>(translation.SearchRequest.Filter);
            Assert.Equal("id", rangeFilter.Field);
            var rangeSpec = Assert.Single(rangeFilter.Specifications);
            Assert.Equal(RangeComparison.GreaterThan, rangeSpec.Comparison);
            Assert.Equal(10, rangeSpec.Value);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("energyUse", facet.Fields[0]);
        }

        [Fact]
        public void AverageCreatesStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots, x => x.Average(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(decimal), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            Assert.Null(translation.SearchRequest.Filter);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("cost", facet.Fields[0]);
        }

        [Fact]
        public void AverWithWhereCreatesFilterWithStatisticalFacet()
        {
            var expression = MakeQueryableExpression(Robots.Where(r => r.Id > 10), x => x.Average(r => r.EnergyUse));

            var translation = ElasticQueryTranslator.Translate(Mapping, expression);

            Assert.Equal(typeof(double), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var rangeFilter = Assert.IsType<RangeCriteria>(translation.SearchRequest.Filter);
            Assert.Equal("id", rangeFilter.Field);
            var rangeSpec = Assert.Single(rangeFilter.Specifications);
            Assert.Equal(RangeComparison.GreaterThan, rangeSpec.Comparison);
            Assert.Equal(10, rangeSpec.Value);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("energyUse", facet.Fields[0]);
        }
    }
}