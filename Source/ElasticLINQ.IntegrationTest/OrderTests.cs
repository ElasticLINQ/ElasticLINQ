using System.Linq;
using ElasticLINQ.IntegrationTest.Models;
using Xunit;

namespace ElasticLINQ.IntegrationTest
{
    public class OrderTests
    {
        [Fact]
        public void OrderByInt()
        {
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Id)) ;
        }

        [Fact]
        public void OrderByString()
        {
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Username));
        }

        [Fact]
        public void OrderByDateTime()
        {
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Joined));
        }

        [Fact]
        public void OrderByStringThenByDateTime()
        {
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Forename).ThenBy(w => w.Joined));
        }

        [Fact]
        public void OrderByStringThenByDateTimeDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Forename).ThenByDescending(w => w.Joined));
        }

        [Fact]
        public void OrderByIntDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Id));
        }

        [Fact]
        public void OrderByStringDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Username));
        }

        [Fact]
        public void OrderByDateTimeDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Joined));
        }

        [Fact]
        public void OrderByStringDescendingThenByDateTime()
        {
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Forename).ThenBy(w => w.Joined));
        }

        [Fact]
        public void OrderByStringDescendingThenByDateTimeDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Forename).ThenByDescending(w => w.Joined));
        }
    }
}