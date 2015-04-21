using System;
using System.Linq;
using ElasticLINQ.IntegrationTest.Models;
using Xunit;

namespace ElasticLINQ.IntegrationTest
{
    public class GroupByTests
    {
        [Fact]
        public void GroupByStringSelectCount()
        {
            DataAssert.Same((IQueryable<WebUser> q) => 
                q.GroupBy(w => w.Forename).Select(g => Tuple.Create(g.Key, g.Count())), ignoreOrder: true);
        }
    }
}
