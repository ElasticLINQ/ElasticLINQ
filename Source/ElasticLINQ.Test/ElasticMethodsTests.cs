// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticMethodsTests
    {
        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ContainsAllThrowsIfAccessed()
        {
            var set = new[] { 1, 2, 3 };
            var ex = Assert.Throws<InvalidOperationException>(() => ElasticMethods.ContainsAll(set, set));
            Assert.Contains("ElasticMethods.ContainsAll", ex.Message);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ContainsAnyThrowsIfAccessed()
        {
            var set = new[] { 1, 2, 3 };
            var ex = Assert.Throws<InvalidOperationException>(() => ElasticMethods.ContainsAny(set, set));
            Assert.Contains("ElasticMethods.ContainsAny", ex.Message);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void PrefixThrowsIfAccessed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => ElasticMethods.Prefix("a", "b"));
            Assert.Contains("ElasticMethods.Prefix", ex.Message);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void RegexpThrowsIfAccessed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => ElasticMethods.Regexp("a", "b"));
            Assert.Contains("ElasticMethods.Regexp", ex.Message);
        }
    }
}