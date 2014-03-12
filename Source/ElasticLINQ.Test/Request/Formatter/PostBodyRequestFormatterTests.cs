// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Formatter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Formatter
{
    public class PostBodyRequestFormatterTests
    {
        private static readonly ElasticConnection defaultConnection = new ElasticConnection(new Uri("http://a.b.com:9000/"));

        [Fact]
        public void UrlPathContainsTypeSpecifier()
        {
            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1" });

            Assert.Contains("type1", formatter.Uri.AbsolutePath);
        }

        [Fact]
        public void UrlPathContainsIndexSpecifier()
        {
            const string expectedIndex = "myIndex";
            var indexConnection = new ElasticConnection(defaultConnection.Endpoint) { Index = expectedIndex };
            var formatter = new PostBodyRequestFormatter(indexConnection, new ElasticSearchRequest { Type = "type1" });

            Assert.Contains(expectedIndex, formatter.Uri.AbsolutePath);
        }

        [Fact]
        public void ParseThrowsInvalidOperationForUnknownCriteriaTypes()
        {
            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Query = new FakeCriteria() });
            Assert.Throws<InvalidOperationException>(() => JObject.Parse(formatter.Body));
        }

        [Fact]
        public void BodyIsValidJsonFormattedResponse()
        {
            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1" });

            Assert.DoesNotThrow(() => JObject.Parse(formatter.Body));
        }

        [Fact]
        public void BodyContainsFilterTerm()
        {
            var termCriteria = TermsCriteria.Build(TermsExecutionMode.@bool, "term1", "singlecriteria");

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "term");
            Assert.Equal(1, result.Count());
            Assert.Equal(termCriteria.Values[0], result[termCriteria.Field].ToString());
            Assert.Null(result["execution"]);  // Only applicable to "terms" filters
        }

        [Fact]
        public void BodyContainsFilterTerms()
        {
            var termCriteria = TermsCriteria.Build("term1", "criteria1", "criteria2");

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "terms");
            var actualTerms = TraverseWithAssert(result, termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains(criteria, actualTerms.Select(t => t.ToString()).ToArray());
            Assert.Null(result["execution"]);
        }

        [Fact]
        public void BodyContainsFilterTermsWithExecutionMode()
        {
            var termCriteria = TermsCriteria.Build(TermsExecutionMode.and, "term1", "criteria1", "criteria2");

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "terms");
            var actualTerms = TraverseWithAssert(result, termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains(criteria, actualTerms.Select(t => t.ToString()).ToArray());
            var execution = (JValue)TraverseWithAssert(result, "execution");
            Assert.Equal("and", execution.Value);
        }

        [Fact]
        public void BodyContainsFilterExists()
        {
            const string expectedFieldName = "fieldShouldExist";
            var existsCriteria = new ExistsCriteria(expectedFieldName);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = existsCriteria });
            var body = JObject.Parse(formatter.Body);

            var field = TraverseWithAssert(body, "filter", "exists", "field");
            Assert.Equal(expectedFieldName, field);
        }

        [Fact]
        public void BodyContainsFilterMissing()
        {
            const string expectedFieldName = "fieldShouldBeMissing";
            var termCriteria = new MissingCriteria(expectedFieldName);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = termCriteria });
            var body = JObject.Parse(formatter.Body);

            var field = TraverseWithAssert(body, "filter", "missing", "field");
            Assert.Equal(expectedFieldName, field);
        }

        [Fact]
        public void BodyContainsFilterNot()
        {
            var termCriteria = TermsCriteria.Build("term1", "alpha", "bravo", "charlie", "delta", "echo");
            var notCriteria = NotCriteria.Create(termCriteria);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = notCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "not", "terms");
            var actualTerms = TraverseWithAssert(result, termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains(criteria, actualTerms.Select(t => t.ToString()).ToArray());
        }

        [Fact]
        public void BodyContainsFilterOr()
        {
            var minCriteria = new RangeCriteria("minField", RangeComparison.GreaterThanOrEqual, 100);
            var maxCriteria = new RangeCriteria("maxField", RangeComparison.LessThan, 32768);
            var orCriteria = OrCriteria.Combine(minCriteria, maxCriteria);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = orCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "or");
            Assert.Equal(2, result.Children().Count());
            foreach (var child in result)
                Assert.True(((JProperty)(child.First)).Name == "range");
        }

        [Fact]
        public void BodyContainsRangeFilter()
        {
            const string expectedField = "capacity";
            const decimal expectedRange = 2.0m;
            var rangeCriteria = new RangeCriteria(expectedField, RangeComparison.GreaterThanOrEqual, expectedRange);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = rangeCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "range", expectedField, "gte");
            Assert.Equal(expectedRange, result);
        }

        [Fact]
        public void BodyContainsRegexFilter()
        {
            const string expectedField = "motor";
            const string expectedRegexp = "SR20DET";
            var regexpCriteria = new RegexpCriteria(expectedField, expectedRegexp);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = regexpCriteria });
            var body = JObject.Parse(formatter.Body);

            var actualRegexp = TraverseWithAssert(body, "filter", "regexp", expectedField);
            Assert.Equal(expectedRegexp, actualRegexp);
        }

        [Fact]
        public void BodyContainsPrefixFilter()
        {
            const string expectedField = "motor";
            const string expectedPrefix = "SR20";
            var prefixCriteria = new PrefixCriteria(expectedField, expectedPrefix);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = prefixCriteria });
            var body = JObject.Parse(formatter.Body);

            var actualRegexp = TraverseWithAssert(body, "filter", "prefix", expectedField);
            Assert.Equal(expectedPrefix, actualRegexp);
        }

        [Fact]
        public void BodyContainsFilterSingleCollapsedOr()
        {
            const string expectedFieldName = "fieldShouldExist";
            var existsCriteria = new ExistsCriteria(expectedFieldName);
            var orCriteria = OrCriteria.Combine(existsCriteria);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Filter = orCriteria });
            var body = JObject.Parse(formatter.Body);

            var field = TraverseWithAssert(body, "filter", "exists", "field");
            Assert.Equal(expectedFieldName, field);
        }

        [Fact]
        public void BodyContainsQueryString()
        {
            const string expectedQuery = "this is my query string";
            var queryString = new QueryStringCriteria(expectedQuery);

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Query = queryString });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "query", "query_string", "query");
            Assert.Equal(expectedQuery, result.ToString());
        }

        [Fact]
        public void BodyContainsQueryRange()
        {
            var rangeCriteria = new RangeCriteria("someField",
                new[]
                {
                    new RangeSpecificationCriteria(RangeComparison.LessThan, 100),
                    new RangeSpecificationCriteria(RangeComparison.GreaterThan, 200)
                });

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Query = rangeCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "query", "range");
            var actualRange = TraverseWithAssert(result, rangeCriteria.Field);
            Assert.Equal(100, actualRange["lt"]);
            Assert.Equal(200, actualRange["gt"]);
        }

        [Fact]
        public void BodyContainsSortOptions()
        {
            var expectedSortOptions = new List<SortOption> { new SortOption("first", true), new SortOption("second", false), new SortOption("third", false, true) };

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", SortOptions = expectedSortOptions });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "sort");
            for (var i = 0; i < expectedSortOptions.Count; i++)
            {
                var actualSort = result[i];
                var desiredSort = expectedSortOptions[i];
                if (desiredSort.IgnoreUnmapped)
                {
                    var first = (JProperty)actualSort.First;
                    Assert.Equal(desiredSort.Name, first.Name);
                    var childProperties = first.First.Children().Cast<JProperty>().ToArray();
                    Assert.Single(childProperties, f => f.Name == "ignore_unmapped" && f.Value.ToObject<bool>());
                    Assert.Single(childProperties, f => f.Name == "order" && f.Value.ToObject<string>() == "desc");
                }
                else
                {
                    if (desiredSort.Ascending)
                    {
                        Assert.Equal(desiredSort.Name, actualSort);
                    }
                    else
                    {
                        var finalActualSort = actualSort[desiredSort.Name];
                        Assert.NotNull(finalActualSort);
                        Assert.Equal("desc", finalActualSort.ToString());
                    }
                }
            }
        }

        [Fact]
        public void BodyContainsFieldSelections()
        {
            var expectedFields = new List<string> { "first", "second", "third" };

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Fields = expectedFields });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "fields");
            foreach (var field in expectedFields)
                Assert.Contains(field, result);
        }

        [Fact]
        public void BodyContainsFromWhenSpecified()
        {
            const int expectedFrom = 1024;

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", From = expectedFrom });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "from");
            Assert.Equal(expectedFrom, result);
        }

        [Fact]
        public void BodyContainsSizeWhenSpecified()
        {
            const int expectedSize = 4096;

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest { Type = "type1", Size = expectedSize });
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "size");
            Assert.Equal(expectedSize, result);
        }

        [Fact]
        public static void BodyContainsTimeoutWhenSpecified()
        {
            const string expectedTimeout = "15s";
            var connection = new ElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.FromSeconds(15));

            var formatter = new PostBodyRequestFormatter(connection, new ElasticSearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "timeout");
            Assert.Equal(expectedTimeout, result);
        }

        [Fact]
        public static void BodyDoesNotContainTimeoutWhenZero()
        {
            var connection = new ElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.Zero);

            var formatter = new PostBodyRequestFormatter(connection, new ElasticSearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = body["timeout"];
            Assert.Null(result);
        }

        private static JToken TraverseWithAssert(JToken token, params string[] paths)
        {
            foreach (var path in paths)
            {
                Assert.NotNull(token);
                token = token[path];
            }

            Assert.NotNull(token);
            return token;
        }

        class FakeCriteria : ICriteria
        {
            public string Name { get; private set; }
        }
    }
}