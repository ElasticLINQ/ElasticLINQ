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
        public void ScoreThrowsIfAccessed()
        {
            Assert.Throws<InvalidOperationException>(() => ElasticMethods.Regexp("a", "b"));
        }
    }
}