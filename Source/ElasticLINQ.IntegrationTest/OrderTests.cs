// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest
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
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Username).ThenBy(w => w.Joined));
        }

        [Fact]
        public void OrderByStringThenByDateTimeDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderBy(w => w.Username).ThenByDescending(w => w.Joined));
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
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Username).ThenBy(w => w.Joined));
        }

        [Fact]
        public void OrderByStringDescendingThenByDateTimeDescending()
        {
            DataAssert.Same<WebUser>(q => q.OrderByDescending(w => w.Username).ThenByDescending(w => w.Joined));
        }
    }
}