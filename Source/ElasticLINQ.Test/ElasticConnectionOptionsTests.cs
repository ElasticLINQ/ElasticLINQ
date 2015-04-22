// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionOptionsTests
    {
        [Fact]
        public void PrettyDefaultsToFalse()
        {
            var actual = new ElasticConnectionOptions();

            Assert.False(actual.Pretty);
        }

        [Fact]
        public void PrettyCanBeSet()
        {
            var actual = new ElasticConnectionOptions { Pretty = true };

            Assert.True(actual.Pretty);
        }

        [Fact]
        public void SearchSizeDefaultDefaultsToNull()
        {
            var actual = new ElasticConnectionOptions();

            Assert.Null(actual.SearchSizeDefault);
        }

        [Fact]
        public void SearchSizeDefaultCanBeSet()
        {
            const int expected = 50000;
            var actual = new ElasticConnectionOptions { SearchSizeDefault = expected };

            Assert.Equal(expected, actual.SearchSizeDefault);
        }
    }
}