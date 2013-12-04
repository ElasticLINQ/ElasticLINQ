// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class RegexpCriteriaTests
    {
        [Fact]
        public void NamePropertyIsRegexp()
        {
            var criteria = new RegexpCriteria("field", "regexp");

            Assert.Equal("regexp", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsProperties()
        {
            const string expectedField = "someField";
            const string expectedRegexp = "someRegexp";

            var criteria = new RegexpCriteria(expectedField, expectedRegexp);
            
            Assert.Equal(expectedField, criteria.Field);
            Assert.Equal(expectedRegexp, criteria.Regexp);
        }

        [Fact]
        public void ToStringContainsFieldAndRegex()
        {
            var criteria = new RegexpCriteria("field", "regexp");
            var result = criteria.ToString();

            Assert.Contains(criteria.Field, result);
            Assert.Contains(criteria.Regexp, result);
        }
    }
}