// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Materializers;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationGroupByConstantTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SelectSumCreatesStatisticalFacet()
        {
            var query = Robots.GroupBy(r => 1).Select(g => g.Sum(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, "p", query.Expression);

            Assert.Equal(typeof(decimal), Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("p.cost", facet.Fields[0]);
        }

        [Fact]
        public void SelectCountCreatesFilterFacetWithMatchAll()
        {
            var query = Robots.GroupBy(r => 1).Select(g => g.Count());

            var translation = ElasticQueryTranslator.Translate(Mapping, "", query.Expression);

            Assert.Equal(typeof(int), Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var facet = Assert.IsType<FilterFacet>(translation.SearchRequest.Facets[0]);
            Assert.IsType<MatchAllCriteria>(facet.Filter);
        }

        [Fact]
        public void SelectCountPredicateCreatesFilterFacet()
        {
            var query = Robots.GroupBy(r => 1).Select(g => g.Count(r => r.EnergyUse > 5.0));

            var translation = ElasticQueryTranslator.Translate(Mapping, "e", query.Expression);

            Assert.Equal(typeof(int), Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var facet = Assert.IsType<FilterFacet>(translation.SearchRequest.Facets[0]);
            var rangeCriteria = Assert.IsType<RangeCriteria>(facet.Filter);
            Assert.Equal("e.energyUse", rangeCriteria.Field);
            Assert.Equal(1, rangeCriteria.Specifications.Count);
            Assert.Equal(RangeComparison.GreaterThan, rangeCriteria.Specifications[0].Comparison);
            Assert.Equal(5.0, rangeCriteria.Specifications[0].Value);
        }

        [Fact]
        public void SelectLongCountCreatesFilterFacetWithMatchAll()
        {
            var query = Robots.GroupBy(r => 1).Select(g => g.LongCount());

            var translation = ElasticQueryTranslator.Translate(Mapping, "", query.Expression);

            Assert.Equal(typeof(long), Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var facet = Assert.IsType<FilterFacet>(translation.SearchRequest.Facets[0]);
            Assert.IsType<MatchAllCriteria>(facet.Filter);
        }

        [Fact]
        public void WhereWithSelectLongCountPredicateCreatesTwoFilters()
        {
            var grouped = Robots.Where(r => r.Zone == 1).GroupBy(r => 1).Select(g => g.LongCount(r => r.Cost == 2.0m));

            var translation = ElasticQueryTranslator.Translate(Mapping, "", grouped.Expression);

            Assert.Equal(typeof(long), Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            {
                var where = Assert.IsType<TermCriteria>(translation.SearchRequest.Filter);
                Assert.Equal("zone", where.Field);
                Assert.Equal(1, where.Value);
            }

            var facet = Assert.IsType<FilterFacet>(translation.SearchRequest.Facets[0]);
            var facetTerm = Assert.IsType<TermCriteria>(facet.Filter);
            Assert.Equal("cost", facetTerm.Field);
            Assert.Equal(2.0m, facetTerm.Value);
        }

        [Fact]
        public void SelectProjectionCreatesMultipleFacets()
        {
            var expectedFields = new[] { "cost", "energyUse", "started" };
            var query = Robots.GroupBy(r => 2m)
                .Select(g => new
                {
                    SumCost = g.Sum(a => a.Cost),
                    AverageEnergyUse = g.Average(a => a.EnergyUse),
                    MinStarted = g.Min(a => a.Started)
                });

            var translation = ElasticQueryTranslator.Translate(Mapping, "", query.Expression);

            Assert.Contains("AnonymousType", Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType.FullName);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(expectedFields.Length, translation.SearchRequest.Facets.Count);
            foreach (var expectedField in expectedFields)
            {
                var facet = translation.SearchRequest.Facets.OfType<StatisticalFacet>().Single(s => s.Fields.Contains(expectedField));
                Assert.Null(facet.Filter);
                Assert.Equal(1, facet.Fields.Count);
            }
        }

        [Fact]
        public void SelectProjectionCombinesSameFieldToSingleFacet()
        {
            var query = Robots.GroupBy(r => "a")
                .Select(g => new
                {
                    AverageEnergy = g.Average(a => a.EnergyUse),
                    MaxEnergy = g.Max(a => a.EnergyUse)
                });

            var translation = ElasticQueryTranslator.Translate(Mapping, "", query.Expression);

            Assert.Contains("AnonymousType", Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType.FullName);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);
            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("energyUse", facet.Fields[0]);
        }

        [Fact]
        public void SelectProjectionCanCreateMixedFacets()
        {
            var query = Robots.GroupBy(r => 2)
                .Select(g => new
                {
                    AverageEnergyUse = g.Average(a => a.EnergyUse),
                    CountHighEnergy = g.Count(a => a.EnergyUse > 50.0)
                });

            var translation = ElasticQueryTranslator.Translate(Mapping, "", query.Expression);

            Assert.Contains("AnonymousType", Assert.IsType<ListTermlessFacetsElasticMaterializer>(translation.Materializer).ElementType.FullName);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal(2, translation.SearchRequest.Facets.Count);

            {
                var termsStatsFacet = translation.SearchRequest.Facets.OfType<StatisticalFacet>().Single();
                Assert.Contains("energyUse", termsStatsFacet.Fields);
                Assert.Null(termsStatsFacet.Filter);
            }

            {
                var termsFacet = translation.SearchRequest.Facets.OfType<FilterFacet>().Single();
                var range = Assert.IsType<RangeCriteria>(termsFacet.Filter);
                Assert.Equal("energyUse", range.Field);
                Assert.Equal(1, range.Specifications.Count);
                var specification = range.Specifications[0];
                Assert.Equal(RangeComparison.GreaterThan, specification.Comparison);
                Assert.Equal(50.0, specification.Value);
            }
        }

        [Fact]
        public void SelectProjectionCanUseTupleCreate()
        {
            var query = Robots.GroupBy(g => 1).Select(g => Tuple.Create(g.Key, g.Count()));

            TestProjectionWithKeyAndCount(query.Expression);
        }

        [Fact]
        public void SelectProjectionCanUseAnonymousConstructor()
        {
            var query = Robots.GroupBy(g => 1).Select(g => new { g.Key, Count = g.Count() });

            TestProjectionWithKeyAndCount(query.Expression);
        }

        private static void TestProjectionWithKeyAndCount(Expression query)
        {
            var searchRequest = ElasticQueryTranslator.Translate(CouchMapping, "", query).SearchRequest;

            var facet = Assert.Single(searchRequest.Facets);
            var filterFacet = Assert.IsType<FilterFacet>(facet);
            Assert.Equal("GroupKey", filterFacet.Name);
            Assert.Same(MatchAllCriteria.Instance, filterFacet.Filter);
            Assert.IsType<ExistsCriteria>(searchRequest.Filter);
        }

        [Fact]
        public void SelectProjectionCanUseTupleCreateWithCountPredicate()
        {
            var query = Robots.GroupBy(g => 1)
                .Select(g => Tuple.Create(g.Key, g.Count(r => r.EnergyUse > 1.5), g.Count(r => r.EnergyUse <= 2.0)));

            TestProjectionWithCountPredicate(query.Expression);
        }

        [Fact]
        public void SelectProjectionCanUseAnonymousConstructorWithCountPredicate()
        {
            var query = Robots.GroupBy(g => 1)
                .Select(g => new { g.Key, High = g.Count(r => r.EnergyUse > 1.5), Low = g.Count(r => r.EnergyUse <= 2.0) });

            TestProjectionWithCountPredicate(query.Expression);
        }

        private static void TestProjectionWithCountPredicate(Expression query)
        {
            var searchRequest = ElasticQueryTranslator.Translate(Mapping, "", query).SearchRequest;

            Assert.All(searchRequest.Facets, f => Assert.IsType<FilterFacet>(f));
            var filterFacets = searchRequest.Facets.OfType<FilterFacet>().ToArray();
            Assert.Equal(2, searchRequest.Facets.Count);

            Assert.All(filterFacets, f => Assert.IsType<RangeCriteria>(f.Filter));
            Assert.All(filterFacets, f => Assert.Equal(((RangeCriteria)f.Filter).Field, "energyUse"));
            Assert.All(filterFacets, f => Assert.Equal(((RangeCriteria)f.Filter).Specifications.Count, 1));

            var specifications = filterFacets.SelectMany(f => ((RangeCriteria)f.Filter).Specifications).ToArray();

            Assert.Single(specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, 1.5));
            Assert.Single(specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, 2.0));
        }
    }
}