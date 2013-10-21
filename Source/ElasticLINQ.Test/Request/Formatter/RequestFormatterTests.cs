// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using ElasticLinq.Request;
using ElasticLinq.Request.Formatter;
using System;
using System.Globalization;
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
        public void CreateReturnsGetPostBodyRequestFormatterByDefault()
        {
            var connection = new ElasticConnection(new Uri("http://a.b.com/d"), TimeSpan.FromSeconds(1));

            var formatter = RequestFormatter.Create(connection, new ElasticSearchRequest("R-Type"));

            Assert.IsType<PostBodyRequestFormatter>(formatter);
        }
    }
}