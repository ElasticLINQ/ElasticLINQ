// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using System;
using System.Collections;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class ElasticTranslateResultTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            var expectedSearch = new ElasticSearchRequest { Type = "someType" };
            Func<Hit, object> expectedProjector = h => null;
            Func<IEnumerable, object> expectedFinalTransform = a => a;

            var result = new ElasticTranslateResult(expectedSearch, expectedProjector, expectedFinalTransform);

            Assert.Same(expectedSearch, result.SearchRequest);
            Assert.Same(expectedProjector, result.ItemCreator);
            Assert.Same(expectedFinalTransform, result.ResultCreator);

            Assert.Null(expectedProjector.Invoke(new Hit())); // For code coverage only
            Assert.Null(expectedFinalTransform.Invoke(null)); // For code coverage only
        }
    }
}