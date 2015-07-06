// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SearchRequestTypeIsSetFromType()
        {
            var actual = Mapping.GetDocumentType(typeof(Robot));

            var translation = ElasticQueryTranslator.Translate(Mapping, Robots.Expression);

            Assert.Equal(actual, translation.SearchRequest.DocumentType);
        }

        [Fact]
        public void TypeExistsCriteriaIsAddedWhenNoOtherCriteria()
        {
            var translation = ElasticQueryTranslator.Translate(CouchMapping, Robots.Expression);

            Assert.IsType<ExistsCriteria>(translation.SearchRequest.Filter);
        }

        [Fact]
        public void TypeExistsCriteriaIsAppliedWhenFilterIsMissingCriteria()
        {
            var query = Robots.Where(r => r.Name == null);
            var translation = ElasticQueryTranslator.Translate(CouchMapping, query.Expression);

            var andCriteria = Assert.IsType<AndCriteria>(translation.SearchRequest.Filter);
            Assert.Equal(2, andCriteria.Criteria.Count);
            Assert.Single(andCriteria.Criteria, c => c is ExistsCriteria);
            Assert.Single(andCriteria.Criteria, c => c is MissingCriteria);
        }

        [Fact]
        public void TypeExistsCriteriaIsAppliedWhenFilterIsAndCriteria()
        {
            var query = Robots.Where(r => r.Name == "a" && r.Cost > 1);
            var translation = ElasticQueryTranslator.Translate(CouchMapping, query.Expression);

            var andCriteria = Assert.IsType<AndCriteria>(translation.SearchRequest.Filter);
            Assert.Equal(3, andCriteria.Criteria.Count);
            Assert.Single(andCriteria.Criteria, c => c is TermCriteria);
            Assert.Single(andCriteria.Criteria, c => c is RangeCriteria);
            Assert.Single(andCriteria.Criteria, c => c is ExistsCriteria);
        }

        [Fact]
        public void TypeExistsCriteriaIsAppliedWhenFilterIsOrCriteria()
        {
            var query = Robots.Where(r => r.Name == "a" || r.Cost > 1);
            var translation = ElasticQueryTranslator.Translate(CouchMapping, query.Expression);

            var andCriteria = Assert.IsType<AndCriteria>(translation.SearchRequest.Filter);
            Assert.Equal(2, andCriteria.Criteria.Count);
            Assert.Single(andCriteria.Criteria, c => c is OrCriteria);
            Assert.Single(andCriteria.Criteria, c => c is ExistsCriteria);
        }

        [Fact]
        public void FilterIsWipedWhenConstantTrue()
        {
            var query = Robots.Where(r => true);
            var translation = ElasticQueryTranslator.Translate(Mapping, query.Expression);

            Assert.Null(translation.SearchRequest.Filter);
        }

        [Fact]
        public void TypeExistsCriteriaIsAppliedWhenFilterIsConstantTrue()
        {
            var query = Robots.Where(r => true);
            var translation = ElasticQueryTranslator.Translate(CouchMapping, query.Expression);

            Assert.IsType<ExistsCriteria>(translation.SearchRequest.Filter);
        }

        [Fact]
        public void SkipTranslatesToFrom()
        {
            const int actual = 325;

            var skipped = Robots.Skip(actual);
            var translation = ElasticQueryTranslator.Translate(Mapping, skipped.Expression);

            Assert.Equal(actual, translation.SearchRequest.From);
        }

        [Fact]
        public void TakeTranslatesToSize()
        {
            const int actual = 73;

            var taken = Robots.Take(actual);
            var translation = ElasticQueryTranslator.Translate(Mapping, taken.Expression);

            Assert.Equal(actual, translation.SearchRequest.Size);
        }

        [Theory]
        [InlineData(42, 2112)]
        [InlineData(2112, 42)]
        public void TakeTranslatesToSmallestSize(int size1, int size2)
        {
            var expectedSize = Math.Min(size1, size2);

            var taken = Robots.Take(size1).Take(size2);
            var translation = ElasticQueryTranslator.Translate(Mapping, taken.Expression);

            Assert.Equal(expectedSize, translation.SearchRequest.Size);
        }

        [Fact]
        public void SimpleSelectProducesValidMaterializer()
        {
            var translation = ElasticQueryTranslator.Translate(Mapping, Robots.Expression);
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>() } };

            Assert.NotNull(translation.Materializer);
            var materialized = translation.Materializer.Materialize(response);
            Assert.IsAssignableFrom<IEnumerable<Robot>>(materialized);
            Assert.Empty((IEnumerable<Robot>)materialized);
        }

        [Fact]
        public void MinScoreCreatesRequestWithMinScore()
        {
            const double expectedScore = 123.4;

            var query = Robots.Query(q => q.Name.Contains("a")).MinScore(expectedScore);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.Null(request.Filter);
            Assert.NotNull(request.Query);
            Assert.Equal(expectedScore, request.MinScore);
        }
    }
}