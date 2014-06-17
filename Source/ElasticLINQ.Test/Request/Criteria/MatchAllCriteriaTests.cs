// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class MatchAllCriteriaTests
    {
        [Fact]
        public void NamePropertyIsMatchAll()
        {
            var criteria = new MatchAllCriteria();

            Assert.Equal("match_all", criteria.Name);
        }
    }
}