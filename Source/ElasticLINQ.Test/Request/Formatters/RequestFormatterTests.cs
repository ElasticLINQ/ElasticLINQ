// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Formatters;
using System;
using System.Globalization;
using Xunit;

namespace ElasticLinq.Test.Request.Formatters
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
    }
}