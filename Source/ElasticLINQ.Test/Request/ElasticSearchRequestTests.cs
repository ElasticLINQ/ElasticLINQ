// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticSearchRequestTests
    {
        [Fact]
        public void ConstructorSetsPropertiesFromRequiredParameters()
        {
            const string expectedType = "myType";

            var request = new ElasticSearchRequest(expectedType);

            Assert.Equal(expectedType, request.Type);
        }

        [Fact]
        public void ConstructorHasSensibleDefaultValues()
        {
            var request = new ElasticSearchRequest("myType");

            Assert.Equal(0, request.From);
            Assert.Null(request.Size);
            Assert.Empty(request.Fields);
            Assert.Empty(request.SortOptions);
            Assert.Null(request.Filter);
        }

        [Fact]
        public void ConstructorSetsPropertiesFromOptionalParameters()
        {
            const int expectedFrom = 101;
            var request = new ElasticSearchRequest("myType", expectedFrom);

            Assert.Equal(expectedFrom, request.From);
        }
    }
}