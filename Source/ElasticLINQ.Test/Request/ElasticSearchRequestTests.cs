// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticSearchRequestTests
    {
        [Fact]
        public void ConstructorHasSensibleDefaultValues()
        {
            var request = new ElasticSearchRequest();

            Assert.Equal(0, request.From);
            Assert.Null(request.Size);
            Assert.Empty(request.Fields);
            Assert.Empty(request.SortOptions);
            Assert.Null(request.Filter);
        }
    }
}