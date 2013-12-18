// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class ElasticTranslateResultTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            var expectedSearch = new ElasticSearchRequest { Type = "someType" };
            Func<ElasticResponse, object> expectedProjector = a => null;

            var result = new ElasticTranslateResult(expectedSearch, expectedProjector);

            Assert.Same(expectedSearch, result.SearchRequest);
            Assert.Same(expectedProjector, result.Materializer);

            Assert.Null(expectedProjector.Invoke(new ElasticResponse())); // For code coverage only
        }
    }
}