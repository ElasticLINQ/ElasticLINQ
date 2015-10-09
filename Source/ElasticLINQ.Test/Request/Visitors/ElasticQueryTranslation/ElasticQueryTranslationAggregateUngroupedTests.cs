// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using ElasticLinq.Request.Facets;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Materializers;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationAggregateUngroupedTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SumCreatesStatisticalFacet()
        {           
            var queryExpression = CaptureExpression(() => Robots.Sum(r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, queryExpression);

            Assert.Equal(typeof(decimal), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("cost", facet.Fields[0]);
        }

        [Fact]
        public void WhereWithSumCreatesStatisticalFacetWithQueryFilter()
        {
            var queryExpression = CaptureExpression(() => Robots.Where(r => r.Id > 10).Sum( r => r.Cost));

            var translation = ElasticQueryTranslator.Translate(Mapping, queryExpression);

            Assert.Equal(typeof(decimal), Assert.IsType<TermlessFacetElasticMaterializer>(translation.Materializer).ElementType);
            Assert.Equal("count", translation.SearchRequest.SearchType);
            Assert.Equal("robots", translation.SearchRequest.DocumentType);
            Assert.Equal(1, translation.SearchRequest.Facets.Count);

            var facet = Assert.IsType<StatisticalFacet>(translation.SearchRequest.Facets[0]);
            Assert.Null(facet.Filter);
            Assert.Equal(1, facet.Fields.Count);
            Assert.Equal("cost", facet.Fields[0]);
        }
   }
}