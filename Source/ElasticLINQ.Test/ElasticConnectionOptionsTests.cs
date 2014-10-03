// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Xunit;

namespace ElasticLinq.Test
{
    public class ElasticConnectionOptionsTests
    {
        [Fact]
        public void PrettyDefaultIsFalse()
        {
            var actual = new ElasticConnectionOptions();

            Assert.False(actual.Pretty);
        }
    }
}