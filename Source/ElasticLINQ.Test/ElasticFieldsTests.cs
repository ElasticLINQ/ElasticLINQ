// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticFieldsTests
    {
        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ScoreThrowsIfAccessed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => ElasticFields.Score);
            Assert.Contains("ElasticFields.Score", ex.Message);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void IdThrowsIfAccessed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => ElasticFields.Id);
            Assert.Contains("ElasticFields.Id", ex.Message);
        }
    }
}