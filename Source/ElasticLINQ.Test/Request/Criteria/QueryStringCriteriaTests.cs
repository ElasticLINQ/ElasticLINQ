// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class QueryStringCriteriaTests
    {
        [Fact]
        public void ConstructorSetsPropertiesFromArguments()
        {
            const string expectedValue = "r2d2"; 

            var criteria = new QueryStringCriteria(expectedValue);

            Assert.Equal(expectedValue, criteria.Value);
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