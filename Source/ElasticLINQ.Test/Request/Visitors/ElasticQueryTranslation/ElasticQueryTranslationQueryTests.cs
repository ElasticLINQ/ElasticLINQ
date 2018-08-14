﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationQueryTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void QueryAndWhereGeneratesQueryAndFilterCriteria()
        {
            var query = Robots.Where(r => ElasticMethods.Regexp(r.Name, "r.*bot")).Where(r => r.Zone.HasValue);

            var searchRequest = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var andCriteria = Assert.IsType<AndCriteria>(searchRequest.Filter);
            Assert.IsType<RegexpCriteria>(andCriteria.Criteria[0]);
            Assert.IsType<ExistsCriteria>(andCriteria.Criteria[1]);
        }

        [Fact]
        public void QueryGeneratesQueryCriteria()
        {
            var where = Robots.Where(r => r.Name == "IG-88" && r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            var boolCriteria = Assert.IsType<BoolCriteria>(request.Filter);
            Assert.Single(boolCriteria.Must, f => f.Name == "term");
            Assert.Single(boolCriteria.Must, f => f.Name == "range");
            Assert.Equal(2, boolCriteria.Must.Count);
        }



        [Fact]
        public void QueryStringWithQueryCombinesToBoolQueryCriteria()
        {
            const string expectedQueryStringValue = "Data";
            var where = Robots.QueryString(expectedQueryStringValue).Where(q => q.Cost > 0);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.NotNull(request.Filter);
            var boolCriteria = Assert.IsType<BoolCriteria>(request.Filter);
            Assert.Single(boolCriteria.Must, a => a.Name == "query_string");
            Assert.Single(boolCriteria.Must, a => a.Name == "range");
            Assert.Equal(2, boolCriteria.Must.Count);
        }

        [Fact]
        public void AndOrQueryGeneratesBoolQueryWithAndArgs()
        {
            var query = Robots.Where(q => q.Cost > 0 && (q.EnergyUse > 0 || q.Started < DateTime.Now));

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<BoolCriteria>(request.Filter);
        }

        [Fact]
        public void BooleanTrueGeneratesMatchAllQuery()
        {
            var query = Robots.Where(q => true);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<MatchAllCriteria>(request.Filter);
        }

        [Fact]
        public void EvaluatedTrueGeneratesMatchAllQuery()
        {
            var query = Robots.Where(q => 1 == 1);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<MatchAllCriteria>(request.Filter);
        }

        [Fact]
        public void BooleanFalseGeneratesNotMatchAllQuery()
        {
            var query = Robots.Where(q => false);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var notCriteria = Assert.IsType<NotCriteria>(request.Filter);
            Assert.IsType<MatchAllCriteria>(notCriteria.Criteria);
        }

        [Fact]
        public void EvaluatedFalseGeneratesNotMatchAllQuery()
        {
            var query = Robots.Where(q => 1 > 1);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var notCriteria = Assert.IsType<NotCriteria>(request.Filter);
            Assert.IsType<MatchAllCriteria>(notCriteria.Criteria);
        }

        [Fact]
        public void BooleanConstantsGeneratesBoolMatchAllQueryAndNotMatchAllQuery()
        {
            var query = Robots.Where(r => (r.Id > 1 && true) || (r.Id < 1 && false));

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var boolCriteria = Assert.IsType<BoolCriteria>(request.Filter);
            Assert.Empty(boolCriteria.Must);
            Assert.Empty(boolCriteria.MustNot);
            Assert.Equal(2, boolCriteria.Should.Count);

            Assert.Single(boolCriteria.Should, c => c is BoolCriteria && ((BoolCriteria)c).Must.Any(s => s is MatchAllCriteria));
            Assert.Single(boolCriteria.Should, c => c is BoolCriteria && ((BoolCriteria)c).Must.Any(s => s is NotCriteria));
        }
    }
}