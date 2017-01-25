// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Test.TestSupport;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Formatters
{
    public class SearchRequestFormatterCriteriaTests
    {
        static readonly ElasticConnection defaultConnection = new ElasticConnection(new Uri("http://a.b.com:9000/"));
        static readonly MemberInfo memberInfo = typeof(string).GetProperty("Length");
        readonly IElasticMapping mapping = Substitute.For<IElasticMapping>();

        public SearchRequestFormatterCriteriaTests()
        {
            mapping.FormatValue(null, null)
                   .ReturnsForAnyArgs(callInfo => new JValue(string.Format("!!! {0} !!!", callInfo.Arg<object>(1))));
        }

        [Fact]
        public void BodyContainsFilterTerm()
        {
            var termCriteria = TermsCriteria.Build(TermsExecutionMode.@bool, "term1", memberInfo, "singlecriteria");

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "filtered", "filter", "term");
            Assert.Equal(1, result.Count());
            Assert.Equal("!!! singlecriteria !!!", result[termCriteria.Field].ToString());
            Assert.Null(result["execution"]);  // Only applicable to "terms" filters
        }

        [Fact]
        public void BodyContainsFilterTerms()
        {
            var termCriteria = TermsCriteria.Build("term1", memberInfo, "criteria1", "criteria2");

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "filtered", "filter", "terms");
            var actualTerms = result.TraverseWithAssert(termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains("!!! " + criteria + " !!!", actualTerms.Select(t => t.ToString()).ToArray());
            Assert.Null(result["execution"]);
        }

        [Fact]
        public void BodyContainsFilterTermsWithExecutionMode()
        {
            var termCriteria = TermsCriteria.Build(TermsExecutionMode.and, "term1", memberInfo, "criteria1", "criteria2");

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "filtered", "filter", "terms");
            var actualTerms = result.TraverseWithAssert(termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains("!!! " + criteria + " !!!", actualTerms.Select(t => t.ToString()).ToArray());
            var execution = (JValue)result.TraverseWithAssert("execution");
            Assert.Equal("and", execution.Value);
        }

        [Fact]
        public void BodyContainsFilterExists()
        {
            const string expectedFieldName = "fieldShouldExist";
            var existsCriteria = new ExistsCriteria(expectedFieldName);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = existsCriteria });
            var body = JObject.Parse(formatter.Body);

            var field = body.TraverseWithAssert("query", "filtered", "filter", "exists", "field");
            Assert.Equal(expectedFieldName, field);
        }

        [Fact]
        public void BodyContainsFilterMissing()
        {
            const string expectedFieldName = "fieldShouldBeMissing";
            var termCriteria = new MissingCriteria(expectedFieldName);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var field = body.TraverseWithAssert("query", "filtered", "filter", "missing", "field");
            Assert.Equal(expectedFieldName, field);
        }

        [Fact]
        public void BodyContainsFilterNot()
        {
            var termCriteria = TermsCriteria.Build("term1", memberInfo, "alpha", "bravo", "charlie", "delta", "echo");
            var notCriteria = NotCriteria.Create(termCriteria);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = notCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "filtered", "filter", "not", "terms");
            var actualTerms = result.TraverseWithAssert(termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains("!!! " + criteria + " !!!", actualTerms.Select(t => t.ToString()).ToArray());
        }

        [Fact]
        public void BodyContainsFilterOr()
        {
            var minCriteria = new RangeCriteria("minField", memberInfo, RangeComparison.GreaterThanOrEqual, 100);
            var maxCriteria = new RangeCriteria("maxField", memberInfo, RangeComparison.LessThan, 32768);
            var orCriteria = OrCriteria.Combine(minCriteria, maxCriteria);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = orCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "filtered", "filter", "or");
            Assert.Equal(2, result.Children().Count());
            foreach (var child in result)
                Assert.True(((JProperty)(child.First)).Name == "range");
        }

        [Fact]
        public void BodyContainsRangeFilter()
        {
            const string expectedField = "capacity";
            const decimal expectedRange = 2.0m;
            var rangeCriteria = new RangeCriteria(expectedField, memberInfo, RangeComparison.GreaterThanOrEqual, expectedRange);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = rangeCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "filtered", "filter", "range", expectedField, "gte");
            Assert.Equal("!!! 2.0 !!!", result);
        }

        [Fact]
        public void BodyContainsRegexFilter()
        {
            const string expectedField = "motor";
            const string expectedRegexp = "SR20DET";
            var regexpCriteria = new RegexpCriteria(expectedField, expectedRegexp);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = regexpCriteria });
            var body = JObject.Parse(formatter.Body);

            var actualRegexp = body.TraverseWithAssert("query", "filtered", "filter", "regexp", expectedField);
            Assert.Equal(expectedRegexp, actualRegexp);
        }

        [Fact]
        public void BodyContainsPrefixFilter()
        {
            const string expectedField = "motor";
            const string expectedPrefix = "SR20";
            var prefixCriteria = new PrefixCriteria(expectedField, expectedPrefix);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = prefixCriteria });
            var body = JObject.Parse(formatter.Body);

            var actualRegexp = body.TraverseWithAssert("query", "filtered", "filter", "prefix", expectedField);
            Assert.Equal(expectedPrefix, actualRegexp);
        }

        [Fact]
        public void BodyContainsMatchAllFilter()
        {
            var matchAllCriteria = MatchAllCriteria.Instance;

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = matchAllCriteria });
            var body = JObject.Parse(formatter.Body);

            body.TraverseWithAssert("query", "filtered", "filter", "match_all");
        }

        [Fact]
        public void BodyContainsFilterSingleCollapsedOr()
        {
            const string expectedFieldName = "fieldShouldExist";
            var existsCriteria = new ExistsCriteria(expectedFieldName);
            var orCriteria = OrCriteria.Combine(existsCriteria);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = orCriteria });
            var body = JObject.Parse(formatter.Body);

            var field = body.TraverseWithAssert("query", "filtered", "filter", "exists", "field");
            Assert.Equal(expectedFieldName, field);
        }

        [Fact]
        public void BodyContainsBoolMustQuery()
        {
            var rangeCriteria = new RangeCriteria("ranged", memberInfo, RangeComparison.GreaterThanOrEqual, 88);
            var boolMust = new BoolCriteria(new[] { rangeCriteria }, null, null);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = boolMust });
            var body = JObject.Parse(formatter.Body);

            var mustItems = body.TraverseWithAssert("query", "bool", "must");
            Assert.Equal(1, mustItems.Count());
            mustItems[0].TraverseWithAssert("range");
        }

        [Fact]
        public void BodyContainsBoolMustNotQuery()
        {
            var rangeCriteria = new RangeCriteria("ranged", memberInfo, RangeComparison.GreaterThanOrEqual, 88);
            var boolMust = new BoolCriteria(null, null, new[] { rangeCriteria });

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = boolMust });
            var body = JObject.Parse(formatter.Body);

            var mustNotItems = body.TraverseWithAssert("query", "bool", "must_not");
            Assert.Equal(1, mustNotItems.Count());
            mustNotItems[0].TraverseWithAssert("range");
        }

        [Fact]
        public void BodyContainsBoolShouldQuery()
        {
            var range1 = new RangeCriteria("range1", memberInfo, RangeComparison.GreaterThanOrEqual, 88);
            var range2 = new RangeCriteria("range2", memberInfo, RangeComparison.GreaterThanOrEqual, 88);
            var boolMust = new BoolCriteria(null, new[] { range1, range2 }, null);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = boolMust });
            var body = JObject.Parse(formatter.Body);

            var boolBody = body.TraverseWithAssert("query", "bool");

            var minMatch = boolBody.TraverseWithAssert("minimum_should_match");
            Assert.Equal(1, minMatch.Value<int>());

            var mustNotItems = boolBody.TraverseWithAssert("should");
            Assert.Equal(2, mustNotItems.Count());
            mustNotItems[0].TraverseWithAssert("range");
        }

        [Fact]
        public void ParseThrowsInvalidOperationForUnknownCriteriaTypes()
        {
            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Filter = new FakeCriteria() });
            Assert.Throws<InvalidOperationException>(() => JObject.Parse(formatter.Body));
        }

        class FakeCriteria : ICriteria
        {
            public string Name { get; private set; }
        }
    }
}