using System;
using System.Linq;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest.WhereTests
{
    public class DateWhereTests
    {
        private readonly DateTime joinDate = new DateTime(2015, 2, 4);

        [Fact]
        public void LessThanConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined < joinDate));
            DataAssert.Same<WebUser>(q => q.Where(w => joinDate < w.Joined));
        }

        [Fact]
        public void LessThanOrEqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined <= joinDate));
            DataAssert.Same<WebUser>(q => q.Where(w => joinDate <= w.Joined));
        }

        [Fact]
        public void GreaterThanConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined > joinDate));
            DataAssert.Same<WebUser>(q => q.Where(w => joinDate > w.Joined));
        }

        [Fact]
        public void GreaterThanOrEqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined >= joinDate));
            DataAssert.Same<WebUser>(q => q.Where(w => joinDate >= w.Joined));
        }

        [Fact]
        public void EqualToConstant()
        {
            var firstDate = DataAssert.Data.Memory<WebUser>().First().Joined;
            DataAssert.Same<WebUser>(q => q.Where(w => firstDate == w.Joined));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined == firstDate));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined.Equals(firstDate)));
            DataAssert.Same<WebUser>(q => q.Where(w => firstDate.Equals(w.Joined)));
            DataAssert.Same<WebUser>(q => q.Where(w => Equals(w.Joined, firstDate)));
            DataAssert.Same<WebUser>(q => q.Where(w => Equals(firstDate, w.Joined)));
        }

        [Fact]
        public void NotEqualToConstant()
        {
            var firstDate = DataAssert.Data.Memory<WebUser>().First().Joined;
            DataAssert.Same<WebUser>(q => q.Where(w => firstDate != w.Joined));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Joined != firstDate));
            DataAssert.Same<WebUser>(q => q.Where(w => !w.Joined.Equals(firstDate)));
            DataAssert.Same<WebUser>(q => q.Where(w => !firstDate.Equals(w.Joined)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Equals(w.Joined, firstDate)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Equals(firstDate, w.Joined)));
        }
    }
}