﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Formatters;
using ElasticLinq.Test.TestSupport;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;
using ElasticLinq.Connection;

namespace ElasticLinq.Test.Request.Formatters
{
    public class SearchRequestFormatterTests
    {
        private static readonly IElasticConnection defaultConnection = new HttpElasticConnection(new Uri("http://a.b.com:9000/"));
        private static readonly MemberInfo memberInfo = typeof(string).GetProperty("Length");
        private readonly IElasticMapping mapping = Substitute.For<IElasticMapping>();

        public SearchRequestFormatterTests()
        {
            mapping.FormatValue(null, null)
                   .ReturnsForAnyArgs(callInfo => new JValue(String.Format("!!! {0} !!!", callInfo.Arg<object>(1))));
        }

        [Theory]
        [InlineData(null, null, "http://a.b.com:9000/_all/_search")]
        [InlineData("index1,index2", null, "http://a.b.com:9000/index1,index2/_search")]
        [InlineData(null, "docType1,docType2", "http://a.b.com:9000/_all/docType1,docType2/_search")]
        [InlineData("index1,index2", "docType1,docType2", "http://a.b.com:9000/index1,index2/docType1,docType2/_search")]
        public void UriFormatting(string index, string documentType, string expectedUri)
        {
            var connection = new HttpElasticConnection(new Uri("http://a.b.com:9000/"), index: index);
            var formatter = new SearchRequestFormatter(connection, mapping, new SearchRequest { DocumentType = documentType });

            Assert.Equal(expectedUri, formatter.Uri.ToString());
        }

        [Fact]
        public void BodyIsValidJsonFormattedResponse()
        {
            var formatter = new SearchRequestFormatter(defaultConnection, mapping, new SearchRequest { DocumentType = "type1" });

            Assert.DoesNotThrow(() => JObject.Parse(formatter.Body));
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
            var connection = new HttpElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.FromSeconds(15));

            var formatter = new SearchRequestFormatter(connection, mapping, new SearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = body.TraverseWithAssert("timeout");
            Assert.Equal(expectedTimeout, result);
        }

        [Fact]
        public void BodyDoesNotContainTimeoutWhenZero()
        {
            var connection = new HttpElasticConnection(new Uri("http://localhost/"), timeout: TimeSpan.Zero);

            var formatter = new SearchRequestFormatter(connection, mapping, new SearchRequest());
            var body = JObject.Parse(formatter.Body);

            var result = body["timeout"];
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

        [Fact]
        public void SearchTypeAppearsOnUriWhenSpecified()
        {
            var searchRequest = new SearchRequest { SearchType = "count" };

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, searchRequest);

            var parameters = formatter.Uri.OriginalString.Split('&');

            Assert.Single(parameters, p => p == "search_type=count");
        }

        [Fact]
        public void SearchTypeDoesNotAppearOnUriWhenNotSpecified()
        {
            var searchRequest = new SearchRequest { SearchType = "" };

            var formatter = new SearchRequestFormatter(defaultConnection, mapping, searchRequest);

            var parameters = formatter.Uri.OriginalString.Split('&');

            Assert.None(parameters, p => p.StartsWith("search_type="));
        }
    }
}