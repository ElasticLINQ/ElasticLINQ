// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using ElasticLinq.Request.Criteria;
using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class QueryStringCriteriaTests
    {
        [Fact]
        public void ConstructorWithOneParameterSetsPropertiesFromArguments()
        {
            const string expectedValue = "r2d2"; 

            var criteria = new QueryStringCriteria(expectedValue);

            Assert.Equal(expectedValue, criteria.Value);
            Assert.Equal(Enumerable.Empty<string>(), criteria.Fields);
        }

        [Fact]
        public void ConstructorWithAllParametersSetsPropertiesFromArguments()
        {
            const string expectedValue = "c3po";
            var expectedFields = new[] { "jawas", "homestead" };

            var criteria = new QueryStringCriteria(expectedValue, expectedFields);

            Assert.Equal(expectedValue, criteria.Value);
            Assert.Equal(expectedFields, criteria.Fields);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryStringCriteria(null));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenValueIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new QueryStringCriteria(""));
        }
    }
}