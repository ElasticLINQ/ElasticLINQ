// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Test.TestSupport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ElasticLinq.Test.Request.Formatters
{
    public class SearchRequestFormatterTests
    {
        static readonly ElasticConnection defaultConnection = new ElasticConnection(new Uri("http://a.b.com:9000/"));
        static readonly MemberInfo memberInfo = typeof(string).GetProperty("Length");
        static readonly ICriteria criteria = new ExistsCriteria("greenField");
        readonly IElasticMapping mapping = Substitute.For<IElasticMapping>();

        public SearchRequestFormatterTests()
        {
            mapping.FormatValue(null, null)
                   .ReturnsForAnyArgs(callInfo => new JValue(string.Format("!!! {0} !!!", callInfo.Arg<object>(1))));
        }

        [Fact]
        public void BodyIsValidJsonFormattedResponse()
        {
            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1" });

            JObject.Parse(formatter.Body);
        }

        [Fact]
        public void BodyContainsQueryString()
        {
            const string expectedQuery = "this is my query string";
            var queryString = new QueryStringCriteria(expectedQuery);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Query = queryString });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "query_string", "query");
            Assert.Equal(expectedQuery, result.ToString());
        }

        [Fact]
        public void BodyContainsQueryStringWithFields()
        {
            const string expectedQuery = "this is my query string";
            var expectedFields = new[] { "green", "brown", "yellow" };
            var queryString = new QueryStringCriteria(expectedQuery, expectedFields);

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Query = queryString });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "query_string");
            Assert.Equal(expectedQuery, result.TraverseWithAssert("query").ToString());
            Assert.Equal(expectedFields, result.TraverseWithAssert("fields").Select(f => f.ToString()).ToArray());
        }

        [Fact]
        public void BodyContainsQueryRange()
        {
            var rangeCriteria = new RangeCriteria("someField", memberInfo,
                new[]
                {
                    new RangeSpecificationCriteria(RangeComparison.LessThan, 100),
                    new RangeSpecificationCriteria(RangeComparison.GreaterThan, 200)
                });

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Query = rangeCriteria });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("query", "range");
            var actualRange = result.TraverseWithAssert(rangeCriteria.Field);
            Assert.Equal("!!! 100 !!!", actualRange["lt"]);
            Assert.Equal("!!! 200 !!!", actualRange["gt"]);
        }

        [Fact]
        public void BodyContainsSortOptions()
        {
            var expectedSortOptions = new List<SortOption> { new SortOption("first", true), new SortOption("second", false), new SortOption("third", false, true) };

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", SortOptions = expectedSortOptions });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("sort");
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

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Fields = expectedFields });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("fields");
            foreach (var field in expectedFields)
                Assert.Contains(field, result);
        }

        [Fact]
        public void PrettyCreatesBodyThatIsOnlyFormattedNicely()
        {
            var plainBody = new SearchRequestFormatter(
                new ElasticConnection(defaultConnection.Endpoint, options: new ElasticConnectionOptions { Pretty = false }),
                mapping, new SearchRequest { DocumentType = "type1", Filter = criteria }).Body;

            var prettyBody = new SearchRequestFormatter(
                new ElasticConnection(defaultConnection.Endpoint, options: new ElasticConnectionOptions { Pretty = true }),
                mapping, new SearchRequest { DocumentType = "type1", Filter = criteria }).Body;

            Assert.NotSame(plainBody, prettyBody);

            var unformattedPrettyBody = JObject.Parse(prettyBody).ToString(Formatting.None);
            Assert.Equal(plainBody, unformattedPrettyBody);
        }

        [Fact]
        public void SearchSizeDefaultIsUsedWhenNoSearchRequestSizeSpecified()
        {
            const int expectedSize = 1234;
            var connectionOptions = new ElasticConnectionOptions { SearchSizeDefault = expectedSize };
            var connection = new ElasticConnection(defaultConnection.Endpoint, options: connectionOptions);
            var searchRequest = new SearchRequest { DocumentType = "type1", Filter = criteria };

            var formatter = new SearchRequestFormatter(connection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            Assert.Equal(expectedSize, body.TraverseWithAssert("size").Value<long>());
        }

        [Fact]
        public void SearchSizeDefaultIsNotUsedWhenSearchRequestSizeSpecified()
        {
            const long expectedSize = 54321;
            var connectionOptions = new ElasticConnectionOptions { SearchSizeDefault = 111222 };
            var connection = new ElasticConnection(defaultConnection.Endpoint, options: connectionOptions);
            var searchRequest = new SearchRequest { DocumentType = "type1", Filter = criteria, Size = expectedSize};

            var formatter = new SearchRequestFormatter(connection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            Assert.Equal(expectedSize, body.TraverseWithAssert("size").Value<long>());
        }

        [Fact]
        public void BodyContainsFromWhenSpecified()
        {
            const int expectedFrom = 1024;

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", From = expectedFrom });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("from");
            Assert.Equal(expectedFrom, result);
        }

        [Fact]
        public void BodyContainsSizeWhenSpecified()
        {
            const int expectedSize = 4096;

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1", Size = expectedSize });
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("size");
            Assert.Equal(expectedSize, result);
        }

        [Fact]
        public void BodyContainsTimeoutWhenSpecified()
        {
            const string expectedTimeout = "15s";
            var connection = new ElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.FromSeconds(15));

            var formatter = new SearchRequestFormatter(connection, mapping, new SearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("timeout");
            Assert.Equal(expectedTimeout, result);
        }

        [Fact]
        public void BodyDoesNotContainTimeoutWhenZero()
        {
            var connection = new ElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.Zero);

            var formatter = new SearchRequestFormatter(connection, mapping, new SearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = body["timeout"];
            Assert.Null(result);
        }

        [Fact]
        public void BodyContainsMinScoreWhenSpecified()
        {
            var searchRequest = new SearchRequest { MinScore = 1.3 };

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, searchRequest);
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("min_score");
            Assert.Equal(searchRequest.MinScore.ToString(), result);
        }

        [Fact]
        public void BodyDoesNotContainMinScoreWhenUnspecified()
        {
            var connection = new ElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.Zero);

            var formatter = new SearchRequestFormatter(connection, mapping, new SearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = body["min_score"];
            Assert.Null(result);
        }

        [Fact]
        public void FormatTimeSpanWithMillisecondPrecisionIsUnquantifiedFormat()
        {
            var timespan = TimeSpan.FromMilliseconds(1500);
            var actual = SearchRequestFormatter.Format(timespan);

            Assert.Equal(timespan.TotalMilliseconds.ToString(CultureInfo.InvariantCulture), actual);
        }

        [Fact]
        public void FormatTimeSpanWithSecondPrecisionIsSecondFormat()
        {
            var timespan = TimeSpan.FromSeconds(3);
            var actual = SearchRequestFormatter.Format(timespan);

            Assert.Equal(timespan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + "s", actual);
        }

        [Fact]
        public void FormatTimeSpanWithMinutePrecisionIsMinuteFormat()
        {
            var timespan = TimeSpan.FromMinutes(4);
            var actual = SearchRequestFormatter.Format(timespan);

            Assert.Equal(timespan.TotalMinutes.ToString(CultureInfo.InvariantCulture) + "m", actual);
        }
    }
}