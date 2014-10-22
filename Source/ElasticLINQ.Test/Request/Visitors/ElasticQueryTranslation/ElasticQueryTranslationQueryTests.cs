// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

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
        public void StringContainsWithinQueryGeneratesQueryStringCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Query(e => e.Name.Contains(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest.Query;

            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria);
            Assert.Equal("prefix.name", queryStringCriteria.Fields.Single());
            Assert.Equal(String.Format("*{0}*", expectedConstant), queryStringCriteria.Value);
        }

        [Fact]
        public void StringStartsWithGeneratesQueryStringCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Query(e => e.Name.StartsWith(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest.Query;

            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria);
            Assert.Equal("prefix.name", queryStringCriteria.Fields.Single());
            Assert.Equal(String.Format("{0}*", expectedConstant), queryStringCriteria.Value);
        }

        [Fact]
        public void StringEndsWithGeneratesQueryStringCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Query(e => e.Name.EndsWith(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest.Query;

            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria);
            Assert.Equal("prefix.name", queryStringCriteria.Fields.Single());
            Assert.Equal(String.Format("*{0}", expectedConstant), queryStringCriteria.Value);
        }

        [Fact]
        public void QueryAndWhereGeneratesQueryAndFilterCriteria()
        {
            var query = Robots.Query(r => ElasticMethods.Regexp(r.Name, "r.*bot")).Where(r => r.Zone.HasValue);

            var searchRequest = ElasticQueryTranslator.Translate(Mapping, "prefix", query.Expression).SearchRequest;

            Assert.IsType<RegexpCriteria>(searchRequest.Query);
            Assert.IsType<ExistsCriteria>(searchRequest.Filter);
        }

        [Fact]
        public void QueryGeneratesQueryCriteria()
        {
            var where = Robots.Query(r => r.Name == "IG-88" && r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest;

            var andCriteria = Assert.IsType<AndCriteria>(request.Query);
            Assert.Null(request.Filter);
            Assert.Equal("and", andCriteria.Name);
            Assert.Single(andCriteria.Criteria, f => f.Name == "term");
            Assert.Single(andCriteria.Criteria, f => f.Name == "range");
            Assert.Equal(2, andCriteria.Criteria.Count);
        }

        [Fact]
        public void QueryStringGeneratesQueryStringCriteria()
        {
            const string expectedQueryStringValue = "Data";
            var where = Robots.QueryString(expectedQueryStringValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest;

            Assert.Null(criteria.Filter);
            Assert.NotNull(criteria.Query);
            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria.Query);
            Assert.Equal(expectedQueryStringValue, queryStringCriteria.Value);
        }

        [Fact]
        public void QueryStringWithFieldsGeneratesQueryStringCriteriaWithFields()
        {
            const string expectedQueryStringValue = "Data";
            var expectedFields = new[] {"Green", "Brown"};
            var where = Robots.QueryString(expectedQueryStringValue, expectedFields);
            var criteria = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest;

            Assert.Null(criteria.Filter);
            Assert.NotNull(criteria.Query);
            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria.Query);
            Assert.Equal(expectedQueryStringValue, queryStringCriteria.Value);
            Assert.Equal(expectedFields, queryStringCriteria.Fields);
        }

        [Fact]
        public void QueryStringWithQueryCombinesToAndQueryCriteria()
        {
            const string expectedQueryStringValue = "Data";
            var where = Robots.QueryString(expectedQueryStringValue).Query(q => q.Cost > 0);
            var request = ElasticQueryTranslator.Translate(Mapping, "prefix", where.Expression).SearchRequest;

            Assert.Null(request.Filter);
            Assert.NotNull(request.Query);
            var andCriteria = Assert.IsType<AndCriteria>(request.Query);
            Assert.Single(andCriteria.Criteria, a => a.Name == "query_string");
            Assert.Single(andCriteria.Criteria, a => a.Name == "range");
            Assert.Equal(2, andCriteria.Criteria.Count);
        }
    }
}