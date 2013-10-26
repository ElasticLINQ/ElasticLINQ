// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
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
        private static readonly ElasticConnection defaultConnection = new ElasticConnection(new Uri("http://a.b.com:9000/"), TimeSpan.FromSeconds(10));

        [Fact]
        public void UrlPathContainsTypeSpecifier()
        {
            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1"));

            Assert.Contains("type1", formatter.Uri.AbsolutePath);
        }

        [Fact]
        public void BodyIsJsonFormattedResponse()
        {
            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1"));

            Assert.DoesNotThrow(() => JObject.Parse(formatter.Body));
        }

        [Fact]
        public void BodyContainsFilterTerms()
        {
            var termCriteria = new TermCriteria("term1", "criteria1", "criteria2");

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1", filter: termCriteria));
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "filter", "terms");
            var actualTerms = TraverseWithAssert(result, termCriteria.Field);
            foreach (var criteria in termCriteria.Values)
                Assert.Contains(criteria, actualTerms.Select(t => t.ToString()).ToArray());
        }

        [Fact]
        public void BodyContainsSortOptions()
        {
            var desiredSortOptions = new List<SortOption> { new SortOption("first", true), new SortOption("second", false) };

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1", sortOptions: desiredSortOptions));
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "sort");
            for (var i = 0; i < desiredSortOptions.Count; i++)
            {
                var actualSort = result[i];
                var desiredSort = desiredSortOptions[i];
                if (desiredSort.Ascending)
                {
                    Assert.Equal(actualSort, desiredSort.Name);
                }
                else
                {
                    var finalActualSort = actualSort[desiredSort.Name];
                    Assert.NotNull(finalActualSort);
                    Assert.Equal("desc", finalActualSort.ToString());
                }
            }
        }

        [Fact]
        public void BodyContainsFieldSelections()
        {
            var desiredFields = new List<string> { "first", "second", "third" };

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1", fields: desiredFields));
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "fields");
            foreach (var field in desiredFields)
                Assert.Contains(field, result);
        }

        [Fact]
        public void BodyContainsFromWhenSpecified()
        {
            const int expectedFrom = 1024;

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1", from: expectedFrom));
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "from");
            Assert.Equal(expectedFrom, result);
        }

        [Fact]
        public void BodyContainsSizeWhenSpecified()
        {
            const int expectedSize = 4096;

            var formatter = new PostBodyRequestFormatter(defaultConnection, new ElasticSearchRequest("type1", size: expectedSize));
            var body = JObject.Parse(formatter.Body);

            var result = TraverseWithAssert(body, "size");
            Assert.Equal(expectedSize, result);
        }

        private static JToken TraverseWithAssert(JToken token, params string[] paths)
        {
            foreach (var path in paths)
            {
                Assert.NotNull(token);
                token = token[path];
            }

            return token;
        }
    }
}