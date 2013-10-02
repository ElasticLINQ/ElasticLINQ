// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq;
using ElasticLinq.Request;
using ElasticLinq.Request.Formatter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Request.Formatter
{
    public class RequestFormatterTests
    {
        [Fact]
        public void FormatTimeSpanWithMillisecondPrecisionIsUnquantifiedFormat()
        {
            var timespan = TimeSpan.FromMilliseconds(1500);
            var actual = RequestFormatter.Format(timespan);

            Assert.Equal(timespan.TotalMilliseconds.ToString(CultureInfo.InvariantCulture), actual);
        }

        [Fact]
        public void FormatTimeSpanWithSecondPrecisionIsSecondFormat()
        {
            var timespan = TimeSpan.FromSeconds(3);
            var actual = RequestFormatter.Format(timespan);

            Assert.Equal(timespan.TotalSeconds.ToString(CultureInfo.InvariantCulture) + "s", actual);
        }

        [Fact]
        public void FormatTimeSpanWithMinutePrecisionIsMinuteFormat()
        {
            var timespan = TimeSpan.FromMinutes(4);
            var actual = RequestFormatter.Format(timespan);

            Assert.Equal(timespan.TotalMinutes.ToString(CultureInfo.InvariantCulture) + "m", actual);
        }

        [Fact]
        public void CreateReturnsGetQueryRequestFormatterWhenPreferredAndPossible()
        {
            var connection = new ElasticConnection(new Uri("http://a.b.com/d"), TimeSpan.FromSeconds(1), preferGetRequests:true);
            
            var formatter = RequestFormatter.Create(connection, new ElasticSearchRequest("R-Type"));

            Assert.IsType<GetQueryRequestFormatter>(formatter);
        }

        [Fact]
        public void CreateReturnsPostBodyRequestFormatterWhenPreferredButNotPossible()
        {
            var connection = new ElasticConnection(new Uri("http://a.b.com/d"), TimeSpan.FromSeconds(1), preferGetRequests: true);
            var terms = new Dictionary<string, IReadOnlyList<object>>
            {
                { "firstTerm", new [] { "1st" }.ToList().AsReadOnly() },
                { "secondTerm", new [] { "2nd" }.ToList().AsReadOnly() },
            };

            var formatter = RequestFormatter.Create(connection, new ElasticSearchRequest("R-Type", termCriteria: terms));

            Assert.IsType<PostBodyRequestFormatter>(formatter);
        }

        [Fact]
        public void CreateReturnsGetPostBodyRequestFormatterByDefault()
        {
            var connection = new ElasticConnection(new Uri("http://a.b.com/d"), TimeSpan.FromSeconds(1));

            var formatter = RequestFormatter.Create(connection, new ElasticSearchRequest("R-Type"));

            Assert.IsType<PostBodyRequestFormatter>(formatter);
        }
    }
}