// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class PrefixCriteriaTests
    {
        [Fact]
        public void NamePropertyIsPrefix()
        {
            var criteria = new PrefixCriteria("field", "aprefix");

            Assert.Equal("prefix", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsProperties()
        {
            const string expectedField = "someField";
            const string expectedPrefix = "somePrefix";

            var criteria = new PrefixCriteria(expectedField, expectedPrefix);
            
            Assert.Equal(expectedField, criteria.Field);
            Assert.Equal(expectedPrefix, criteria.Prefix);
        }

        [Fact]
        public void ToStringContainsFieldAndPrefix()
        {
            var criteria = new PrefixCriteria("field", "regexp");
            var result = criteria.ToString();

            Assert.Contains(criteria.Field, result);
            Assert.Contains(criteria.Prefix.ToString(), result);
        }
    }
}